﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Whisper.Crypto.Algorithms;

namespace Whisper.Client.Models
{
    public class Chat
    {
        [JsonPropertyName("userid")]
        public int UserId { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("pubkey")]
        public string PubKey { get; set; }
        [JsonPropertyName("channelid")]
        public long ChannelId { get; set; }

        public AES256 Aes = null;

        public List<Message> Messages = new List<Message>();
    }
}
