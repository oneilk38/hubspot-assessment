using System.Text.Encodings.Web;
using System.Text.Json;

namespace HubSpot.Main
{
    public class Serialization
    {
        public static T Deserialise<T>(string json) => JsonSerializer.Deserialize<T>(json); 
        public static string Serialize<T>(T payload) => JsonSerializer.Serialize(payload);
    }
}