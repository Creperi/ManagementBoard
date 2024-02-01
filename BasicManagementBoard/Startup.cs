using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using BasicManagementBoard.Models;
using Microsoft.OpenApi.Models;

namespace BasicManagementBoard
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Adding services to the container
            services.AddControllers();

            // Configuring MySQL Data Source
            services.AddMySqlDataSource(Configuration.GetConnectionString("Default")!);

            // Configure Entity Framework for Task and Project contexts
            services.AddDbContext<TaskContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("Default"), new MySqlServerVersion(new Version(8, 0, 27))));

            services.AddDbContext<ProjectContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("Default"), new MySqlServerVersion(new Version(8, 0, 27))));

            // Swagger for API documentation
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Management Board API", Version = "v1" });
            });

            // Adding API Explorer for endpoint information
            services.AddEndpointsApiExplorer();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure HTTP request pipeline
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Swagger UI in the development environment
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Management Board API V1");
            });

            app.UseRouting();

            // Enable CORS (Cross-Origin Resource Sharing)
            app.UseCors(options => options.AllowAnyOrigin());

            // Enable authorization (if needed)
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
