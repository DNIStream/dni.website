using System.Threading.Tasks;

namespace DNI.Services.Captcha {
    public interface ICaptchaService {
        Task<bool> VerifyAsync(string userResponse, string userIP);
    }
}