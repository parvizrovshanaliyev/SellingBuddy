using CatalogService.Api.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Api.Infrastructure.Configurations;

public class CatalogBrandConfiguration : IEntityTypeConfiguration<CatalogBrand>
{
    public void Configure(EntityTypeBuilder<CatalogBrand> builder)
    {
        builder.ToTable("CatalogBrands");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Brand)
            .IsRequired()
            .HasMaxLength(50);
    }
}

public class CatalogTypeConfiguration : IEntityTypeConfiguration<CatalogType>
{
    public void Configure(EntityTypeBuilder<CatalogType> builder)
    {
        builder.ToTable("CatalogTypes");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Type)
            .IsRequired()
            .HasMaxLength(50);
    }
}

public class CatalogItemConfiguration : IEntityTypeConfiguration<CatalogItem>
{
    public void Configure(EntityTypeBuilder<CatalogItem> builder)
    {
        builder.ToTable("CatalogItems");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(i => i.Description)
            .HasMaxLength(500);
        builder.Property(i => i.Price)
            .HasColumnType("decimal(18, 2)");
        builder.Property(i => i.PictureFileName)
            .HasMaxLength(255);
        builder.Property(i => i.PictureUri)
            .HasMaxLength(1024);

        builder.HasOne(i => i.CatalogType)
            .WithMany()
            .HasForeignKey(i => i.CatalogTypeId);

        builder.HasOne(i => i.CatalogBrand)
            .WithMany()
            .HasForeignKey(i => i.CatalogBrandId);
    }
}