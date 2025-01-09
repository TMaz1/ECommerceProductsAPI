using ECommerceProductsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceProductsAPI.Data;

public class ProductsDataContext(DbContextOptions<ProductsDataContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();
    public DbSet<ProductVariation> ProductVariations => Set<ProductVariation>();
    public DbSet<ProductVariationAttribute> ProductVariationAttributes => Set<ProductVariationAttribute>();
    public DbSet<GroupedProductItem> GroupedProductItems => Set<GroupedProductItem>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // prevent deletion of attributes shared between products
        modelBuilder.Entity<ProductVariationAttribute>()
            .HasOne(va => va.Attribute)
            .WithMany(a => a.VariationAttributes)
            .HasForeignKey(va => va.AttributeId)
            .OnDelete(DeleteBehavior.Restrict);

        // prevent GroupedProduct from being deleted if there are any existing GroupedProductItems
        modelBuilder.Entity<GroupedProductItem>()
            .HasOne(gpi => gpi.GroupedProduct)
            .WithMany(p => p.GroupedProductItems)
            .HasForeignKey(gpi => gpi.GroupedProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // ProductItem is deleted if it is part of any GroupedProductItem upon Product deletion
        modelBuilder.Entity<GroupedProductItem>()
            .HasOne(gpi => gpi.ProductItem)
            .WithMany()
            .HasForeignKey(gpi => gpi.ProductItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
