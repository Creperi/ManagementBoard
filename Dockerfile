FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BasicManagementBoard/BasicManagementBoard.csproj", "BasicManagementBoard/"]
RUN dotnet restore "./BasicManagementBoard/./BasicManagementBoard.csproj"
COPY . .
WORKDIR "/src/BasicManagementBoard"
RUN dotnet build "./BasicManagementBoard.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BasicManagementBoard.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BasicManagementBoard.dll"]