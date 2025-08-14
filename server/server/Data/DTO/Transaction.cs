using server.Data.Annotations;
using server.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace server.Data.DTO
{
    public class Transaction
    {
        [Required]
        public TransactionType Type { get; set; }
        [HigherThanCero]
        public decimal Amount { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        [NoFutureDateTime]
        public DateTime Date { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;
    }
}