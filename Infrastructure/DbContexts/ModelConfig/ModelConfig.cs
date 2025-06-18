using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DbContexts.ModelConfig
{
    public class CustomerConfig : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.Email)
                   .HasMaxLength(100);

            builder.Property(c => c.Phone)
                   .HasMaxLength(20);

            builder.Property(c => c.CreatedAt)
                   .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasMany(c => c.Orders)
                   .WithOne(o => o.Customer)
                   .HasForeignKey(o => o.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Description)
                   .HasMaxLength(250);

            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql("SYSUTCDATETIME()");
        }
    }

    public class OrderConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.OriginLatitude).IsRequired();
            builder.Property(o => o.OriginLongitude).IsRequired();
            builder.Property(o => o.DestinationLatitude).IsRequired();
            builder.Property(o => o.DestinationLongitude).IsRequired();

            builder.Property(o => o.DistanceKm).IsRequired().HasPrecision(10, 2);
            builder.Property(o => o.ShippingCostUSD).IsRequired().HasPrecision(10, 2);

            builder.Property(o => o.Status).IsRequired().HasDefaultValue(1);

            builder.Property(o => o.CreatedAt)
                   .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.Property(o => o.UpdatedAt);
            builder.Property(o => o.DeletedAt);

            builder.HasOne(o => o.Customer)
                   .WithMany(c => c.Orders)
                   .HasForeignKey(o => o.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(o => o.OrderItems)
                   .WithOne(oi => oi.Order)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class OrderItemConfig : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.Quantity)
                   .IsRequired()
                   .HasDefaultValue(1);

            builder.HasOne(oi => oi.Order)
                   .WithMany(o => o.OrderItems)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(oi => oi.Product)
                   .WithMany(p => p.OrderItems)
                   .HasForeignKey(oi => oi.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
    public class ShippingCostRangeConfig : IEntityTypeConfiguration<ShippingCostRange>
    {
        public void Configure(EntityTypeBuilder<ShippingCostRange> builder)
        {
            builder.ToTable("ShippingCostRanges");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.MinKm).IsRequired().HasPrecision(10, 2);
            builder.Property(s => s.MaxKm).IsRequired().HasPrecision(10, 2);
            builder.Property(s => s.CostUSD).IsRequired().HasPrecision(10, 2);

            builder.Property(s => s.CreatedAt)
                   .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.Property(s => s.UpdatedAt);
        }
    }
}
