using System;
using ExerciseApp.Controllers;
using ExerciseApp.Model;
using ExerciseApp.Service;
using Xunit;

namespace ExerciseApp.Tests
{
    public class QuoteControllerTests
    {
        private readonly QuoteController _controller = new QuoteController();

        #region GET Endpoint Tests

            [Fact]
            public void GetQuoteDetail_ReturnsQuoteDetail()
            {
                // Act
                var result = _controller.Get();

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result.Makes);
                Assert.NotEmpty(result.Models);
                Assert.NotEmpty(result.InsuranceTypes);
            }

            [Fact]
            public void GetQuoteDetail_ContainsFordMakes()
            {
                // Act
                var result = _controller.Get();

                // Assert
                Assert.Contains("Ford", result.Makes);
            }

            [Fact]
            public void GetQuoteDetail_ContainsAudiMakes()
            {
                // Act
                var result = _controller.Get();

                // Assert
                Assert.Contains("Audi", result.Makes);
            }

            [Fact]
            public void GetQuoteDetail_ContainsBMWMakes()
            {
                // Act
                var result = _controller.Get();

                // Assert
                Assert.Contains("BMW", result.Makes);
            }

            [Fact]
            public void GetQuoteDetail_ReturnsMultipleModelsForFord()
            {
                // Act
                var result = _controller.Get();
                var fordModels = result.Models.Find(m => m.Make == "Ford");

                // Assert
                Assert.NotNull(fordModels);
                Assert.NotEmpty(fordModels.Models);
                Assert.Contains("Focus", fordModels.Models);
            }

            #endregion
    }
}
