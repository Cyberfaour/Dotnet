using AMC_Generator.Models;
using FluentAssertions;
using NPOI.SS.Formula.Functions;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace AMC_Generator.Tests.Models
{
    public class ModelTests
    {
        [Fact]
        public void Owner_Should_Have_Valid_DataAnnotations()
        {
            // Arrange
            var owner = new Owner
            {
                Id = 1,
                FullName = "John Doe",
                Phone = "1234567890",
                Buildings = new List<Building>()
            };

            // Act
            var validationResults = ValidateModel(owner);

            // Assert
            validationResults.Should().BeEmpty(); // No validation errors expected
        }

        [Fact]
        public void Building_Should_Have_Valid_DataAnnotations()
        {
            // Arrange
            var building = new Building
            {
                Id = 1,
                Name = "Building A",
                Address = "123 Main St",
                OwnerId = 1,
                Owner = new Owner(),
                AMCs = new List<AMC>()
            };

            // Act
            var validationResults = ValidateModel(building);

            // Assert
            validationResults.Should().BeEmpty(); // No validation errors expected
        }

        [Fact]
        public void AMC_Should_Have_Valid_DataAnnotations()
        {
            // Arrange
            var amc = new AMC
            {
                Id = 1,
                ProjectNumber = "P12345",
                AMCValue = 10000.50m,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddYears(1),
                BuildingId = 1,
                Building = new Building(),
                PdfPath = "path/to/pdf"
            };

            // Act
            var validationResults = ValidateModel(amc);

            // Assert
            validationResults.Should().BeEmpty(); // No validation errors expected
        }

        private static IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }
    }
}
