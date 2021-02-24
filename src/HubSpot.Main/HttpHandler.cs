using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;

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

            Client.DefaultRequestHeaders.Accept.Add( // may need charset UTF-8 
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
 
        public string BuildUriWithApiKey(string endpoint, string queryParam) => 
            $"{endpoint}?{queryParam}={ApiKey}";
        
        // Make HTTP request to hubspot API to get input data 
        public async Task<HubspotInput> GetData(string endpoint)
        {
            var uriWithApiKey = BuildUriWithApiKey(endpoint, "userKey");
            var request = new HttpRequestMessage(HttpMethod.Get, uriWithApiKey); 
            
            HttpResponseMessage response = await Client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var input = Serialization.Deserialise<List<HubspotInput>>(json); 
                
                return input[0]; // change back to input type and not List<input>
            }

            throw new Exception($"Failed to retrieve data from {endpoint}"); 
        }
        
        // POST transformed data to hubspot API 
        public async Task<HttpResponseMessage> PostData(string endpoint, HubspotOutput data)
        {
            var uriWithApiKey = BuildUriWithApiKey(endpoint, "userKey");
            var request = new HttpRequestMessage(HttpMethod.Post, uriWithApiKey);
            request.Content = new StringContent(Serialization.Serialize("data"));

            var response = await Client.SendAsync(request);
            return response; 
        }
    }
    
    public class HttpHandlerOriginal : IHubspotHttpHandler
    {
        public string ApiKey { get; }
        public HttpClient client;

        public HttpHandlerOriginal(string apiKey, string baseUrl)
        {
            ApiKey = apiKey; 
        }
        
        // Try Deserialise the response from the GET Request to the input type specified 
        public async Task<T> TryParseResponse<T>(HttpResponseMessage response)
        {
            try
            {
                var json = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(json);
                var input = Serialization.Deserialise<T>(json); 
                
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


            var request = new HttpRequestMessage(HttpMethod.Get, ""); 
            
            HttpResponseMessage response = await client.SendAsync(request);


            if (response.IsSuccessStatusCode)
            {
                var data = await TryParseResponse<HubspotInput>(response);
                return data; 
            }

            throw new Exception($"Failed to retrieve data from {endpoint}"); 
        }
        
        // POST transformed data to hubspot API 
        public async Task<HttpResponseMessage> PostData(string endpoint, HubspotOutput data)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(endpoint); 
            
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            
            client.DefaultRequestHeaders
                .Add("Authorization", "Bearer 21ef6cdd7af72725cfe07365b54819d1bc995e438b89068cddc075c6355097de");



            var content = JsonContent.Create(data);

            var response = await  client.PostAsync(endpoint, content);
            return response; 
        }
    }
    
}