using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Verifile.Function;

public class GetPersonFunction
{
    private readonly ILogger<GetPersonFunction> _logger;

    public GetPersonFunction(ILogger<GetPersonFunction> logger)
    {
        _logger = logger;
    }

    [Function("GetPersonFunction")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}