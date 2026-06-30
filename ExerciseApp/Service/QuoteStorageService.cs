using ExerciseApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExerciseApp.Service
{
    /// <summary>
    /// Service for storing and retrieving quotes
    /// Uses in-memory storage (can be replaced with database later)
    /// </summary>
    public class QuoteStorageService
    {
        // In-memory storage of quotes
        private static List<StoredQuote> _storedQuotes = new List<StoredQuote>();
        private static int _quoteIdCounter = 1000;

        /// <summary>
        /// Stores a new quote and returns the reference number
        /// </summary>
        public string StoreQuote(QuoteRequest request, decimal quoteAmount)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (!request.DateOfBirth.HasValue)
                throw new ArgumentException("Date of birth is required", nameof(request.DateOfBirth));

            var referenceNumber = GenerateReferenceNumber();
            var now = DateTime.Now;

            var storedQuote = new StoredQuote
            {
                Id = _quoteIdCounter++,
                ReferenceNumber = referenceNumber,
                DateOfBirth = request.DateOfBirth.Value,
                Make = request.Make,
                Model = request.Model,
                InsuranceType = request.InsuranceType.Value,
                QuoteAmount = quoteAmount,
                QuoteDate = now,
                ExpiryDate = now.AddDays(30), // Quote valid for 30 days
                IsAccepted = false,
                Notes = string.Empty
            };

            _storedQuotes.Add(storedQuote);
            return referenceNumber;
        }

        /// <summary>
        /// Retrieves a quote by reference number
        /// </summary>
        public StoredQuote GetQuoteByReference(string referenceNumber)
        {
            if (string.IsNullOrWhiteSpace(referenceNumber))
                return null;

            return _storedQuotes.FirstOrDefault(q => q.ReferenceNumber == referenceNumber);
        }

        /// <summary>
        /// Retrieves all valid (non-expired, non-accepted) quotes
        /// </summary>
        public List<StoredQuote> GetValidQuotes()
        {
            return _storedQuotes.Where(q => q.IsValid).ToList();
        }

        /// <summary>
        /// Retrieves all stored quotes
        /// </summary>
        public List<StoredQuote> GetAllQuotes()
        {
            return _storedQuotes.ToList();
        }

        /// <summary>
        /// Retrieves all quotes for a specific date of birth
        /// </summary>
        public List<StoredQuote> GetQuotesByDateOfBirth(DateTime dateOfBirth)
        {
            return _storedQuotes.Where(q => q.DateOfBirth.Date == dateOfBirth.Date).ToList();
        }

        /// <summary>
        /// Retrieves all quotes for a specific vehicle make and model
        /// </summary>
        public List<StoredQuote> GetQuotesByVehicle(string make, string model)
        {
            if (string.IsNullOrWhiteSpace(make) || string.IsNullOrWhiteSpace(model))
                return new List<StoredQuote>();

            return _storedQuotes.Where(q => q.Make == make && q.Model == model).ToList();
        }

        /// <summary>
        /// Accepts/purchases a quote
        /// </summary>
        public bool AcceptQuote(string referenceNumber)
        {
            var quote = GetQuoteByReference(referenceNumber);
            if (quote == null)
                return false;

            if (!quote.IsValid)
                return false; // Cannot accept expired or already accepted quotes

            quote.IsAccepted = true;
            quote.AcceptedDate = DateTime.Now;
            return true;
        }

        /// <summary>
        /// Rejects/cancels a quote
        /// </summary>
        public bool CancelQuote(string referenceNumber)
        {
            var quote = GetQuoteByReference(referenceNumber);
            if (quote == null)
                return false;

            _storedQuotes.Remove(quote);
            return true;
        }

        /// <summary>
        /// Updates notes for a quote
        /// </summary>
        public bool UpdateQuoteNotes(string referenceNumber, string notes)
        {
            var quote = GetQuoteByReference(referenceNumber);
            if (quote == null)
                return false;

            quote.Notes = notes ?? string.Empty;
            return true;
        }

        /// <summary>
        /// Gets quotes within a date range
        /// </summary>
        public List<StoredQuote> GetQuotesByDateRange(DateTime startDate, DateTime endDate)
        {
            return _storedQuotes.Where(q => 
                q.QuoteDate.Date >= startDate.Date && q.QuoteDate.Date <= endDate.Date
            ).ToList();
        }

        /// <summary>
        /// Gets statistics about stored quotes
        /// </summary>
        public QuoteStatistics GetStatistics()
        {
            var stats = new QuoteStatistics
            {
                TotalQuotes = _storedQuotes.Count,
                ValidQuotes = _storedQuotes.Count(q => q.IsValid),
                AcceptedQuotes = _storedQuotes.Count(q => q.IsAccepted),
                ExpiredQuotes = _storedQuotes.Count(q => q.QuoteDate.AddDays(30) < DateTime.Now && !q.IsAccepted),
                AverageQuoteAmount = _storedQuotes.Count > 0 ? (double)_storedQuotes.Average(q => q.QuoteAmount) : 0
            };

            return stats;
        }

        /// <summary>
        /// Clears all stored quotes (for testing purposes)
        /// </summary>
        public void ClearAllQuotes()
        {
            _storedQuotes.Clear();
            _quoteIdCounter = 1000;
        }

        /// <summary>
        /// Generates a unique reference number for quotes
        /// Format: QT-YYYYMMDD-XXXXXX (e.g., QT-20231215-001234)
        /// </summary>
        private string GenerateReferenceNumber()
        {
            var now = DateTime.Now;
            var dateString = now.ToString("yyyyMMdd");
            var sequenceNumber = _quoteIdCounter - 1000;
            return $"QT-{dateString}-{sequenceNumber:D6}";
        }
    }

    /// <summary>
    /// Statistics about stored quotes
    /// </summary>
    public class QuoteStatistics
    {
        public int TotalQuotes { get; set; }
        public int ValidQuotes { get; set; }
        public int AcceptedQuotes { get; set; }
        public int ExpiredQuotes { get; set; }
        public double AverageQuoteAmount { get; set; }
    }
}
