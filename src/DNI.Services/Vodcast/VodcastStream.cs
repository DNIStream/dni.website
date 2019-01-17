using System.Collections.Generic;

using RestSharp.Deserializers;

namespace DNI.Services.Vodcast {
    public class VodcastStream {
        [DeserializeAs(Name = "items")]
        public List<VodcastShow> Shows { get; set; }

        [DeserializeAs(Name = "pageInfo.totalResults")]
        public int TotalRecords { get; set; }

        [DeserializeAs(Name = "pageInfo.resultsPerPage")]
        public int RecordsPerPage { get; set; }
    }
}