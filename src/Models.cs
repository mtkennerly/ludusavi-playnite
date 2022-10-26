using Newtonsoft.Json;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;

namespace LudusaviPlaynite
{
    public class Backup
    {
        public string name;
        public DateTime when;
    }

    public enum Mode
    {
        Backup,
        Backups,
        Restore,
    }

    public class Invocation
    {
        private Mode mode;
        private string game;
        private string path;
        private bool bySteamId;
        private string backup;

        public Invocation(Mode mode)
        {
            this.mode = mode;
        }

        public Invocation Path(string value)
        {
            this.path = value;
            return this;
        }

        public Invocation Game(string value)
        {
            this.game = value;
            this.bySteamId = false;
            return this;
        }

        public Invocation SteamId(string value)
        {
            this.bySteamId = true;
            this.game = value;
            return this;
        }

        public Invocation Backup(string backup)
        {
            this.backup = backup;
            return this;
        }

        public string Render()
        {
            var rendered = "";

            switch (this.mode)
            {
                case Mode.Backup:
                    rendered += "backup --force --merge --try-update";
                    break;
                case Mode.Backups:
                    rendered += "backups";
                    break;
                case Mode.Restore:
                    rendered += "restore --force";
                    break;
            }

            rendered += " --api";

            if (this.path != null && this.path != "")
            {
                rendered += string.Format(" --path \"{0}\"", this.path);
            }

            if (this.bySteamId)
            {
                rendered += " --by-steam-id";
            }

            if (this.backup != null)
            {
                rendered += string.Format(" --backup \"{0}\"", this.backup);
            }

            if (this.game != null && this.game != "")
            {
                rendered += string.Format(" -- \"{0}\"", this.game.Replace("\"", "\"\""));
            }

            return rendered;
        }
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

    public enum OperationTiming
    {
        Free,
        BeforePlay,
        DuringPlay,
        AfterPlay,
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

    public struct ApiBackup
    {
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("when")]
        public DateTime When;
    }

    public struct ApiGame
    {
        [JsonProperty("decision")]
        public string Decision;
        [JsonProperty("files")]
        public Dictionary<string, ApiFile> Files;
        [JsonProperty("registry")]
        public Dictionary<string, ApiRegistry> Registry;
        [JsonProperty("backups")]
        public List<ApiBackup> Backups;
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
