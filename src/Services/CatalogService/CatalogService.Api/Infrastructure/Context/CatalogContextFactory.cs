namespace CatalogService.Api.Infrastructure.Context;

// public class CatalogContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
// {
//     public CatalogDbContext CreateDbContext(string[] args)
//     {
//         IConfigurationRoot configuration = new ConfigurationBuilder()
//             .SetBasePath(Directory.GetCurrentDirectory())
//             .AddJsonFile("appsettings.json")
//             .Build();
//
//         var optionsBuilder = new DbContextOptionsBuilder<CatalogDbContext>();
//         var connectionString = configuration.GetConnectionString("CatalogConnection"); // Update connection string key
//
//         optionsBuilder.UseSqlServer(connectionString, options =>
//         {
//             options.MigrationsHistoryTable("__CatalogMigrationsHistory"); // Update history table and schema if needed
//         });
//
//         return new CatalogDbContext(optionsBuilder.Options);
//     }
// }