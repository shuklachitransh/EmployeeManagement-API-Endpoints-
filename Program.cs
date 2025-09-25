using EmployeeManagement.Data;
using EmployeeManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // ✅ Register DbContext with SQL Server
            builder.Services.AddDbContext<AppDbContext>(options =>
              options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 🆕 Register Employee Service
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();

            // 🆕 Add API Controllers
            builder.Services.AddControllers();

            // 🆕 Add CORS for API access
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // 🆕 Add Swagger for API documentation
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "Employee Management API", Version = "v1" });
                c.SwaggerDoc("v2", new() { Title = "Employee Management Advanced API", Version = "v2" });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                // 🆕 Enable Swagger in development
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee Management API v1");
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "Employee Management Advanced API v2");
                });
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // 🆕 Enable CORS
            app.UseCors("AllowAll");

            app.UseRouting();

            app.UseAuthorization();

            // 🆕 Map API Controllers
            app.MapControllers();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Employee}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
