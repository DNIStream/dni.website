using System;

using RestSharp.Deserializers;

namespace DNI.Services.Captcha {
    public class SiteVerifyResponse {
        public bool Success { get; set; }

        [DeserializeAs(Name = "challenge_ts")]
        public DateTime ChallengeTS { get; set; }

        public string Hostname { get; set; }

        [DeserializeAs(Name = "error-codes")]
        public string[] ErrorCodes { get; set; }
    }
}