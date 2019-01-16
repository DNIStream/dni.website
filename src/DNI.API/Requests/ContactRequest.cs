#pragma warning disable IDE1006 // Naming Styles
namespace DNI.API.Requests {
    /// <summary>
    ///     A request message for submitting the contact form
    /// </summary>
    public class ContactRequest {
        /// <summary>
        ///     The sender's name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        ///     The sender's email address
        /// </summary>
        public string email { get; set; }

        /// <summary>
        ///     The message to be sent
        /// </summary>
        public string message { get; set; }

        /// <summary>
        ///     Indicates if the sender wants to be included in marketing lists
        /// </summary>
        public bool marketingOptOut { get; set; }

        /// <summary>
        ///     indicates if the sender wants their details deleted once their inquiry has been dealt with
        /// </summary>
        public bool deleteDetails { get; set; }
    }
}