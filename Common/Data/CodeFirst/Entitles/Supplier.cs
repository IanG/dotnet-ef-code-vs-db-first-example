using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFExample.Common.Data.CodeFirst.Entitles;

[Table("suppliers")]
public class Supplier
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key, Column("id", TypeName = "integer", Order = 1)]
    public int Id { get; set; }

    [Required]
    [Column("supplier_name", TypeName = "varchar(100)", Order = 2)]
    public string SupplierName { get; set; } = null!;

    [Column("contact_name", TypeName = "varchar(100)", Order = 3)]
    public string? ContactName { get; set; }

    [Column("phone", TypeName = "varchar(15)", Order = 4)]
    public string? Phone { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone", Order = 5)]
    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
