using System.Text.Encodings.Web;
using System.Text.Json;

namespace HubSpot.Main
{
    public class Serialization
    {
        public static T Deserialise<T>(string json) => JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            //PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        }); 
        public static string Serialize<T>(T payload) => JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            //PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }
}