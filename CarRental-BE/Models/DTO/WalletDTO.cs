using CarRental_BE.Models.Enum;
using System;
using System.ComponentModel.DataAnnotations;

namespace CarRental_BE.Models.DTO
{
    public record TransactionDTO
    {
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public long Amount { get; set; }

        public string? Message { get; set; }

        public string? BookingId { get; set; }

        public string? CarName { get; set; }

        public TransactionType? Type { get; set; }

        public TransactionStatus? Status { get; set; }

    }

    public record TransactionFilterDTO
    {
        public string? SearchTerm { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
    }
}
