using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Verifile.Models;
using Verifile.Repositories;

namespace Verifile.Function
{
    public class GetPersonInfo
    {
        private readonly ILogger _logger;
        private readonly PersonRepository _personRepository;
        private static readonly HttpClient _httpClient = new HttpClient();

        public GetPersonInfo(ILoggerFactory loggerFactory, PersonRepository personRepository)
        {
            _logger = loggerFactory.CreateLogger<GetPersonInfo>();
            _personRepository = personRepository;
        }

        [Function("GetPersonInfo")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("Received call GetPersonInfo");

            try
            {
                string apiKey = "BZTKiWBeMv8tFb5wFzFcoYBW";
                string personId = "10001";
                string baseUrl = "https://www.theyworkforyou.com/api/getPerson";
                
                string url = $"{baseUrl}?key={apiKey}&id={personId}&output=json";
                
                _logger.LogInformation($"Calling API: {url}");
                
                HttpResponseMessage apiResponse = await _httpClient.GetAsync(url);
                apiResponse.EnsureSuccessStatusCode();
                
                string responseBody = await apiResponse.Content.ReadAsStringAsync();
                _logger.LogInformation("Success response received from API");
                
                var personList = JsonSerializer.Deserialize<List<Person>>(responseBody);

                if (personList == null || !personList.Any())
                {
                    _logger.LogWarning("Data not found from external API");
                    var emptyResponse = req.CreateResponse(HttpStatusCode.NotFound);
                    await emptyResponse.WriteAsJsonAsync(new
                    {
                        success = false,
                        message = "Data not found from external API"
                    });
                    return emptyResponse;
                }
                
                _logger.LogInformation($"List with {personList.Count} elements");
                
                Person latestPerson = GetLatestUpdate(personList);

                if (latestPerson != null)
                {
                    _logger.LogInformation($"Saving person {latestPerson.GivenName}");
                    int idGerado = await _personRepository.SavePersonAsync(latestPerson);
                    _logger.LogInformation($"Success! ID: {idGerado}");
                }
                
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(new
                {
                    success = true,
                    totalElements = personList.Count,
                    latestElement = latestPerson,
                    latestUpdate = latestPerson?.LastUpdate,
                    allData = personList
                });
                
                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP request Error : {ex.Message}");
                
                var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await errorResponse.WriteAsJsonAsync(new
                {
                    success = false,
                    error = "Error to call external API",
                    details = ex.Message
                });
                
                return errorResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteAsJsonAsync(new
                {
                    success = false,
                    error = "Internal server error",
                });
                
                return errorResponse;
            }
        }
        
        private Person? GetLatestUpdate(List<Person> personList)
        {
            return personList?
                .Where(p => p.LastUpdateDate.HasValue)
                .MaxBy(p => p.LastUpdateDate.Value);
        }
    }
}