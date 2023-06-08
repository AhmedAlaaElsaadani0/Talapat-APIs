using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Talabat.API.Errors;
using Talabat.API.Extensions;
using Talabat.API.Helpers;
using Talabat.API.Middlewares;
using Talabat.Core.Entities.Identity;
using Talabat.Core.IRepositories;
using Talabat.Repository;
using Talabat.Repository.Data;
using Talabat.Repository.Identity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TalabatAPI
{
    public class Program
    {
        // Entry Point
        public static async Task Main(string[] args)
        {
            // Create App
            var builder = WebApplication.CreateBuilder(args);

            #region Configure Services
            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });

            builder.Services.AddSingleton<IConnectionMultiplexer>(options =>
            {
                var connection = ConfigurationOptions.Parse( builder.Configuration.GetConnectionString("Redis") );
                
                return ConnectionMultiplexer.Connect(connection);
            });

            builder.Services.AddApplicationServices();

            builder.Services.AddSwaggerServices();

            builder.Services.AddIdentityServices(builder.Configuration);

            #endregion

            // Build App
            var app = builder.Build();

            #region Seeding Data and Apply Migrations if exist, Then Update Database & Log Exception if exist also.
            // using: to dispose connection
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            try
            {
                // ASK CLR To Create dbContext Explicitly
                var dbContext = services.GetRequiredService<StoreContext>();

                // If any Migrations created, Apply it, then Update-Database
                await dbContext.Database.MigrateAsync();

                // Seeding Data 
                await StoreContextSeed.SeedAsync(dbContext);

                var IdentityDbContext = services.GetRequiredService<AppIdentityDbContext>();
                
                await IdentityDbContext.Database.MigrateAsync();

                var userManager = services.GetRequiredService<UserManager<AppUser>>();

                await AppIdentityDbContextSeed.SeedUsersAsync(userManager);
            }
            catch (Exception ex)
            {
                // Logging Error to Console [Kestrel]
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An Error Occured During apply the migration");
            } 
            #endregion

            #region Configue Kestrel Middlewares

            app.UseMiddleware<ExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerMiddlewares();
            }

            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.MapControllers();

            app.UseAuthentication();

            app.UseAuthorization();

            #endregion
            
            app.Run();
        }
    }
}