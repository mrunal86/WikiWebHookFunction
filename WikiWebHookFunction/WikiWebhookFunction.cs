using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json;

namespace WikiWebHookFunction;

public class WikiWebhookFunction
{
    //THis function listen for webhook messages and parses the github wiki event payload
    private readonly ILogger<WikiWebhookFunction> _logger;
    private readonly string _secret;

    public WikiWebhookFunction(ILogger<WikiWebhookFunction> logger)
    {
        _logger = logger;
        _secret = Environment.GetEnvironmentVariable("GitHubWebhookSecret");
    }

    [Function("GitHubWikiWebhook")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("Github webhook received.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        
        if (!req.Headers.TryGetValues("X-Hub-Signature-256", out var signatureHeader))
        {
            var unauthorised = req.CreateResponse(HttpStatusCode.Unauthorized);
            await unauthorised.WriteStringAsync("missing signature");
            return unauthorised;
        }

        var signature = signatureHeader.First();

        if (!IsValidSignature(requestBody, signature))
        {
            var unauthorised = req.CreateResponse(HttpStatusCode.Unauthorized);
            await unauthorised.WriteStringAsync("invalid signature");
            return unauthorised;
        }
        // Parse JSON payload
        var payload = JsonElement.Parse(requestBody);


        string action = payload.GetProperty("action").ToString();
        string repoName = payload.GetProperty("repository").ToString();

        string pageTitle = "N/A";

        if (payload.TryGetProperty("pages", out JsonElement pages))
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

    private bool IsValidSignature(string body, string signature)
    {
        var key = Encoding.UTF8.GetBytes(_secret);
        using var hmac = new HMACSHA256(key);

        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
        var computedSignature = "sha256=" + Convert.ToHexString(hash).ToLower();

        _logger.LogInformation($"Received Signature: {signature}");
        _logger.LogInformation($"Computed Signature: {computedSignature}");

        return computedSignature == signature;
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