using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace WikiWebHookFunction;

public class WikiWebhookFunction
{
    //THis function listen for webhook messages and parses the github wiki event payload
    private readonly ILogger<WikiWebhookFunction> _logger;

    public WikiWebhookFunction(ILogger<WikiWebhookFunction> logger)
    {
        _logger = logger;
    }

    [Function("GitHubWikiWebhook")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("Github webhook received.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        // Parse JSON payload
        var payload = JsonElement.Parse(requestBody);
         

        string action = payload.GetProperty("action").ToString();
        string repoName = payload.GetProperty("repository").ToString();

        string pageTitle = "N/A";

        if(payload.TryGetProperty("pages", out JsonElement pages))
        {
            pageTitle = pages[0].GetProperty("title").GetString();
        }
        //_logger.LogInformation($"Wiki page '{page.page_name}' was {page.action} by {payload.sender.login}.");

        //string action = root;
        _logger.LogInformation($"Wiki page action: {action}");
        _logger.LogInformation($"Wiki page: {pageTitle}");
        _logger.LogInformation($"Wiki page title: {repoName}");
             
        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteStringAsync($"GitHub Event Processed \nAction:{action}\nRepository:{repoName}\nWikiPage:{pageTitle}.");

        return response;
    }
}

///Test with Postman post request 
///{
  //"action": "edited",
  //"page": {
  //  "page_name": "Career",
  //  "title": "Career",
  //  "html_url": "https://github.com/user/repo/wiki/Career"
  //},
  //"repository": {
  //  "name": "WikiWebhookFunction"/**/
  //}
//}

//Create Webhhok in Github => Gollum event=> Select  from the checkboxes individual 
//Give Azure Function URL
//Create Wiki in the Github 