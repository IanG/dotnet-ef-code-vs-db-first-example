using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFExample.Common.Data.CodeFirst.Entitles;

[Table("products")]
public class Product
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key, Column("id", TypeName = "integer", Order = 1)]
    public int Id { get; set; }

    [Column("product_name", TypeName = "varchar(100)", Order = 2)]
    public string ProductName { get; set; } = null!;

    [Column("price", TypeName = "numeric(10, 2)", Order = 3)]
    public decimal Price { get; set; }
    
    [Column("stock_quantity", TypeName = "integer", Order = 4)]
    public int StockQuantity { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone", Order = 6)]
    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
}
