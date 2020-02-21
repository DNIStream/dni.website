using AutoFixture;
using AutoFixture.AutoMoq;

using DNI.API.Validators;
using DNI.Testing;

using FluentValidation.TestHelper;

using Xunit;
using Xunit.Abstractions;

namespace DNI.API.Tests.Validators {
    [Trait(TraitConstants.TraitTestType, TraitConstants.TraitTestTypeUnit)]
    public class GetShowsRequestValidatorUnitTests {
        private readonly ITestOutputHelper _output;
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});

        public GetShowsRequestValidatorUnitTests(ITestOutputHelper output) {
            _output = output;
        }

        private GetShowsRequestValidator GetValidator() {
            return new GetShowsRequestValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\n")]
        [InlineData("\r")]
        [InlineData("\t")]
        public void Field_Invalid_WhenFieldIsNullOrWhitespace(string field) {
            // Arrange
            var validator = GetValidator();

            // Act & Assert
            validator.ShouldHaveValidationErrorFor(o => o.Field, field)
                .WithErrorMessage("Field is required");
        }

        [Fact]
        public void Field_Invalid_WhenInvalidFieldValuePassed() {
            // Arrange
            const string expectedErrorMessage =
                "The specified 'sort-field' is not valid. Possible values: 'PublishedTime', 'Title', 'DurationInSeconds'.";
            var validator = GetValidator();

            // Act & Assert
            validator.ShouldHaveValidationErrorFor(o => o.Field, "NOTAVALIDFIELD")
                .WithErrorMessage(expectedErrorMessage);
        }

        [Theory]
        [InlineData("PublishedTime")]
        [InlineData("Title")]
        [InlineData("DurationInSeconds")]
        public void Field_Validates_WhenExpectedFieldsAreSpecified(string field) {
            // Arrange
            var validator = GetValidator();

            // Act & Assert
            validator.ShouldNotHaveValidationErrorFor(o => o.Field, field);
        }
    }
}