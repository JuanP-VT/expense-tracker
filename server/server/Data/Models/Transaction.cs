using server.Data.Annotations;
using server.Data.Enums;
using System.ComponentModel.DataAnnotations;
namespace server.Data.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required]
        public TransactionType Type { get; set; }
        [HigherThanCero]
        public decimal Amount { get; set; }
        [Required]
        public string Category { get; set; } = string.Empty;
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public User User { get; set; }
    }
}
