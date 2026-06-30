using System;
using System.Collections.Generic;
using ExerciseApp.Model;
using ExerciseApp.Service;
using Xunit;

namespace ExerciseApp.Tests
{
    public class QuoteStorageServiceTests
    {
        private readonly QuoteStorageService _storageService = new QuoteStorageService();

        public QuoteStorageServiceTests()
        {
            // Clear quotes before each test
            _storageService.ClearAllQuotes();
        }

        #region Store Quote Tests

        [Fact]
        public void StoreQuote_WithValidRequest_ReturnsReferenceNumber()
        {
            // Arrange
            var request = new QuoteRequest
            {
                DateOfBirth = DateTime.Today.AddYears(-35),
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Ford",
                Model = "Focus"
            };
            var quoteAmount = 200m;

            // Act
            var referenceNumber = _storageService.StoreQuote(request, quoteAmount);

            // Assert
            Assert.NotNull(referenceNumber);
            Assert.NotEmpty(referenceNumber);
            Assert.StartsWith("QT-", referenceNumber);
        }

        [Fact]
        public void StoreQuote_WithNullRequest_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _storageService.StoreQuote(null, 200));
        }

        [Fact]
        public void StoreQuote_WithoutDateOfBirth_ThrowsException()
        {
            // Arrange
            var request = new QuoteRequest
            {
                DateOfBirth = null,
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Ford",
                Model = "Focus"
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _storageService.StoreQuote(request, 200));
        }

        #endregion

        #region Retrieve Quote Tests

        [Fact]
        public void GetQuoteByReference_WithValidReference_ReturnsStoredQuote()
        {
            // Arrange
            var request = new QuoteRequest
            {
                DateOfBirth = DateTime.Today.AddYears(-35),
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Ford",
                Model = "Focus"
            };
            var referenceNumber = _storageService.StoreQuote(request, 200);

            // Act
            var retrievedQuote = _storageService.GetQuoteByReference(referenceNumber);

            // Assert
            Assert.NotNull(retrievedQuote);
            Assert.Equal(referenceNumber, retrievedQuote.ReferenceNumber);
            Assert.Equal(200, retrievedQuote.QuoteAmount);
            Assert.Equal("Ford", retrievedQuote.Make);
        }

        [Fact]
        public void GetQuoteByReference_WithInvalidReference_ReturnsNull()
        {
            // Act
            var quote = _storageService.GetQuoteByReference("QT-INVALID-000000");

            // Assert
            Assert.Null(quote);
        }

        [Fact]
        public void GetQuoteByReference_WithNullReference_ReturnsNull()
        {
            // Act
            var quote = _storageService.GetQuoteByReference(null);

            // Assert
            Assert.Null(quote);
        }

        #endregion

        #region Get All Quotes Tests

        [Fact]
        public void GetAllQuotes_WithMultipleQuotes_ReturnsAllQuotes()
        {
            // Arrange
            for (int i = 0; i < 5; i++)
            {
                var request = new QuoteRequest
                {
                    DateOfBirth = DateTime.Today.AddYears(-35),
                    InsuranceType = InsuranceType.FullyComprehensive,
                    Make = "Ford",
                    Model = "Focus"
                };
                _storageService.StoreQuote(request, 200 + i);
            }

            // Act
            var quotes = _storageService.GetAllQuotes();

            // Assert
            Assert.Equal(5, quotes.Count);
        }

        [Fact]
        public void GetValidQuotes_WithExpiredQuote_ExcludesExpiredQuote()
        {
            // Arrange
            var request = new QuoteRequest
            {
                DateOfBirth = DateTime.Today.AddYears(-35),
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Ford",
                Model = "Focus"
            };
            var referenceNumber = _storageService.StoreQuote(request, 200);
            var quote = _storageService.GetQuoteByReference(referenceNumber);

            // Manually expire the quote
            quote.ExpiryDate = DateTime.Now.AddDays(-1);

            // Act
            var validQuotes = _storageService.GetValidQuotes();

            // Assert
            Assert.Empty(validQuotes);
        }

        #endregion

        #region Quote Validity Tests

        [Fact]
        public void StoredQuote_IsValid_WhenNotExpiredAndNotAccepted()
        {
            // Arrange
            var request = new QuoteRequest
            {
                DateOfBirth = DateTime.Today.AddYears(-35),
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Ford",
                Model = "Focus"
            };
            var referenceNumber = _storageService.StoreQuote(request, 200);
            var quote = _storageService.GetQuoteByReference(referenceNumber);

            // Assert
            Assert.True(quote.IsValid);
            Assert.False(quote.IsAccepted);
        }

        [Fact]
        public void StoredQuote_ExpiryDate_Is30DaysFromCreation()
        {
            // Arrange
            var request = new QuoteRequest
            {
                DateOfBirth = DateTime.Today.AddYears(-35),
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Ford",
                Model = "Focus"
            };
            var beforeCreation = DateTime.Now;
            var referenceNumber = _storageService.StoreQuote(request, 200);
            var afterCreation = DateTime.Now;
            var quote = _storageService.GetQuoteByReference(referenceNumber);

            // Assert
            var expectedExpiry = beforeCreation.AddDays(30);
            var actualExpiry = quote.ExpiryDate;

            Assert.True((actualExpiry - expectedExpiry).TotalMinutes < 1);
        }

        #endregion

        #region Accept Quote Tests

        [Fact]
        public void AcceptQuote_WithValidReference_AcceptsQuote()
        {
            // Arrange
            var request = new QuoteRequest
            {
                DateOfBirth = DateTime.Today.AddYears(-35),
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Ford",
                Model = "Focus"
            };
            var referenceNumber = _storageService.StoreQuote(request, 200);

            // Act
            var success = _storageService.AcceptQuote(referenceNumber);
            var acceptedQuote = _storageService.GetQuoteByReference(referenceNumber);

            // Assert
            Assert.True(success);
            Assert.True(acceptedQuote.IsAccepted);
            Assert.NotNull(acceptedQuote.AcceptedDate);
        }

        [Fact]
        public void AcceptQuote_WithInvalidReference_ReturnsFalse()
        {
            // Act
            var success = _storageService.AcceptQuote("QT-INVALID-000000");

            // Assert
            Assert.False(success);
        }

        [Fact]
        public void AcceptQuote_WhenAlreadyAccepted_ReturnsFalse()
        {
            // Arrange
            var request = new QuoteRequest
            {
                DateOfBirth = DateTime.Today.AddYears(-35),
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Ford",
                Model = "Focus"
            };
            var referenceNumber = _storageService.StoreQuote(request, 200);
            _storageService.AcceptQuote(referenceNumber);

            // Act
            var secondAcceptance = _storageService.AcceptQuote(referenceNumber);

            // Assert
            Assert.False(secondAcceptance);
        }

        #endregion

        #region Cancel Quote Tests

        [Fact]
        public void CancelQuote_WithValidReference_CancelsQuote()
        {
            // Arrange
            var request = new QuoteRequest
            {
                DateOfBirth = DateTime.Today.AddYears(-35),
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Ford",
                Model = "Focus"
            };
            var referenceNumber = _storageService.StoreQuote(request, 200);

            // Act
            var success = _storageService.CancelQuote(referenceNumber);
            var cancelledQuote = _storageService.GetQuoteByReference(referenceNumber);

            // Assert
            Assert.True(success);
            Assert.Null(cancelledQuote);
        }

        [Fact]
        public void CancelQuote_WithInvalidReference_ReturnsFalse()
        {
            // Act
            var success = _storageService.CancelQuote("QT-INVALID-000000");

            // Assert
            Assert.False(success);
        }

        #endregion

        #region Update Notes Tests

        [Fact]
        public void UpdateQuoteNotes_WithValidReference_UpdatesNotes()
        {
            // Arrange
            var request = new QuoteRequest
            {
                DateOfBirth = DateTime.Today.AddYears(-35),
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Ford",
                Model = "Focus"
            };
            var referenceNumber = _storageService.StoreQuote(request, 200);
            var newNotes = "Additional coverage requested";

            // Act
            var success = _storageService.UpdateQuoteNotes(referenceNumber, newNotes);
            var updatedQuote = _storageService.GetQuoteByReference(referenceNumber);

            // Assert
            Assert.True(success);
            Assert.Equal(newNotes, updatedQuote.Notes);
        }

        [Fact]
        public void UpdateQuoteNotes_WithInvalidReference_ReturnsFalse()
        {
            // Act
            var success = _storageService.UpdateQuoteNotes("QT-INVALID-000000", "Some notes");

            // Assert
            Assert.False(success);
        }

        #endregion

        #region Query Tests

        [Fact]
        public void GetQuotesByDateOfBirth_ReturnsMatchingQuotes()
        {
            // Arrange
            var dob = DateTime.Today.AddYears(-35);
            for (int i = 0; i < 3; i++)
            {
                var request = new QuoteRequest
                {
                    DateOfBirth = dob,
                    InsuranceType = InsuranceType.FullyComprehensive,
                    Make = "Ford",
                    Model = "Focus"
                };
                _storageService.StoreQuote(request, 200);
            }

            // Act
            var quotes = _storageService.GetQuotesByDateOfBirth(dob);

            // Assert
            Assert.Equal(3, quotes.Count);
        }

        [Fact]
        public void GetQuotesByVehicle_ReturnsMatchingQuotes()
        {
            // Arrange
            for (int i = 0; i < 2; i++)
            {
                var request = new QuoteRequest
                {
                    DateOfBirth = DateTime.Today.AddYears(-35),
                    InsuranceType = InsuranceType.FullyComprehensive,
                    Make = "Ford",
                    Model = "Focus"
                };
                _storageService.StoreQuote(request, 200);
            }

            // Act
            var quotes = _storageService.GetQuotesByVehicle("Ford", "Focus");

            // Assert
            Assert.Equal(2, quotes.Count);
        }

        [Fact]
        public void GetQuotesByVehicle_WithNonexistentVehicle_ReturnsEmpty()
        {
            // Act
            var quotes = _storageService.GetQuotesByVehicle("NonExistent", "Model");

            // Assert
            Assert.Empty(quotes);
        }

        #endregion

        #region Statistics Tests

        [Fact]
        public void GetStatistics_ReturnsCorrectCounts()
        {
            // Arrange
            for (int i = 0; i < 3; i++)
            {
                var request = new QuoteRequest
                {
                    DateOfBirth = DateTime.Today.AddYears(-35),
                    InsuranceType = InsuranceType.FullyComprehensive,
                    Make = "Ford",
                    Model = "Focus"
                };
                _storageService.StoreQuote(request, 200);
            }

            // Act
            var stats = _storageService.GetStatistics();

            // Assert
            Assert.Equal(3, stats.TotalQuotes);
            Assert.Equal(3, stats.ValidQuotes);
            Assert.Equal(0, stats.AcceptedQuotes);
        }

        [Fact]
        public void GetStatistics_CalculatesAverageQuoteAmount()
        {
            // Arrange
            var amounts = new[] { 100m, 200m, 300m };
            int i = 0;
            foreach (var amount in amounts)
            {
                var request = new QuoteRequest
                {
                    DateOfBirth = DateTime.Today.AddYears(-35),
                    InsuranceType = InsuranceType.FullyComprehensive,
                    Make = "Ford",
                    Model = "Focus"
                };
                _storageService.StoreQuote(request, amount);
                i++;
            }

            // Act
            var stats = _storageService.GetStatistics();

            // Assert
            Assert.Equal(200, stats.AverageQuoteAmount); // Average of 100, 200, 300
        }

        #endregion

        #region Age Calculation Tests

        [Fact]
        public void StoredQuote_AgeAtQuote_CalculatesCorrectly()
        {
            // Arrange
            var dob = DateTime.Today.AddYears(-25);
            var request = new QuoteRequest
            {
                DateOfBirth = dob,
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Ford",
                Model = "Focus"
            };
            var referenceNumber = _storageService.StoreQuote(request, 200);
            var quote = _storageService.GetQuoteByReference(referenceNumber);

            // Assert
            Assert.Equal(25, quote.AgeAtQuote);
        }

        #endregion
    }
}
