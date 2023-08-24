using CatalogService.Api.Core.Domain;
using CatalogService.Api.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Api.Infrastructure.Context;

public class CatalogDbContext : DbContext
{
    public const string DEFAULT_SCHEMA = "catalog";

    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<CatalogBrand> CatalogBrands { get; set; }
    public DbSet<CatalogItem> CatalogItems { get; set; }
    public DbSet<CatalogType> CatalogTypes { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Add your entity configurations here
        modelBuilder.ApplyConfiguration(new CatalogBrandConfiguration());
        modelBuilder.ApplyConfiguration(new CatalogTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CatalogItemConfiguration());
    }
}