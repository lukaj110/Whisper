using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Whisper.Client.Models;
using Whisper.Crypto.Algorithms;

namespace Whisper.Client.Helpers
{
    public class APIHelper
    {
        private string key = string.Empty;

        private string baseUrl;

        private HttpClient client;

        public DiffieHellman dh;

        private JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public APIHelper(string baseUrl)
        {
            client = new HttpClient();

            this.baseUrl = baseUrl;
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

                dh.ExportNewKeys(username);

                return StatusCode.OK;
            }
            catch
            {
                return StatusCode.ConnectionRefused;
            }
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

                dh = CryptoHelper.InitializeKeys(username);

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

            return await JsonSerializer.DeserializeAsync<IEnumerable<Chat>>(stream, options);
        }

        public async Task<IEnumerable<Message>> GetMessages(long channelId)
        {
            var response = await client.GetAsync($"{baseUrl}/message/{channelId}");

            if (!response.IsSuccessStatusCode) return null;

            var stream = await response.Content.ReadAsStreamAsync();

            return (await JsonSerializer.DeserializeAsync<IEnumerable<Message>>(stream, options));
        }

        public async Task<IEnumerable<Message>> GetAllMessages()
        {
            var response = await client.GetAsync($"{baseUrl}/message/all");

            if (!response.IsSuccessStatusCode) return null;

            var stream = await response.Content.ReadAsStreamAsync();

            return (await JsonSerializer.DeserializeAsync<IEnumerable<Message>>(stream, options));
        }


        public async Task<Chat> GetUserInfo(string username)
        {
            var response = await client.GetAsync($"{baseUrl}/user/by-username/{username}");

            if (!response.IsSuccessStatusCode) return null;

            var stream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<Chat>(stream, options);
        }

        public async Task<Chat> GetMyInfo()
        {
            var response = await client.GetAsync($"{baseUrl}/user/me");

            if (!response.IsSuccessStatusCode) return null;

            var stream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<Chat>(stream, options);
        }

        public async Task<Message> SendMessage(Message message)
        {
            var content = new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{baseUrl}/message", content);

            if (!response.IsSuccessStatusCode) return null;

            var stream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<Message>(stream, options);
        }
    }
}
