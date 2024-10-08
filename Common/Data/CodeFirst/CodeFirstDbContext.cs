using EFExample.Common.Data.CodeFirst.Entitles;
using Microsoft.EntityFrameworkCore;

namespace EFExample.Common.Data.CodeFirst;

public class CodeFirstDbContext : DbContext
{
    public CodeFirstDbContext(DbContextOptions<CodeFirstDbContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; } 
    public DbSet<Order> Orders { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public"); 
        
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("customers_pkey");
            
            entity.Property(e => e.Id)
                .UseIdentityColumn();
            
            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("customers_email_key");
           
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
        });
        
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orders_pkey");

            entity.Property(e => e.CustomerId)
                .IsRequired()
                .HasColumnName("customer_id")
                .HasColumnType("integer");

            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");

            entity.Property(e => e.TotalAmount)
                .HasColumnName("total_amount")
                .HasColumnType("numeric(10, 2)");

            // Configure the foreign key relationship
            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("orders_customer_id_fkey");
            
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.CustomerId)
                .HasDatabaseName("orders_customer_id_idx")
                .IsUnique(false); 
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("suppliers_pkey");
            
            entity.ToTable("suppliers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ContactName)
                .HasMaxLength(100)
                .HasColumnName("contact_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(100)
                .HasColumnName("supplier_name");
        });
        
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_items_pkey");

            entity.ToTable("order_items");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PricePerItem)
                .HasPrecision(10, 2)
                .HasColumnName("price_per_item");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("order_items_order_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("order_items_product_id_fkey");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("products_pkey");

            entity.ToTable("products");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .HasColumnName("product_name");
            entity.Property(e => e.StockQuantity).HasColumnName("stock_quantity");

            entity.HasMany(d => d.Suppliers).WithMany(p => p.Products)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductSupplier",
                    r => r.HasOne<Supplier>().WithMany()
                        .HasForeignKey("SupplierId")
                        .HasConstraintName("product_suppliers_supplier_id_fkey"),
                    l => l.HasOne<Product>().WithMany()
                        .HasForeignKey("ProductId")
                        .HasConstraintName("product_suppliers_product_id_fkey"),
                    j =>
                    {
                        j.HasKey("ProductId", "SupplierId").HasName("product_suppliers_pkey");
                        j.ToTable("product_suppliers");
                        j.IndexerProperty<int>("ProductId").HasColumnName("product_id");
                        j.IndexerProperty<int>("SupplierId").HasColumnName("supplier_id");
                    });
        });
    }
}