# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
EXPOSE 80/tcp


# Copy only the project file and restore dependencies
COPY ["BasicManagementBoard/BasicManagementBoard.csproj", "BasicManagementBoard/"]
RUN dotnet restore "./BasicManagementBoard/BasicManagementBoard.csproj"

# Copy the entire project and build
COPY . .
WORKDIR "/src/BasicManagementBoard"
RUN dotnet build "./BasicManagementBoard.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "./BasicManagementBoard.csproj" -c Release -o /app/publish

# Stage 3: Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
USER app
WORKDIR /app

# Copy the published output from the publish stage
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "BasicManagementBoard.dll"]
