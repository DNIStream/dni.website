using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.AutoMoq;

using DNI.API.Controllers;
using DNI.API.Requests;
using DNI.API.Responses;
using DNI.Services.Captcha;
using DNI.Services.Shared.Paging;
using DNI.Services.Shared.Sorting;
using DNI.Services.Show;
using DNI.Testing;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

using Xunit;

namespace DNI.API.Tests.Controllers {
    [Trait(TraitConstants.TraitTestType, TraitConstants.TraitTestTypeUnit)]
    public class ShowsControllerUnitTests {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});

        private readonly Mock<IShowService> showServiceMock;

        public ShowsControllerUnitTests() {
            showServiceMock = Mock.Get(_fixture.Create<IShowService>());
        }

        private ShowsController GetController() {
            return new ShowsController(showServiceMock.Object);
        }

        #region GetShowsAsync

        [Fact]
        public async Task GetShowsAsync_Returns400_WhenModelStateIsInvalid() {
            // Arrange
            var controller = GetController();
            controller.ModelState.AddModelError(_fixture.Create<string>(), _fixture.Create<string>());
            var request = _fixture.Create<GetShowsRequest>();

            // Act
            var result = await controller.GetShowsAsync(request);

            // Assert
            var httpResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<APIErrorResponse>(httpResult.Value);
        }

        [Fact]
        public async Task GetShowsAsync_CallsGetShowsAsync_WhenRequestIsValid() {
            // Arrange
            var request = _fixture.Create<GetShowsRequest>();
            var controller = GetController();

            // Act
            await controller.GetShowsAsync(request);
            
            // Assert
            showServiceMock
                .Verify(x => x.GetShowsAsync(It.Is<IPagingRequest>(p => p == request), It.Is<ISortingRequest>(s => s == request), It.Is<string>(s => s == null)), Times.Once());
        }

        [Fact]
        public async Task GetShowsAsync_Returns204NoContent_WhenNoResponseIsReturned() {
            // Arrange
            var request = _fixture.Create<GetShowsRequest>();
            showServiceMock
                .Setup(x => x.GetShowsAsync(It.IsAny<IPagingRequest>(), It.IsAny<ISortingRequest>(), It.IsAny<string>()))
                .ReturnsAsync(() => null);

            var controller = GetController();

            // Act
            var result = await controller.GetShowsAsync(request);
            
            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetShowsAsync_Returns200AndPagedShowData_WhenShowsAreFound() {
            // Arrange
            var request = _fixture.Create<GetShowsRequest>();
            var pagedResponse = _fixture.Create<IPagedResponse<Show>>();
            showServiceMock
                .Setup(x => x.GetShowsAsync(It.IsAny<IPagingRequest>(), It.IsAny<ISortingRequest>(), It.IsAny<string>()))
                .ReturnsAsync(() => pagedResponse);

            var controller = GetController();

            // Act
            var result = await controller.GetShowsAsync(request);
            
            // Assert
            Assert.IsType<OkObjectResult>(result);
            var resultData = ((OkObjectResult)result).Value;
            Assert.IsType<ShowListAPIResponse>(resultData);
            var actualData = (ShowListAPIResponse) resultData;
            Assert.Equal(pagedResponse.StartIndex, actualData.PageInfo.StartIndex);
            Assert.Equal(pagedResponse.TotalRecords, actualData.PageInfo.TotalRecords);
            Assert.Equal(pagedResponse.EndIndex, actualData.PageInfo.EndIndex);
            Assert.Equal(pagedResponse.CurrentPage, actualData.PageInfo.CurrentPage);
            Assert.Equal(pagedResponse.TotalPages, actualData.PageInfo.TotalPages);
            Assert.Equal(pagedResponse.ItemsPerPage, actualData.PageInfo.ItemsPerPage);
            Assert.Equal(pagedResponse.Items, actualData.PagedShows);
        }

        [Fact]
        public async Task GetShowsAsync_Returns200AndKeywordData_WhenShowsAreFound() {
            // Arrange
            var request = _fixture.Create<GetShowsRequest>();
            var keywordResponse = _fixture.Create<Dictionary<string, int>>();
            showServiceMock
                .Setup(x => x.GetAggregatedKeywords())
                .ReturnsAsync(() => keywordResponse)
                .Verifiable();

            var controller = GetController();

            // Act
            var result = await controller.GetShowsAsync(request);
            
            // Assert
            showServiceMock.Verify();
            Assert.IsType<OkObjectResult>(result);
            var resultData = ((OkObjectResult)result).Value;
            Assert.IsType<ShowListAPIResponse>(resultData);
            var actualData = (ShowListAPIResponse) resultData;
            Assert.Equal(keywordResponse, actualData.ShowKeywords);
        }

        [Fact]
        public async Task GetShowsAsync_CallsGetShowsAsyncKeywordOverload_WhenKeywordIsNotNull() {
            // Arrange
            var request = _fixture.Create<GetShowsRequest>();
            var expectedKeyword = _fixture.Create<string>();

            var controller = GetController();

            // Act
            await controller.GetShowsAsync(request, expectedKeyword);
            
            // Assert
            showServiceMock
                .Verify(x => x.GetShowsAsync(It.Is<IPagingRequest>(p => p == request),
                    It.Is<ISortingRequest>(s => s == request),
                    It.Is<string>(k => k == expectedKeyword)), Times.Once());
        }

        #endregion

        #region GetShowBySlugAsync

        [Fact]
        public async Task GetShowBySlugAsync_Returns400_WhenModelStateIsInvalid() {
            // Arrange
            var controller = GetController();
            controller.ModelState.AddModelError(_fixture.Create<string>(), _fixture.Create<string>());
            var request = _fixture.Create<string>();

            // Act
            var result = await controller.GetShowBySlugAsync(request);

            // Assert
            var httpResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<APIErrorResponse>(httpResult.Value);
        }

        [Fact]
        public async Task GetShowBySlugAsync_Returns400_WhenSlugNotFound() {
            // Arrange
            var request = _fixture.Create<string>();
            showServiceMock
                .Setup(x => x.GetShowBySlugAsync(It.Is<string>(s => s == request)))
                .ThrowsAsync(new InvalidOperationException())
                .Verifiable();

            var controller = GetController();

            // Act
            var result = await controller.GetShowBySlugAsync(request);

            // Assert
            showServiceMock.Verify();
            var httpResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<APIErrorResponse>(httpResult.Value);
            Assert.Contains("The specified slug does not exist", ((APIErrorResponse) httpResult.Value).ValidationErrors);
        }

        [Fact]
        public async Task GetShowBySlugAsync_Returns200AndData_WhenSlugFound() {
            // Arrange
            var request = _fixture.Create<string>();
            var show = _fixture.Create<Show>();
            showServiceMock
                .Setup(x => x.GetShowBySlugAsync(It.IsAny<string>()))
                .ReturnsAsync(() => show);

            var controller = GetController();

            // Act
            var result = await controller.GetShowBySlugAsync(request);
            
            // Assert
            Assert.IsType<OkObjectResult>(result);
            var resultData = ((OkObjectResult)result).Value;
            Assert.IsType<Show>(resultData);
            Assert.Equal(show, (Show)resultData);
        }

        #endregion

    }
}