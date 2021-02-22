using System.Net.Http;
using System.Threading.Tasks;

namespace HubSpot.Main
{
    public class Solution
    {
        public IHubspotHttpHandler httpHandler { get; }

        public Solution(IHubspotHttpHandler handler) => httpHandler = handler;

        public async Task<HubspotInput> GetData(string endpoint) => 
            await httpHandler.GetData(endpoint);

        public HubspotOutput Transform(HubspotInput input)
        {
            var output = new HubspotOutput(); // do some data transformation 
            return output; 
        }

        public async Task<HttpResponseMessage> PostData(string endpoint, HubspotOutput output) => 
            await httpHandler.PostData(endpoint, output);

        public async Task<HttpResponseMessage> Run(string getEndpoint, string postEndpoint)
        {
            // GET input data from API 
            var input = await GetData(getEndpoint);
            
            // Transform data to expected POST request format 
            var transformedData = Transform(input);
            
            // POST transformed data to API 
            var response = await PostData(postEndpoint, transformedData);
            
            return response; 
        }
        
    }
}