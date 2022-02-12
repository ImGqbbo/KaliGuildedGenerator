using Newtonsoft.Json;
using System;

namespace KaliGuildedGenerator
{
    public class EmailMessage
    {
        [JsonProperty("subject")]
        public string Subject { get; private set; }

        [JsonProperty("to")]
        public string Recipient { get; private set; }

        [JsonProperty("from")]
        public string Author { get; private set; }

        [JsonProperty("body_html")]
        public string Content { get; private set; }

        [JsonProperty("body_text")]
        public string Title { get; private set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; private set; }
    }
}
