using System.Net.Http;
using System.Threading.Tasks;

namespace HubSpot.Main
{
    // Using an interface to abstract the logic for making HTTP requests in our solution
    // We can mock out this interface and check that our solution works correctly at each step 
    public interface IHubspotHttpHandler
    {
        public Task<HubspotInput> GetData(string endpoint);
        public Task<HttpResponseMessage> PostData(string endpoint, HubspotOutput data); 
    }
}