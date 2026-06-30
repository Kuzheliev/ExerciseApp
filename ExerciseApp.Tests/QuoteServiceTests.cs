using System;
using ExerciseApp.Model;
using ExerciseApp.Service;
using Xunit;

namespace ExerciseApp.Tests
{
    public class QuoteServiceTests
    {
        private readonly QuoteService _quoteService = new QuoteService();

        [Fact]
        public void WhenDetailsAreProvided_AQuoteIsProduced()
        {
            var quoteResult = _quoteService.PerformQuote(new QuoteRequest 
                { DateOfBirth = new DateTime(2000, 05, 01),
                    InsuranceType = InsuranceType.FullyComprehensive, 
                    Make = "Ford", 
                    Model = "Focus"});
            Assert.Equal(200, quoteResult);
        }

        #region Age Eligibility Tests

        [Fact]
        public void IsEligibleForQuote_WhenAgeIs17_ReturnsTrue()
        {
            // Arrange
            var dateOfBirth = DateTime.Today.AddYears(-17);
            var request = new QuoteRequest
            {
                DateOfBirth = dateOfBirth,
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Ford",
                Model = "Focus"
            };

            // Act
            var result = _quoteService.IsEligibleForQuote(request);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsEligibleForQuote_WhenAgeIs80_ReturnsTrue()
        {
            // Arrange
            var dateOfBirth = DateTime.Today.AddYears(-80);
            var request = new QuoteRequest
            {
                DateOfBirth = dateOfBirth,
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Ford",
                Model = "Focus"
            };

            // Act
            var result = _quoteService.IsEligibleForQuote(request);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsEligibleForQuote_WhenAgeIs16_ReturnsFalse()
        {
            // Arrange
            var dateOfBirth = DateTime.Today.AddYears(-16);
            var request = new QuoteRequest
            {
                DateOfBirth = dateOfBirth,
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Ford",
                Model = "Focus"
            };

            // Act
            var result = _quoteService.IsEligibleForQuote(request);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsEligibleForQuote_WhenAgeIs81_ReturnsFalse()
        {
            // Arrange
            var dateOfBirth = DateTime.Today.AddYears(-81);
            var request = new QuoteRequest
            {
                DateOfBirth = dateOfBirth,
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Ford",
                Model = "Focus"
            };

            // Act
            var result = _quoteService.IsEligibleForQuote(request);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsEligibleForQuote_WhenDateOfBirthIsNull_ReturnsFalse()
        {
            // Arrange
            var request = new QuoteRequest
            {
                DateOfBirth = null,
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Ford",
                Model = "Focus"
            };

            // Act
            var result = _quoteService.IsEligibleForQuote(request);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsEligibleForQuote_WhenAgeIs30_ReturnsTrue()
        {
            // Arrange
            var dateOfBirth = DateTime.Today.AddYears(-30);
            var request = new QuoteRequest
            {
                DateOfBirth = dateOfBirth,
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Ford",
                Model = "Focus"
            };

            // Act
            var result = _quoteService.IsEligibleForQuote(request);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Age Error Message Tests

        [Fact]
        public void GetAgeEligibilityError_WhenAgeIsUnder17_ReturnsUnderAgeMessage()
        {
            // Arrange
            var dateOfBirth = DateTime.Today.AddYears(-16);
            var request = new QuoteRequest
            {
                DateOfBirth = dateOfBirth
            };

            // Act
            var result = _quoteService.GetAgeEligibilityError(request);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("at least 17", result);
        }

        [Fact]
        public void GetAgeEligibilityError_WhenAgeIsOver80_ReturnsOverAgeMessage()
        {
            // Arrange
            var dateOfBirth = DateTime.Today.AddYears(-85);
            var request = new QuoteRequest
            {
                DateOfBirth = dateOfBirth
            };

            // Act
            var result = _quoteService.GetAgeEligibilityError(request);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("80 years old or younger", result);
        }

        [Fact]
        public void GetAgeEligibilityError_WhenAgeIsValid_ReturnsNull()
        {
            // Arrange
            var dateOfBirth = DateTime.Today.AddYears(-30);
            var request = new QuoteRequest
            {
                DateOfBirth = dateOfBirth
            };

            // Act
            var result = _quoteService.GetAgeEligibilityError(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetAgeEligibilityError_WhenDateOfBirthIsNull_ReturnsErrorMessage()
        {
            // Arrange
            var request = new QuoteRequest
            {
                DateOfBirth = null
            };

            // Act
            var result = _quoteService.GetAgeEligibilityError(request);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("required", result);
        }

        [Fact]
        public void GetAgeEligibilityError_WhenAgeIs17_ReturnsNull()
        {
            // Arrange
            var dateOfBirth = DateTime.Today.AddYears(-17);
            var request = new QuoteRequest
            {
                DateOfBirth = dateOfBirth
            };

            // Act
            var result = _quoteService.GetAgeEligibilityError(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetAgeEligibilityError_WhenAgeIs80_ReturnsNull()
        {
            // Arrange
            var dateOfBirth = DateTime.Today.AddYears(-80);
            var request = new QuoteRequest
            {
                DateOfBirth = dateOfBirth
            };

            // Act
            var result = _quoteService.GetAgeEligibilityError(request);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Quote Calculation Tests

        [Fact]
        public void PerformQuote_FordFullyComprehensive_Returns200()
        {
            // Arrange
            var request = new QuoteRequest
            {
                DateOfBirth = DateTime.Today.AddYears(-30),
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Ford",
                Model = "Focus"
            };

            // Act
            var result = _quoteService.PerformQuote(request);

            // Assert
            Assert.Equal(200, result);
        }

        [Fact]
        public void PerformQuote_BMWFullyComprehensive_Returns500()
        {
            // Arrange
            var request = new QuoteRequest
            {
                DateOfBirth = DateTime.Today.AddYears(-30),
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "BMW",
                Model = "X5"
            };

            // Act
            var result = _quoteService.PerformQuote(request);

            // Assert
            Assert.Equal(500, result);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void IsEligibleForQuote_WhenAgeTurning17Today_ReturnsTrue()
        {
            // Arrange - Someone who turned 17 today
            var today = DateTime.Today;
            var dateOfBirth = new DateTime(today.Year - 17, today.Month, today.Day);
            var request = new QuoteRequest
            {
                DateOfBirth = dateOfBirth,
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Ford",
                Model = "Focus"
            };

            // Act
            var result = _quoteService.IsEligibleForQuote(request);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(15)]
        [InlineData(14)]
        [InlineData(10)]
        [InlineData(5)]
        public void IsEligibleForQuote_WithVariousYoungAges_ReturnsFalse(int age)
        {
            // Arrange
            var dateOfBirth = DateTime.Today.AddYears(-age);
            var request = new QuoteRequest
            {
                DateOfBirth = dateOfBirth,
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Ford",
                Model = "Focus"
            };

            // Act
            var result = _quoteService.IsEligibleForQuote(request);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData(82)]
        [InlineData(85)]
        [InlineData(90)]
        [InlineData(100)]
        public void IsEligibleForQuote_WithVariousOldAges_ReturnsFalse(int age)
        {
            // Arrange
            var dateOfBirth = DateTime.Today.AddYears(-age);
            var request = new QuoteRequest
            {
                DateOfBirth = dateOfBirth,
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Ford",
                Model = "Focus"
            };

            // Act
            var result = _quoteService.IsEligibleForQuote(request);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData(17)]
        [InlineData(25)]
        [InlineData(45)]
        [InlineData(65)]
        [InlineData(80)]
        public void IsEligibleForQuote_WithValidAges_ReturnsTrue(int age)
        {
            // Arrange
            var dateOfBirth = DateTime.Today.AddYears(-age);
            var request = new QuoteRequest
            {
                DateOfBirth = dateOfBirth,
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Ford",
                Model = "Focus"
            };

            // Act
            var result = _quoteService.IsEligibleForQuote(request);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void PerformQuote_AudiFullyComprehensive_Returns300()
        {
            // Arrange
            var request = new QuoteRequest
            {
                DateOfBirth = DateTime.Today.AddYears(-35),
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "Audi",
                Model = "A3"
            };

            // Act
            var result = _quoteService.PerformQuote(request);

            // Assert
            Assert.Equal(300, result);
        }

        [Fact]
        public void PerformQuote_AudiThirdPartyOnly_Returns250()
        {
            // Arrange
            var request = new QuoteRequest
            {
                DateOfBirth = DateTime.Today.AddYears(-35),
                InsuranceType = InsuranceType.ThirdPartyOnly,
                Make = "Audi",
                Model = "A5"
            };

            // Act
            var result = _quoteService.PerformQuote(request);

            // Assert
            Assert.Equal(250, result);
        }

        [Fact]
        public void PerformQuote_FordThirdPartyFireAndTheft_Returns180()
        {
            // Arrange
            var request = new QuoteRequest
            {
                DateOfBirth = DateTime.Today.AddYears(-40),
                InsuranceType = InsuranceType.ThirdPartyFireAndTheft,
                Make = "Ford",
                Model = "Fiesta"
            };

            // Act
            var result = _quoteService.PerformQuote(request);

            // Assert
            Assert.Equal(180, result);
        }

        [Fact]
        public void PerformQuote_WithUnknownMake_ReturnsDefaultPrice()
        {
            // Arrange
            var request = new QuoteRequest
            {
                DateOfBirth = DateTime.Today.AddYears(-35),
                InsuranceType = InsuranceType.FullyComprehensive,
                Make = "UnknownMake",
                Model = "UnknownModel"
            };

            // Act
            var result = _quoteService.PerformQuote(request);

            // Assert
            Assert.Equal(300, result); // Should return default price
        }

        [Fact]
        public void GetAgeEligibilityError_WithAllowedAgeRange_ReturnsNullForAllValidAges()
        {
            // Arrange & Act & Assert
            for (int age = 17; age <= 80; age++)
            {
                var dateOfBirth = DateTime.Today.AddYears(-age);
                var request = new QuoteRequest { DateOfBirth = dateOfBirth };
                var result = _quoteService.GetAgeEligibilityError(request);
                Assert.Null(result);
            }
        }

        #endregion
    }
}
