using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Whisper.Client.Models;
using Whisper.Crypto.Algorithms;

namespace Whisper.Client.Helpers
{
    public class APIHelper
    {
        private string key = string.Empty;

        private string baseUrl;

        private HttpClient client;

        public RSA4096 rsa;

        public APIHelper(string baseUrl)
        {
            client = new HttpClient();

            this.baseUrl = baseUrl;

            rsa = CryptoHelper.InitializeKeys();
        }

        public async Task<StatusCode> Register(string email, string username, string password, string pubKey)
        {
            var content = new StringContent(JsonSerializer.Serialize(new { email, username, password, pubKey }), Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync($"{baseUrl}/auth/register", content);

                if (!response.IsSuccessStatusCode) return (StatusCode)response.StatusCode;

                key = await response.Content.ReadAsStringAsync();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);

                return StatusCode.OK;
            }
            catch
            {
                return StatusCode.ConnectionRefused;
            }
        }

        public async Task<StatusCode> AddChat(string text)
        {
            throw new NotImplementedException();
        }

        public async Task<StatusCode> Login(string username, string password)
        {
            var content = new StringContent(JsonSerializer.Serialize(new { username, password }), Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync($"{baseUrl}/auth/login", content);

                if (!response.IsSuccessStatusCode) return (StatusCode)response.StatusCode;

                key = await response.Content.ReadAsStringAsync();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);

                return StatusCode.OK;
            }
            catch
            {
                return StatusCode.ConnectionRefused;
            }
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
