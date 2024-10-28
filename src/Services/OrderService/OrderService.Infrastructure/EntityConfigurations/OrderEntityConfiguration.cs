namespace OrderService.Infrastructure.EntityConfigurations;

public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders", OrderDbContext.DEFAULT_SCHEMA);
        
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedOnAdd();
        
        builder.Ignore(o => o.DomainEvents);
        
        builder.OwnsOne(o => o.Address, a =>
        {
            a.WithOwner();
            
            // a.Property(a => a.Street).HasColumnName("Street").HasMaxLength(200).IsRequired();
            // a.Property(a => a.City).HasColumnName("City").HasMaxLength(100).IsRequired();
            // a.Property(a => a.State).HasColumnName("State").HasMaxLength(100).IsRequired();
            // a.Property(a => a.Country).HasColumnName("Country").HasMaxLength(100).IsRequired();
            // a.Property(a => a.ZipCode).HasColumnName("ZipCode").HasMaxLength(18).IsRequired();
        });
        
        builder.Property<int>("orderStatusId")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("OrderStatusId")
            .IsRequired();
        
        var navigation = builder.Metadata.FindNavigation(nameof(Order.OrderItems));
        
        navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        
        builder.HasOne(o => o.OrderStatus)
            .WithMany()
            .HasForeignKey("orderStatusId");
        
        builder.HasOne(o => o.Buyer)
            .WithMany()
            .HasForeignKey(o => o.BuyerId);
    }
}