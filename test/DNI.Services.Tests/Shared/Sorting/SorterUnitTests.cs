using System;
using System.Linq;
using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.AutoMoq;

using DNI.Services.Shared.Sorting;
using DNI.Testing;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Services.Tests.Shared.Sorting {
    [Trait(TraitConstants.TraitTestType, TraitConstants.TraitTestTypeUnit)]
    public class SorterUnitTests {
        private readonly ITestOutputHelper _output;
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});

        public SorterUnitTests(ITestOutputHelper output) {
            _output = output;
        }

        private ISorter<string> GetCalculator() {
            return new Sorter<string>();
        }

        [Fact]
        public async Task SortAsync_ThrowsException_WhenNullSortingInfo() {
            // Arrange
            var results = _fixture.CreateMany<string>(56);
            var sorter = GetCalculator();

            // Act
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => sorter.SortAsync(results, null));

            // Assert
            Assert.Equal("sortingRequest", ex.ParamName);
        }

        [Fact]
        public async Task SortAsync_ThrowsException_WhenNullItems() {
            // Arrange
            var sortingInfo = _fixture.Create<TestSortingRequest>();
            var sorter = GetCalculator();

            // Act
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => sorter.SortAsync(null, sortingInfo));

            // Assert
            Assert.Equal("allItems", ex.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\n")]
        [InlineData("\r")]
        [InlineData("\t")]
        public async Task SortAsync_ThrowsException_WhenSortingInfoHasNullFieldName(string fieldName) {
            // Arrange
            var sortingInfo = _fixture.Create<TestSortingRequest>();
            sortingInfo.Field = fieldName;

            var results = _fixture.CreateMany<string>();
            var sorter = GetCalculator();

            // Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => sorter.SortAsync(results, sortingInfo));

            // Assert
            Assert.Equal("sortingRequest", ex.ParamName);
            Assert.Contains("sortingRequest.Field is required", ex.Message);
        }

        [Fact]
        public async Task SortAsync_ThrowsInvalidOperation_WhenSpecifiedFieldDoesNotExistOnType() {
            // Arrange
            var results = _fixture.CreateMany<string>(23);
            var sortingInfo = _fixture.Create<TestSortingRequest>();
            sortingInfo.Field = "NOTASTRINGMEMBER";
            var sorter = GetCalculator();

            // Act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sorter.SortAsync(results, sortingInfo));

            // Assert
            Assert.Contains("'NOTASTRINGMEMBER' is not a property of type 'String'", ex.Message);
        }

        [Fact]
        public async Task SortAsync_ReturnsEmptyArray_IfAllItemsIsEmpty() {
            // Arrange
            var results = _fixture.CreateMany<string>(0);
            var sortingInfo = _fixture.Create<TestSortingRequest>();
            sortingInfo.Field = "Length";
            var sorter = GetCalculator();

            // Act
            var actualResult = await sorter.SortAsync(results, sortingInfo);

            // Assert
            Assert.NotNull(actualResult);
            Assert.Empty(actualResult);
        }

        [Fact]
        public async Task SortAsync_ReturnsNewArrayOrderedBySpecifiedProperty_WhenSortingInfo_OrdersAscending() {
            // Arrange
            var first = string.Join("", Enumerable.Repeat("A", 3));
            var second = string.Join("", Enumerable.Repeat("B", 6));
            var third = string.Join("", Enumerable.Repeat("C", 10));
            var fourth = string.Join("", Enumerable.Repeat("D", 17));
            var fifth = string.Join("", Enumerable.Repeat("E", 25));

            var inputArray = new[] {second, fifth, third, first, fourth};
            var sortingInfo = _fixture.Create<TestSortingRequest>();
            sortingInfo.Field = "Length";
            sortingInfo.Order = FieldOrder.Ascending;

            var sorter = GetCalculator();

            // Act
            var result = await sorter.SortAsync(inputArray, sortingInfo);

            // Assert
            Assert.Equal(first, result[0]);
            Assert.Equal(second, result[1]);
            Assert.Equal(third, result[2]);
            Assert.Equal(fourth, result[3]);
            Assert.Equal(fifth, result[4]);
        }

        [Fact]
        public async Task SortAsync_ReturnsNewArrayOrderedBySpecifiedProperty_WhenSortingInfo_OrdersDescending() {
            // Arrange
            var first = string.Join("", Enumerable.Repeat("A", 3));
            var second = string.Join("", Enumerable.Repeat("B", 6));
            var third = string.Join("", Enumerable.Repeat("C", 10));
            var fourth = string.Join("", Enumerable.Repeat("D", 17));
            var fifth = string.Join("", Enumerable.Repeat("E", 25));

            var inputArray = new[] {second, fifth, third, first, fourth};
            var sortingInfo = _fixture.Create<TestSortingRequest>();
            sortingInfo.Field = "Length";
            sortingInfo.Order = FieldOrder.Descending;

            var sorter = GetCalculator();

            // Act
            var result = await sorter.SortAsync(inputArray, sortingInfo);

            // Assert
            Assert.Equal(first, result[4]);
            Assert.Equal(second, result[3]);
            Assert.Equal(third, result[2]);
            Assert.Equal(fourth, result[1]);
            Assert.Equal(fifth, result[0]);
        }

        private class TestSortingRequest : ISortingRequest {
            public string Field { get; set; }

            public FieldOrder Order { get; set; }

        }
    }
}