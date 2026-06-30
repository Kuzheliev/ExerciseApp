using System;
using System.ComponentModel.DataAnnotations;

namespace ExerciseApp.Model
{
    /// <summary>
    /// Represents a stored quote for later retrieval
    /// </summary>
    public class StoredQuote
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ReferenceNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(10)]
        public string Make { get; set; }

        [Required]
        [StringLength(10)]
        public string Model { get; set; }

        [Required]
        public InsuranceType InsuranceType { get; set; }

        [Required]
        public decimal QuoteAmount { get; set; }

        [Required]
        public DateTime QuoteDate { get; set; }

        /// <summary>
        /// Quote expiry date (30 days from creation)
        /// </summary>
        [Required]
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// Indicates if the quote has been accepted/purchased
        /// </summary>
        public bool IsAccepted { get; set; }

        /// <summary>
        /// Date when the quote was accepted
        /// </summary>
        public DateTime? AcceptedDate { get; set; }

        /// <summary>
        /// Notes or comments about the quote
        /// </summary>
        [StringLength(500)]
        public string Notes { get; set; }

        /// <summary>
        /// Indicates if the quote is valid (not expired and not accepted)
        /// </summary>
        public bool IsValid => !IsAccepted && DateTime.Now <= ExpiryDate;

        /// <summary>
        /// Calculated age at time of quote
        /// </summary>
        public int AgeAtQuote
        {
            get
            {
                var quoteDate = QuoteDate.Date;
                var age = quoteDate.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > quoteDate.AddYears(-age))
                {
                    age--;
                }
                return age;
            }
        }
    }
}
