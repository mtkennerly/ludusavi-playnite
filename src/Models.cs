using Newtonsoft.Json;
using Playnite.SDK.Models;
using System.Collections.Generic;

namespace LudusaviPlaynite
{
    public enum Language
    {
        English,
    }

    public enum Choice
    {
        Yes,
        Always,
        No,
        Never,
    }

    public static class ChoiceExt
    {
        public static bool Accepted(this Choice choice)
        {
            return choice == Choice.Yes || choice == Choice.Always;
        }
    }

    public struct OperationPreference
    {
        public bool Do;
        public bool Ask;
    }

    public struct OperationPreferences
    {
        public OperationPreference Backup;
        public OperationPreference Restore;
    }

    public struct PlayPreferences
    {
        public OperationPreferences Game;
        public OperationPreferences Platform;
    }

    public struct OperationResult
    {
        public string Name;
        public ApiResponse Response;
    }

    public struct BackupCriteria
    {
        public bool ByPlatform;
    }

    public struct RestorationError
    {
        public string Message;
        public bool Empty;
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
        public ulong TotalBytes;
        [JsonProperty("processedGames")]
        public int ProcessedGames;
        [JsonProperty("processedBytes")]
        public ulong ProcessedBytes;
    }

    public struct ApiFile
    {
        [JsonProperty("failed")]
        public bool Failed;
        [JsonProperty("bytes")]
        public ulong Bytes;
        [JsonProperty("originalPath")]
        public string OriginalPath;
    }

    public struct ApiRegistry
    {
        [JsonProperty("failed")]
        public bool Failed;
    }

    public struct ApiGame
    {
        [JsonProperty("decision")]
        public string Decision;
        [JsonProperty("files")]
        public Dictionary<string, ApiFile> Files;
        [JsonProperty("registry")]
        public Dictionary<string, ApiRegistry> Registry;
    }

    public struct ApiResponse
    {
        [JsonProperty("errors")]
        public ApiErrors Errors;
        [JsonProperty("overall")]
        public ApiOverall Overall;
        [JsonProperty("games")]
        public Dictionary<string, ApiGame> Games;
    }
}
