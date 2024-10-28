namespace OrderService.Infrastructure.EntityConfigurations;

public class OrderStatusEntityConfiguration : IEntityTypeConfiguration<OrderStatus>
{
    public void Configure(EntityTypeBuilder<OrderStatus> builder)
    {
        builder.ToTable("orderStatuses", OrderDbContext.DEFAULT_SCHEMA);

        builder.HasKey(os => os.Id);
        builder.Property(os => os.Id).ValueGeneratedOnAdd();

        builder.Property(os => os.Id)
            .HasDefaultValue(1)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(os => os.Name)
            .HasMaxLength(200)
            .IsRequired();
    }
}