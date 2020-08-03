using Newtonsoft.Json;
using Playnite.SDK.Models;
using System.Collections.Generic;

namespace LudusaviPlaynite
{
    public enum Language
    {
        English,
    }

    public struct OperationResult
    {
        public Game Game;
        public string Name;
        public ApiResponse Response;
    }

    public struct ApiErrors
    {
        [JsonProperty("someGamesFailed")]
        public bool SomeGamesFailed;
        [JsonProperty("unknownGames")]
        public List<string> UnknownGames;
    }

    public struct ApiOverall
    {
        [JsonProperty("totalGames")]
        public int TotalGames;
        [JsonProperty("totalBytes")]
        public int TotalBytes;
        [JsonProperty("processedGames")]
        public int ProcessedGames;
        [JsonProperty("processedBytes")]
        public int ProcessedBytes;
    }

    public struct ApiResponse
    {
        [JsonProperty("errors")]
        public ApiErrors Errors;
        [JsonProperty("overall")]
        public ApiOverall Overall;
    }
}
