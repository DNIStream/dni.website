namespace DNI.API.Requests {
    /// <summary>
    ///     A standard captcha verification request
    /// </summary>
    public class CaptchaRequest {
        /// <summary>
        ///     The response token to validate
        /// </summary>
        public string UserResponse { get; set; }
    }
}