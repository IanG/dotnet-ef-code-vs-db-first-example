using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFExample.Common.Data.CodeFirst.Entitles;

[Table("customers")]
[Index(nameof(Email), IsUnique = true, Name = "customers_email_key")]
public class Customer
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key, Column("id", TypeName = "integer", Order = 1)]
    public int Id { get; set; }

    [Required]
    [Column("first_name", TypeName = "varchar(50)", Order = 2)]
    public string FirstName { get; set; } = null!;

    [Required]
    [Column("last_name", TypeName = "varchar(50)", Order = 3)]
    public string LastName { get; set; } = null!;
    
    [Column("email", TypeName = "varchar(100)", Order = 4)]
    public string Email { get; set; } = null!;

    [Column("phone", TypeName = "varchar(15)", Order = 5)]
    public string? Phone { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone", Order = 6)]
    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}