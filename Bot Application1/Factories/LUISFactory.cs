using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace YesBot
{
    public class LUISFactory
    {
        public static async Task<LUISResponse> GetIntentFromLUIS(string Query)
        {
            Query = Uri.EscapeDataString(Query);
            LUISResponse Data = new LUISResponse();
            using (HttpClient client = new HttpClient())
            {
                string RequestURI = "https://api.projectoxford.ai/luis/v1/application?id=" + Environment.GetEnvironmentVariable("YESBOT.LUIS_ID") + "&subscription-key=" + Environment.GetEnvironmentVariable("YESBOT.LUIS_PASSWORD") + "&q=" + Query;
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Data = JsonConvert.DeserializeObject<LUISResponse>(JsonDataResponse);
                }
            }
            return Data;
        }
    }

    public class LUISResponse
    {
        public string query { get; set; }
        public Intent[] intents { get; set; }
        public Entity[] entites { get; set; }
    }

    public class Intent
    {
        public string intent { get; set; }
        public float score { get; set; }
    }

    public class Entity
    {
        public string entity { get; set; }
        public string type { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public float score { get; set; }
    }
}