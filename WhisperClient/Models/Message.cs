using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Whisper.Client.Models
{
    public class Message
    {
        [JsonPropertyName("messageid")]
        public int MessageId { get; set; }
        [JsonPropertyName("content")]
        public string Content { get; set; }
        [JsonPropertyName("sentat")]
        public DateTime SentAt { get; set; }
        [JsonPropertyName("sender")]
        public int Sender { get; set; }
        [JsonPropertyName("channelid")]
        public long ChannelId { get; set; }
        [JsonPropertyName("checksum")]
        public string Checksum { get; set; }
    }
}
