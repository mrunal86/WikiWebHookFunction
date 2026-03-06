using Microsoft.AspNetCore.DataProtection.Repositories;
using System.Text.Json.Serialization;

namespace WikiWebHookFunction
{
    public class GitHubWikiEventPayload
    {
         
        public string action { get; set; }
        public WikiPage pages { get; set; }
        public Repository repository{ get; set; }
    }

    public class Repository
    {
        public string name { get; set; }
    }

    public class WikiPage
    {
        [JsonPropertyName("page_name")]
        public string page_name { get; set; }

        [JsonPropertyName("title")]
        public string title { get; set; }
        [JsonPropertyName("url")]
        public string url { get; set; }
	} 
}