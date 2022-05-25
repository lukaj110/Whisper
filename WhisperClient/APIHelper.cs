using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Whisper.Client.Models;

namespace Whisper.Client
{
    public class APIHelper
    {
        private string key = string.Empty;

        private string baseUrl;

        private HttpClient client;

        public APIHelper(string baseUrl)
        {
            client = new HttpClient();
            this.baseUrl = baseUrl;
        }

        public async Task<bool> Register(string email, string username, string password)
        {
            var content = new StringContent(JsonSerializer.Serialize(new { email, username, password }));

            var response = await client.PostAsync($"{baseUrl}/auth/register", content);

            if (!response.IsSuccessStatusCode) return false;

            key = await response.Content.ReadAsStringAsync();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);

            return true;
        }

        public async Task<bool> Login(string username, string password)
        {
            var content = new StringContent(JsonSerializer.Serialize(new { username, password }));

            var response = await client.PostAsync($"{baseUrl}/auth/login", content);

            if (!response.IsSuccessStatusCode) return false;

            key = await response.Content.ReadAsStringAsync();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);

            return true;
        }

        public async Task<IEnumerable<Chat>> GetChats()
        {
            var response = await client.GetAsync($"{baseUrl}/chat/active");

            if (!response.IsSuccessStatusCode) return null;

            var stream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<IEnumerable<Chat>>(stream);
        }

        public async Task<IEnumerable<Message>> GetMessages(long channelId)
        {
            var response = await client.GetAsync($"{baseUrl}/chat/message/{channelId}");

            if (!response.IsSuccessStatusCode) return null;

            var stream = await response.Content.ReadAsStreamAsync();

            return (await JsonSerializer.DeserializeAsync<IEnumerable<Message>>(stream)).OrderBy(e => e.SentAt);
        }
    }
}
