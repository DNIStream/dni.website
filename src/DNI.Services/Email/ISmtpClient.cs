using System.Net.Mail;
using System.Threading.Tasks;

namespace DNI.Services.Email {
    /// <summary>
    ///     Interface for <see cref="SmtpClient" />, which wraps
    ///     <see cref="System.Net.Mail.SmtpClient" />
    /// </summary>
    public interface ISmtpClient {
        void Send(MailMessage message);

        Task SendMailAsync(MailMessage message);

        int Port { get; set; }

        string Host { get; set; }
    }
}