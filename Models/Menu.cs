using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AccessManagementSystem_API.Models
{
    [Table("tbl_menu")]
    public class Menu
    {
        [Key]
        [Column("code")]
        [StringLength(50)]
        public string Code { get; set; } = null!;

        [Column("name")]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        [Column("status")]
        public bool? Status { get; set; }
    }
}
