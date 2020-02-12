using DNI.API.Requests;
using DNI.Services.Shared.Sorting;
using DNI.Testing;

using Xunit;

namespace DNI.API.Tests.Requests {
    [Trait(TraitConstants.TraitTestType, TraitConstants.TraitTestTypeUnit)]
    public class GetShowsRequestUnitTests {
        [Fact]
        public void GetShowsRequest_HasExpectedDefaults() {
            // Arrange & Act
            var request = new GetShowsRequest();

            // Assert
            Assert.Equal(1, request.PageNumber);
            Assert.Equal(7, request.ItemsPerPage);
            Assert.Equal(FieldOrder.Descending, request.Order);
            Assert.Equal("PublishedTime", request.Field);
        }
    }
}