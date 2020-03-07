using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.AutoMoq;

using DNI.Services.Email;
using DNI.Testing;

using Moq;

using SendGrid;
using SendGrid.Helpers.Mail;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Services.Tests.Email {
    [Trait(TraitConstants.TraitTestType, TraitConstants.TraitTestTypeUnit)]
    public class SendGridEmailServiceTests {
        private readonly ITestOutputHelper _output;
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});
        private ISendGridClient _smtpClient;
        private Mock<ISendGridClient> _smtpClientMock;

        public SendGridEmailServiceTests(ITestOutputHelper output) {
            _output = output;
        }

        private SendGridEmailService GetService() {
            _smtpClient = _fixture.Create<ISendGridClient>();
            _smtpClientMock = Mock.Get(_smtpClient);
            _smtpClientMock
                .Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => null); // Fixes "parameterless constructor" error in tests

            return new SendGridEmailService(_smtpClient);
        }

        #region Validation

        [Fact]
        public async Task SendAsync_ThrowsArgumentNull_WhenRequestIsNull() {
            // Arrange
            var service = GetService();

            // Act // Assert
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => service.SendAsync(null));

            Assert.Equal("req", ex.ParamName);
        }

        [Theory]
        [InlineData(null, null, null, null, "To")]
        [InlineData("a@email.com", null, null, null, "From")]
        [InlineData("a@email.com", "from@from.com", null, null, "Subject")]
        [InlineData("a@email.com", "from@from.com", "Test Subject", null, "Body")]
        public async Task SendAsync_ValidatesRequest_ThrowsArgumentNull(string to, string from, string subject, string body, string paramName) {
            // Arrange
            var req = new SendEmailRequest {
                To = to,
                From = from,
                Subject = subject,
                Body = body
            };
            var service = GetService();

            // Act // Assert
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => service.SendAsync(req));

            Assert.Equal(paramName, ex.ParamName);
        }

        #endregion

        [Fact]
        public async Task SendAsync_ReturnsTrueWhenValidRequest() {
            // Arrange
            var service = GetService();
            var req = new SendEmailRequest {
                To = "test@test.com",
                From = "from@from.com",
                Body = "test body",
                Subject = "test subject",
                ReplyTo = "reply@test.com"
            };

            // Act
            var result = await service.SendAsync(req);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SendAsync_SendEmailAsync_OnSmtpClient() {
            // Arrange
            var service = GetService();
            var req = new SendEmailRequest {
                To = "test@test.com",
                From = "from@from.com",
                Body = "test body",
                Subject = "test subject",
                ReplyTo = "reply@test.com"
            };

            // Act
            await service.SendAsync(req);

            // Assert
            var mock = Mock.Get(_smtpClient);
            mock.Verify(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task SendAsync_SendsEmail_WithSpecifiedSubjectFromAndPlainTextBody() {
            // Arrange
            var service = GetService();
            var req = new SendEmailRequest {
                To = "test@test.com",
                From = "from@from.com",
                Body = "test body",
                Subject = "test subject",
                ReplyTo = "reply@test.com"
            };

            // Act
            await service.SendAsync(req);

            // Assert
            var mock = Mock.Get(_smtpClient);
            mock.Verify(x => x.SendEmailAsync(It.Is<SendGridMessage>(m =>
                    m.Subject == req.Subject
                    && m.From.Email == req.From
                    && m.PlainTextContent == req.Body
                    && m.HtmlContent == null),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task CompileAndSendAsync_SendsExpectedEmailContent() {
            // Arrange
            var service = GetService();

            const string templateResourcePath = "DNI.Services.Tests.TestResources.contact.json";
            const string htmlLayoutResourcePath = "DNI.Services.Tests.TestResources.layout.html";

            var req = new CompileAndSendEmailRequest {
                To = "test@test.com",
                ResourceAssembly = Assembly.GetExecutingAssembly(),
                EmailTemplateResourceId = templateResourcePath,
                HtmlLayoutResourceId = htmlLayoutResourcePath,
                TemplateReplacements = new Dictionary<string, string> {
                    {"test1", "capybara"},
                    {"test2", "ferret"},
                    {"test3", "badger"},
                    {"test4", "platypus"}
                }
            };

            const string expectedHtmlBody = @"<!doctype html>
<html>

<head>
    <meta name=""viewport""
          content=""width=device-width""/>
    <meta http-equiv=""Content-Type""
          content=""text/html; charset=UTF-8""/>
    <title>Test Email Layout</title>
</head>

<body>
<div class=""preHeaderHtml"">This is a ferret</div>
<div class=""introHtml"">This is a badger</div>
<div class=""actionLink"">http://www.test.com</div>
<div class=""actionText"">Test Link</div>
<div class=""outroHtml"">This is a platypus</div>
</body>

</html>";
            // Act
            await service.CompileAndSendAsync(req);

            // Assert
            var mock = Mock.Get(_smtpClient);
            mock.Verify(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(),
                It.IsAny<CancellationToken>()), Times.Once(), "SendEmailAsync was not called exactly once");
            mock.Verify(x => x.SendEmailAsync(
                It.Is<SendGridMessage>(m => m.PlainTextContent == "This is a plain text body with capybara and platypus replacements"),
                It.IsAny<CancellationToken>()), Times.Once(), "PlainTextContent mismatch");
            mock.Verify(x => x.SendEmailAsync(
                It.Is<SendGridMessage>(m => m.HtmlContent == expectedHtmlBody),
                It.IsAny<CancellationToken>()), Times.Once(), "HtmlContent mismatch");
            mock.Verify(x => x.SendEmailAsync(
                It.Is<SendGridMessage>(m => m.Subject == "This is a capybara"),
                It.IsAny<CancellationToken>()), Times.Once(), "Subject mismatch");
            mock.Verify(x => x.SendEmailAsync(
                    It.Is<SendGridMessage>(m => m.From.Email == "from@test.com"),
                    It.IsAny<CancellationToken>()), Times.Once(),
                "From email should come from the resource as no From address was specified in the request.");
        }
    }
}