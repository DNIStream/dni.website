using System.Collections.Generic;
using System.Security.Authentication;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DNI.API {
    /// <summary>
    ///     Provides various MVC helpers
    /// </summary>
    public static class Helpers {
        /// <summary>
        ///     Helper method to retrieve the Origin request headers.
        /// </summary>
        /// <returns></returns>
        public static string GetRequestOrigin(this HttpRequest request) {
            var origin = request?.Headers["Origin"];
            if(origin.HasValue && origin.Value.Count > 0) {
                return origin.Value[0];
            }

            throw new AuthenticationException("Unable to find Origin header.");
        }

        /// <summary>
        ///     Retrieves the requesting client's IP Address
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetClientIp(this HttpRequest request) {
            return request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        /// <summary>
        ///     Returns a simple array of errors stored in the model state
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static IEnumerable<string> FlattenErrors(this ModelStateDictionary modelState) {
            foreach(var kvp in modelState.Values) {
                foreach(var error in kvp.Errors) {
                    yield return error.ErrorMessage;
                }
            }
        }
    }
}