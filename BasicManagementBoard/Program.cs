using BasicManagementBoard.Controllers;
using BasicManagementBoard.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using System;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// Add services to the container
builder.Services.AddControllers();

// Configure MySQL Data Source
builder.Services.AddMySqlDataSource(builder.Configuration.GetConnectionString("Default")!);

// Configure Entity Framework for Task and Project contexts
builder.Services.AddDbContext<TaskContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("Default"), new MySqlServerVersion(new Version(8, 0, 27))));

builder.Services.AddDbContext<ProjectContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("Default"), new MySqlServerVersion(new Version(8, 0, 27))));

//API Explorer for endpoint information

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Management Board API", Version = "v1" });
});

//Swagger for API documentation

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    //Swagger UI in development environment
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Management Board API V1");
    });
}
app.UseRouting();

app.UseCors(options => options.AllowAnyOrigin());


// Enable authorization (if needed)
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.Run();
