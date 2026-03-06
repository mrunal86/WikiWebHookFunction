using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

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

       var wikiPage = System.Text.Json.JsonSerializer.Deserialize<GitHubWikiEventPayload>(requestBody);
                 //_logger.LogInformation($"Wiki page '{page.page_name}' was {page.action} by {payload.sender.login}.");
       
        _logger.LogInformation($"Wiki page action: {wikiPage?.action}");
        _logger.LogInformation($"Wiki page: {wikiPage?.pages?.title}");
        _logger.LogInformation($"Wiki page title: {wikiPage?.repository?.name}");
             
        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteStringAsync("Webhook received and processed.");

        return response;
    }
}