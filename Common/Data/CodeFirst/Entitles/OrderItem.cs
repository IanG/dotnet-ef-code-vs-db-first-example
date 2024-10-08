using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFExample.Common.Data.CodeFirst.Entitles;

[Table("order_items")]
public class OrderItem
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key, Column("id", TypeName = "integer", Order = 1)]
    public int Id { get; set; }
    
    [Key, Column("order_id", TypeName = "integer", Order = 2)]
    public int OrderId { get; set; }
    
    [Key, Column("product_id", TypeName = "integer", Order = 3)]
    public int ProductId { get; set; }

    [Key, Column("quantity", TypeName = "integer", Order = 4)]
    public int Quantity { get; set; }

    [Column("price_per_item", TypeName = "numeric(10, 2)", Order = 5)]
    public decimal PricePerItem { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
