using System.Net.Mail;

using DNI.Options;
using DNI.Services.Tests.TestResources;
using DNI.Testing;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Services.Tests.Email {
    [Trait(TraitConstants.TraitTestType, TraitConstants.TraitTestTypeIntegration)]
    public class SmtpClientTests {
        private readonly ITestOutputHelper _output;

        private readonly MailMessage message;
        private readonly IOptions<GeneralOptions> opts;

        private readonly IConfiguration config;

        public SmtpClientTests(ITestOutputHelper output) {
            _output = output;

            message = new MailMessage();
            opts = Microsoft.Extensions.Options.Options.Create(new GeneralOptions());

            config = TestHelpers.GetConfigFromFile();
        }

        [Fact]
        public void Send_SendsSSLMailMessage() {
            // Arrange
            var settings = config.CreateOptions<SMTPTestOptions>("SMTPTestOptions").Value;
            opts.Value.SmtpEnableSSL = false;
            opts.Value.SmtpServer = settings.SMTPServer;
            opts.Value.SmtpServerPort = settings.SMTPPort;
            opts.Value.SmtpUsername = settings.SMTPUsername;
            opts.Value.SmtpPassword = settings.SMTPPassword;

            var contactFrom = new MailAddress(settings.MailFrom);
            message.From = contactFrom;
            message.To.Add(new MailAddress(settings.MailTo));
            message.Subject = "Test Subject";
            message.Body = "Test Body";
            message.ReplyToList.Add(contactFrom);

            var client = new Services.Email.SmtpClient(opts);

            // Act & Asset
            client.Send(message);
        }
    }
}