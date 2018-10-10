using System;

namespace DNI.Options {
    public static class OptionHelper {
        /// <summary>
        ///     Correctly concatenates the base WebBaseUri with the LogoPath
        /// </summary>
        /// <param name="opt"></param>
        /// <returns></returns>
        public static string GetLogoUrl(this GeneralOptions opt) {
            var baseUri = new Uri(opt.WebBaseUri);
            return new Uri(baseUri, opt.LogoPath).AbsolutePath;
        }
    }
}