using System.Text.Json.Serialization;

namespace api_ai_rag_intent.Models
{
    // Not being used but could come in handy for other features.
    public class DataSourceParameters
    {
        [JsonPropertyName("endpoint")]
        public string Endpoint { get; set; } = string.Empty;

        [JsonPropertyName("key")]
        public string Key { get; set; } = string.Empty;

        [JsonPropertyName("indexName")]
        public string IndexName { get; set; } = string.Empty;
    }

    public class DataSource
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("parameters")]
        public DataSourceParameters? Parameters { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    public class RootObject
    {
        [JsonPropertyName("temperature")]
        public int Temperature { get; set; }

        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; }

        [JsonPropertyName("top_p")]
        public double TopP { get; set; }

        [JsonPropertyName("dataSources")]
        public List<DataSource>? DataSources { get; set; }

        [JsonPropertyName("messages")]
        public List<Message>? Messages { get; set; }
    }
}
