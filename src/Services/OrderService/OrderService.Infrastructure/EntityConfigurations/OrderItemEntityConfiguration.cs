namespace OrderService.Infrastructure.EntityConfigurations;

public class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("orderItems", OrderDbContext.DEFAULT_SCHEMA);
        
        builder.HasKey(oi => oi.Id);
        builder.Property(oi => oi.Id).ValueGeneratedOnAdd();
        
        builder.Ignore(oi => oi.DomainEvents);
        
        builder.Property<int>(nameof(OrderItem.OrderId)).IsRequired();
    }
}