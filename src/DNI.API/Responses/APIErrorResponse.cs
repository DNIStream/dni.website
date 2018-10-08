using System;
using System.Collections.Generic;

namespace DNI.API.Responses {
    /// <summary>
    ///     Used when returning an exception from the API
    /// </summary>
    public class APIErrorResponse {
        /// <summary>
        ///     The HTTP Status code, if relevant
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        ///     The user-friendly message informing the user what is wrong
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     A list of user-friendly validation error messages
        /// </summary>
        public IEnumerable<string> ValidationErrors { get; set; }

        /// <summary>
        ///     The time that this error occurred
        /// </summary>
        public DateTime TimeStamp { get; } = DateTime.Now;
    }
}