namespace DNI.API.Requests {
    public class ContactRequest {
        public string name { get; set; }

        public string phone { get; set; }

        public string email { get; set; }

        public string message { get; set; }

        public bool marketingOptOut { get; set; }

        public bool deleteDetails { get; set; }
    }
}