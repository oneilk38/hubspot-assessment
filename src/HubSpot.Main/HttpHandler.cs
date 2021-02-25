using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HubSpot.Main
{
    // this class implements the IHubspotHttpHandler interface.
    // This is the code that will make the GET and POST requests when solution is run 
    public class HttpHandler : IHubspotHttpHandler
    {
        public string ApiKey { get; }
        public HttpClient Client;

        public HttpHandler(string apiKey, string baseUrl)
        {
            ApiKey = apiKey; 
            Client = new HttpClient();
            
            Client.BaseAddress = new Uri(baseUrl);

            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
 
        public string BuildUriWithApiKey(string endpoint, string queryParam) => 
            $"{Client.BaseAddress}{endpoint}?{queryParam}={ApiKey}";
        
        // Make HTTP request to hubspot API to get input data 
        public async Task<Dataset> GetData(string endpoint)
        {
            var uriWithApiKey = BuildUriWithApiKey(endpoint, "userKey");
            
            var request = new HttpRequestMessage(HttpMethod.Get, uriWithApiKey); 

            HttpResponseMessage response = await Client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                
                var input = Serialization.Deserialise<Dataset>(json); 
                
                return input;
            }

            throw new Exception($"Failed to retrieve data from {endpoint}"); 
        }
        
        // POST transformed data to hubspot API 
        public async Task<HttpResponseMessage> PostData(string endpoint, Conversations data)
        {
            var uriWithApiKey = BuildUriWithApiKey(endpoint, "userKey");

            var request = new HttpRequestMessage(HttpMethod.Post, uriWithApiKey);
            
            request.Content = new StringContent(Serialization.Serialize(data), System.Text.Encoding.UTF8, "application/json");
            
            request.Headers.Add("Accept", "application/json");

            var response = await Client.SendAsync(request);
            return response; 
        }
    }
}