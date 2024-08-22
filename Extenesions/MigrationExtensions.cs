using Microsoft.EntityFrameworkCore;
using UrlShortner;

namespace Web.Api.Extensions;

public static class MigrationExtension
{
    public static void ApplayMigrations (IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        {
            var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

            if (dbContext == null)
            {
                throw new InvalidOperationException("ApplicationDbContext could not be resolved.");
            }
            System.Console.WriteLine("Appling Migrations...");
            dbContext.Database.Migrate();
        }
    }
}