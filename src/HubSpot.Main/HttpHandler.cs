using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;

namespace HubSpot.Main
{
    // this class implements the IHubspotHttpHandler interface.
    // This is the code that will make the GET and POST requests when solution is run 
    public class HttpHandler : IHubspotHttpHandler
    {
        public string ApiKey { get; }

        public HttpHandler(string apiKey) => ApiKey = apiKey; 
        
        // Try Deserialise the response from the GET Request to the input type specified 
        public async Task<HubspotInput> TryParseResponse(HttpResponseMessage response)
        {
            try
            {
                var input = await response.Content.ReadFromJsonAsync<HubspotInput>();
                Console.WriteLine($"{input.@base}, {input.date}"); // payloads from test endpoint, ignore 
                return input;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to deserialise input to target type, {e}"); 
            }
        }
        
        // Make HTTP request to hubspot API to get input data 
        public async Task<HubspotInput> GetData(string endpoint)
        {
            HttpClient client = new HttpClient();
            
            var builder = new UriBuilder(endpoint);
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["access_key"] = ApiKey;
            
            builder.Query = query.ToString();
            client.BaseAddress = new Uri(builder.ToString()); 
            
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync("");

            if (response.IsSuccessStatusCode)
            {
                var data = await TryParseResponse(response);
                return data; 
            }

            throw new Exception($"Failed to retrieve data from {endpoint}"); 
        }
        
        // POST transformed data to hubspot API 
        public Task<HttpResponseMessage> PostData(string endpoint, HubspotOutput data)
        {
            throw new NotImplementedException();
        }
    }
}