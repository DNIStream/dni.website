using System.Net;
using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.AutoMoq;

using DNI.API.Controllers;
using DNI.API.Requests;
using DNI.API.Responses;
using DNI.Services.Captcha;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

using Xunit;

namespace DNI.API.Tests.Controllers {
    [Trait("TestType", "Unit")]
    public class CaptchaControllerUnitTests {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});

        private readonly Mock<ICaptchaService> captchaServiceMock;

        public CaptchaControllerUnitTests() {
            captchaServiceMock = Mock.Get(_fixture.Create<ICaptchaService>());
        }

        private CaptchaController GetController() {
            return new CaptchaController(captchaServiceMock.Object);
        }

        #region Captcha

        [Fact]
        public async Task Captcha_Returns400_WhenModelStateIsInvalid() {
            // Arrange
            var controller = GetController();
            controller.ModelState.AddModelError(_fixture.Create<string>(), _fixture.Create<string>());
            var request = _fixture.Create<CaptchaRequest>();

            // Act
            var result = await controller.CaptchaAsync(request);

            // Assert
            var httpResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<APIErrorResponse>(httpResult.Value);
        }

        [Fact]
        public async Task Captcha_Returns400_WhenCaptchaVerificationFails() {
            // Arrange
            var request = _fixture.Create<CaptchaRequest>();
            captchaServiceMock
                .Setup(x => x.VerifyAsync(It.Is<string>(s => s == request.UserResponse), It.IsAny<string>()))
                .ReturnsAsync(() => false);

            var controller = GetController();
            SetupContext(controller, _fixture.Create<string>());

            // Act
            var result = await controller.CaptchaAsync(request);

            // Assert
            var httpResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<APIErrorResponse>(httpResult.Value);
            Assert.Contains("The CAPTCHA could not be verified", ((APIErrorResponse)httpResult.Value).ValidationErrors);
        }

        [Fact]
        public async Task Captcha_Returns204_WhenCaptchaVerified() {
            // Arrange
            var request = _fixture.Create<CaptchaRequest>();
            captchaServiceMock
                .Setup(x => x.VerifyAsync(It.Is<string>(s => s == request.UserResponse), It.IsAny<string>()))
                .ReturnsAsync(() => true);

            var controller = GetController();
            SetupContext(controller, _fixture.Create<string>());

            // Act
            var result = await controller.CaptchaAsync(request);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        #endregion

        #region Helpers

        /// <summary>
        ///     Sets up the controller's HTTP context to contain generic required headers
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="audience"></param>
        /// <param name="ip"></param>
        private void SetupContext(CaptchaController controller, string audience, string ip = "127.0.0.1") {
            controller.ControllerContext = new ControllerContext {
                HttpContext = new DefaultHttpContext()
            };
            controller.ControllerContext.HttpContext.Request.Headers["Origin"] = audience;
            controller.ControllerContext.HttpContext.Connection.RemoteIpAddress = IPAddress.Parse(ip);
        }

        #endregion
    }
}