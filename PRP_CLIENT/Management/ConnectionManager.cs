using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using PRP_CLIENT.Models;

namespace PRP_CLIENT.Management
{
    public static class ConnectionManager
    {
        private static string baseUrl;
        private static HttpClient client;

        static ConnectionManager()
        {
            baseUrl = "http://localhost:65268/";
            client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static async Task<HttpResponseMessage> RegisterUser(User user)
        {
            return await client.PostAsJsonAsync("api/users", user);
        }

        public async static Task<HttpResponseMessage> PostAsync(string route, object obj, string accessToken = "")
        {
            if (accessToken != "")
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            return await client.PostAsJsonAsync(route, obj);
        }

        public async static Task<HttpResponseMessage> PutAsync(string route, object obj, string accessToken = "")
        {
            if (accessToken != "")
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            return await client.PutAsJsonAsync(route, obj);
        }

        public async static Task<HttpResponseMessage> GetAsync(string route, string accessToken = "")
        {
            if (accessToken != "")
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            return await client.GetAsync(route);
        }

        public async static Task<HttpResponseMessage> DeleteAsync(string route, string accessToken = "")
        {
            if (accessToken != "")
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            return await client.DeleteAsync(route);
        }
    }
}
