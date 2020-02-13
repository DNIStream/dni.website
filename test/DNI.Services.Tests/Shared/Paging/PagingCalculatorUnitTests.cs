using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.AutoMoq;

using DNI.Services.Shared.Paging;
using DNI.Testing;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Services.Tests.Shared.Paging {
    [Trait(TraitConstants.TraitTestType, TraitConstants.TraitTestTypeUnit)]
    public class PagingCalculatorUnitTests {
        private readonly ITestOutputHelper _output;
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});

        public PagingCalculatorUnitTests(ITestOutputHelper output) {
            _output = output;
        }

        private IPagingCalculator<string> GetCalculator() {
            return new PagingCalculator<string>();
        }

        [Fact]
        public async Task Calculate_ThrowsException_IfPagingRequestIsNull() {
            // Arrange
            var results = _fixture.CreateMany<string>(56).ToArray();
            var calculator = GetCalculator();

            // Act
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => calculator.PageItemsAsync<TestPagedResponse>(results, null));

            // Assert
            Assert.Equal("pagingRequest", ex.ParamName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public async Task Calculate_ThrowsException_IfPagingRequestHasInvalidItemsPerPage(int itemsPagePage) {
            // Arrange
            var results = _fixture.CreateMany<string>(56).ToArray();
            var pagingInfo = new TestPagingRequest {
                ItemsPerPage = itemsPagePage,
                PageNumber = 1
            };
            var calculator = GetCalculator();

            // Act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => calculator.PageItemsAsync<TestPagedResponse>(results, pagingInfo));

            // Assert
            Assert.Equal("pagingRequest.ItemsPerPage must be greater than zero", ex.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public async Task Calculate_ThrowsException_IfPagingRequestHasInvalidPageNumber(int pageNumber) {
            // Arrange
            var results = _fixture.CreateMany<string>(56).ToArray();
            var pagingInfo = new TestPagingRequest {
                ItemsPerPage = 10,
                PageNumber = pageNumber
            };
            var calculator = GetCalculator();

            // Act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => calculator.PageItemsAsync<TestPagedResponse>(results, pagingInfo));

            // Assert
            Assert.Equal("pagingRequest.PageNumber must be greater than zero", ex.Message);
        }

        [Fact]
        public async Task Calculate_ReturnsResponse() {
            // Arrange
            var results = _fixture.CreateMany<string>(56).ToArray();
            var pagingInfo = new TestPagingRequest {
                ItemsPerPage = 10,
                PageNumber = 1
            };
            var calculator = GetCalculator();

            // Act
            var pagedResponse = await calculator.PageItemsAsync<TestPagedResponse>(results, pagingInfo);

            // Assert
            Assert.NotNull(pagedResponse);
        }

        [Theory]
        [InlineData(56, 1, 10, 1, 0, 9, 6)] // Checks if first page returns expected results
        [InlineData(56, 2, 10, 2, 10, 19, 6)] // Checks if page 2 returns expected results
        [InlineData(81, 9, 10, 9, 80, 80, 9)] // Checks if last page with remainder returns expected results
        [InlineData(82, 9, 10, 9, 80, 81, 9)] // Duplicate of above test for sanity (with one more record)
        [InlineData(58, 7, 10, 6, 50, 57, 6)] // Checks if total pages is returns if requested page number is greater than total pages
        [InlineData(100, 10, 10, 10, 90, 99, 10)] // Checks if last page without remainder returns expected results
        [InlineData(100, 3, 5, 3, 10, 14, 20)] // Tests varied page length
        [InlineData(0, 1, 7, -1, -1, -1, 0)] // Tests zero length enumeration for page 1
        [InlineData(0, 3, 20, -1, -1, -1, 0)] // Tests zero length enumeration for page 3
        public async Task Calculate_ReturnsExpectedPagingInformation(
            int totalRecords, int requestedPageNo, int requestedItemsPerPage, int expectedCurrentPage,
            int expectedStartIndex, int expectedEndIndex, int expectedTotalPages) {
            // Arrange
            var results = _fixture.CreateMany<string>(totalRecords).ToArray();
            var pagingInfo = new TestPagingRequest {
                ItemsPerPage = requestedItemsPerPage,
                PageNumber = requestedPageNo
            };
            var calculator = GetCalculator();

            // Act
            var pagedResponse = await calculator.PageItemsAsync<TestPagedResponse>(results, pagingInfo);

            // Assert
            Assert.Equal(expectedCurrentPage, pagedResponse.CurrentPage);
            Assert.Equal(totalRecords, pagedResponse.TotalRecords);
            Assert.Equal(expectedTotalPages, pagedResponse.TotalPages);
            Assert.Equal(expectedStartIndex, pagedResponse.StartIndex);
            Assert.Equal(expectedEndIndex, pagedResponse.EndIndex);
        }

        [Fact]
        public async Task Calculate_ReturnsExpectedPagedItems_ForFirstPage() {
            // Arrange
            var results = _fixture.CreateMany<string>(56).ToArray();
            var pagingInfo = new TestPagingRequest {
                ItemsPerPage = 10,
                PageNumber = 1
            };
            var calculator = GetCalculator();
            var expectedResults = results.Take(10);

            // Act
            var pagedResponse = await calculator.PageItemsAsync<TestPagedResponse>(results, pagingInfo);

            // Assert
            Assert.NotNull(pagedResponse.Items);
            Assert.Equal(10, pagedResponse.Items.Count());
            Assert.True(pagedResponse.Items.SequenceEqual(expectedResults));
        }

        [Fact]
        public async Task Calculate_ReturnsExpectedPagedItems_ForSecondPage() {
            // Arrange
            var results = _fixture.CreateMany<string>(56).ToArray();
            var pagingInfo = new TestPagingRequest {
                ItemsPerPage = 10,
                PageNumber = 2
            };
            var calculator = GetCalculator();
            var expectedResults = results.Skip(10).Take(10);

            // Act
            var pagedResponse = await calculator.PageItemsAsync<TestPagedResponse>(results, pagingInfo);

            // Assert
            Assert.NotNull(pagedResponse.Items);
            Assert.Equal(10, pagedResponse.Items.Count());
            Assert.True(pagedResponse.Items.SequenceEqual(expectedResults));
        }

        [Fact]
        public async Task Calculate_ReturnsExpectedPagedItems_ForLastPage() {
            // Arrange
            var results = _fixture.CreateMany<string>(53).ToArray();
            var pagingInfo = new TestPagingRequest {
                ItemsPerPage = 10,
                PageNumber = 6
            };
            var calculator = GetCalculator();
            var expectedResults = results.TakeLast(3);

            // Act
            var pagedResponse = await calculator.PageItemsAsync<TestPagedResponse>(results, pagingInfo);

            // Assert
            Assert.NotNull(pagedResponse.Items);
            Assert.Equal(3, pagedResponse.Items.Count());
            Assert.True(pagedResponse.Items.SequenceEqual(expectedResults));
        }

        private class TestPagingRequest : IPagingRequest {
            public int PageNumber { get; set; }

            public int ItemsPerPage { get; set; }
        }

        private class TestPagedResponse : IPagedResponse<string> {
            public int CurrentPage { get; set; }

            public int TotalRecords { get; set; }

            public int TotalPages { get; set; }

            public int StartIndex { get; set; }

            public int EndIndex { get; set; }

            public IEnumerable<string> Items { get; set; }
        }
    }
}