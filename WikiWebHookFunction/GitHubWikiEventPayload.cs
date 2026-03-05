using System.Text.Json.Serialization;

namespace WikiWebHookFunction
{
    public class GitHubWikiEventPayload
    {
        [JsonPropertyName("pages")]
        public List<WikiPage> pages { get; set; }
    }
    public class WikiPage
    {
        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("html_url")]
        public string Url { get; set; }
	} 
}