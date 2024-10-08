using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFExample.Common.Data.CodeFirst.Entitles;

[Table("orders")]
public class Order
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key, Column("id", TypeName = "integer", Order = 1)]
    public int Id { get; set; }

    [Required]
    [Column("customer_id", TypeName = "integer", Order = 2)]
    public int CustomerId { get; set; }

    
    [Column("order_date", TypeName = "timestamp without time zone", Order = 3)]
    public DateTime? OrderDate { get; set; }

    [Column("total_amount", TypeName = "numeric(10, 2)", Order = 4)]
    public decimal TotalAmount { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
