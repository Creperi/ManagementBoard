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

//Swagger for API documentation
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    //Swagger UI in development environment
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Enable authorization (if needed)
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.Run();
