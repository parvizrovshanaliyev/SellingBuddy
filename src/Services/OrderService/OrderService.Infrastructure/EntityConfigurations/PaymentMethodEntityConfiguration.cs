namespace OrderService.Infrastructure.EntityConfigurations;

public class PaymentMethodEntityConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("paymentMethods", OrderDbContext.DEFAULT_SCHEMA);

        builder.HasKey(pm => pm.Id);
        builder.Property(pm => pm.Id).ValueGeneratedOnAdd();

        builder.Ignore(pm => pm.DomainEvents);
        
        builder.Property(pm => pm.BuyerId)
            .IsRequired();
        
        builder.Property(pm => pm.CardHolderName)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(pm => pm.Alias)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(pm => pm.CardNumber)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasMaxLength(25)
            .IsRequired();

        builder.Property(pm => pm.Expiration)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasMaxLength(25)
            .IsRequired();
        
        builder.Property(pm => pm.CardTypeId)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired();
        
        builder.Property(pm => pm.SecurityNumber)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasMaxLength(3)
            .IsRequired(false);
        
        builder.HasOne(pm => pm.CardType)
            .WithMany()
            .HasForeignKey(pm => pm.CardTypeId);
    }
}