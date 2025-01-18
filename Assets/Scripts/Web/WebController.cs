using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEngine;

public class WebController {
    public static string apiURL = "http://localhost:3000";
    public static HttpClient client = new();

    public static async Task<string> Get(string endpoint)
    {
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        HttpResponseMessage response = await client.GetAsync(apiURL + endpoint);
        Debug.Log(await response.Content.ReadAsStringAsync());
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> Post(string endpoint, string json)
    {
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        HttpContent content = new StringContent(json);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        HttpResponseMessage response = await client.PostAsync(apiURL + endpoint, content);
        Debug.Log(await response.Content.ReadAsStringAsync());
        return await response.Content.ReadAsStringAsync();
    }
}