using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccessManagementSystem_API.Models
{
    [Table("tbl_product")]
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        [Unicode(false)]
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? ImagePath { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // Default to current UTC time

        //[NotMapped] // Exclude from database mapping
        //public IFormFile? File { get; set; }
    }
}
