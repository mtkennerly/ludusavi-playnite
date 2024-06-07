using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LudusaviPlaynite
{
    public class Backup
    {
        public string name;
        public DateTime when;
    }

    public enum Operation
    {
        Backup,
        Restore,
    }

    public enum Mode
    {
        Backup,
        Backups,
        Find,
        Restore,
        ManifestShow,
    }

    public class Invocation
    {
        private Mode mode;
        private List<string> games;
        private string path;
        private bool bySteamId;
        private int? steamId;
        private string backup;
        private bool findBackup;
        private bool normalized;

        public Invocation(Mode mode)
        {
            this.mode = mode;
            this.games = new List<string>();
            this.steamId = null;
        }

        public Invocation PathIf(string value, bool condition)
        {
            if (condition)
            {
                this.path = value;
            }
            return this;
        }

        public Invocation AddGame(string value)
        {
            this.games.Add(value);
            return this;
        }

        public Invocation Game(string value)
        {
            this.games.Clear();
            this.games.Add(value);
            this.bySteamId = false;
            return this;
        }

        public Invocation BySteamId(string value)
        {
            this.bySteamId = true;
            this.games.Clear();
            this.games.Add(value);
            return this;
        }

        public Invocation SteamId(int value)
        {
            this.steamId = value;
            return this;
        }

        public Invocation Backup(string backup)
        {
            this.backup = backup;
            return this;
        }

        public Invocation FindBackup()
        {
            this.findBackup = true;
            return this;
        }

        public Invocation Normalized()
        {
            this.normalized = true;
            return this;
        }

        private string Quote(string text)
        {
            return string.Format("\"{0}\"", text);
        }

        public string Render(LudusaviPlayniteSettings settings, LudusaviVersion version)
        {
            var parts = new List<String>();

            if (version.hasGlobalManifestUpdateFlag())
            {
                parts.Add("--try-manifest-update");
            }

            switch (this.mode)
            {
                case Mode.Backup:
                    parts.Add("backup");
                    parts.Add("--force");
                    if (version.requiresMergeFlag())
                    {
                        parts.Add("--merge");
                    }
                    if (!version.hasGlobalManifestUpdateFlag())
                    {
                        parts.Add("--try-update");
                    }
                    break;
                case Mode.Backups:
                    parts.Add("backups");
                    break;
                case Mode.Find:
                    parts.Add("find");
                    break;
                case Mode.Restore:
                    parts.Add("restore");
                    parts.Add("--force");
                    break;
                case Mode.ManifestShow:
                    parts.Add("manifest");
                    parts.Add("show");
                    break;
            }

            parts.Add("--api");

            if (this.path != null && this.path != "")
            {
                parts.Add("--path");
                parts.Add(Quote(this.path));
            }

            if (this.bySteamId)
            {
                parts.Add("--by-steam-id");
            }

            if (this.steamId != null)
            {
                parts.Add("--steam-id");
                parts.Add(this.steamId.ToString());
            }

            if (this.backup != null)
            {
                parts.Add("--backup");
                parts.Add(Quote(this.backup));
            }

            if (this.findBackup)
            {
                parts.Add("--backup");
            }

            if (this.normalized)
            {
                parts.Add("--normalized");
            }

            if (this.mode == Mode.Backup && version.supportsCustomizingBackupFormat())
            {
                if (settings.OverrideBackupFormat)
                {
                    parts.Add("--format");
                    switch (settings.BackupFormat)
                    {
                        case BackupFormatType.Simple:
                            parts.Add("simple");
                            break;
                        case BackupFormatType.Zip:
                            parts.Add("zip");
                            break;
                    }
                }
                if (settings.OverrideBackupCompression)
                {
                    parts.Add("--compression");
                    switch (settings.BackupCompression)
                    {
                        case BackupCompressionType.None:
                            parts.Add("none");
                            break;
                        case BackupCompressionType.Deflate:
                            parts.Add("deflate");
                            break;
                        case BackupCompressionType.Bzip2:
                            parts.Add("bzip2");
                            break;
                        case BackupCompressionType.Zstd:
                            parts.Add("zstd");
                            break;
                    }
                }
                if (settings.OverrideBackupRetention)
                {
                    parts.Add("--full-limit");
                    parts.Add(settings.FullBackupLimit.ToString());
                    parts.Add("--differential-limit");
                    parts.Add(settings.DifferentialBackupLimit.ToString());
                }
            }

            if (this.games.Count > 0)
            {
                parts.Add("--");
                foreach (var game in this.games)
                {
                    parts.Add(Quote(game.Replace("\"", "\"\"")));
                }
            }

            return String.Join(" ", parts);
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

    public static class PlayPreferencesExt
    {
        public static bool ShouldAsk(this PlayPreferences prefs, OperationTiming timing, BackupCriteria criteria, Operation operation)
        {
            var byPref = false;
            switch (criteria)
            {
                case BackupCriteria.Game:
                    switch (operation)
                    {
                        case Operation.Backup:
                            byPref = prefs.Game.Backup.Ask;
                            break;
                        case Operation.Restore:
                            byPref = prefs.Game.Restore.Ask;
                            break;
                    }
                    break;
                case BackupCriteria.Platform:
                    switch (operation)
                    {
                        case Operation.Backup:
                            byPref = prefs.Platform.Backup.Ask;
                            break;
                        case Operation.Restore:
                            byPref = prefs.Platform.Restore.Ask;
                            break;
                    }
                    break;
            }

            switch (timing)
            {
                case OperationTiming.Free:
                    return true;
                case OperationTiming.BeforePlay:
                    return byPref;
                case OperationTiming.DuringPlay:
                    return false;
                case OperationTiming.AfterPlay:
                    return byPref;
                default:
                    return false;
            }
        }
    }

    public struct OperationResult
    {
        public string Name;
        public ApiResponse Response;
    }

    public enum BackupCriteria
    {
        Game,
        Platform,
    }

    public static class BackupCriteriaExt
    {
        public static bool ByGame(this BackupCriteria criteria)
        {
            return criteria == BackupCriteria.Game;
        }

        public static bool ByPlatform(this BackupCriteria criteria)
        {
            return criteria == BackupCriteria.Platform;
        }
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

    public class LudusaviVersion
    {
        public Version version;

        public LudusaviVersion(Version version)
        {
            this.version = version;
        }

        public bool supportsMultiBackup()
        {
            return this.version >= new Version(0, 12, 0);
        }

        public bool supportsRestoreBySteamId()
        {
            // This version fixed a defect when restoring by Steam ID.
            return this.version >= new Version(0, 12, 0);
        }

        public bool supportsFindCommand()
        {
            return this.version >= new Version(0, 14, 0);
        }

        public bool supportsApiCommand()
        {
            return this.version >= new Version(0, 24, 0);
        }

        public bool supportsCustomizingBackupFormat()
        {
            return this.version >= new Version(0, 14, 0);
        }

        public bool supportsManifestShow()
        {
            return this.version >= new Version(0, 16, 0);
        }

        public bool requiresMergeFlag()
        {
            return this.version < new Version(0, 18, 0);
        }

        public bool hasGlobalManifestUpdateFlag()
        {
            return this.version >= new Version(0, 18, 0);
        }
    }

    public struct ApiEmptyConcern
    { }

    public struct ApiErrors
    {
        [JsonProperty("someGamesFailed")]
        public bool SomeGamesFailed;
        [JsonProperty("unknownGames")]
        public List<string> UnknownGames;
        [JsonProperty("cloudConflict")]
        public ApiEmptyConcern? CloudConflict;
        [JsonProperty("cloudSyncFailed")]
        public ApiEmptyConcern? CloudSyncFailed;
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
        [JsonProperty("changedGames")]
        public ApiChangeCount? ChangedGames;
    }

    public struct ApiChangeCount
    {
        [JsonProperty("new")]
        public int New;
        [JsonProperty("different")]
        public ulong Different;
        [JsonProperty("same")]
        public int Same;
    }

    public struct ApiFile
    {
        [JsonProperty("failed")]
        public bool Failed;
        [JsonProperty("bytes")]
        public ulong Bytes;
        [JsonProperty("change")]
        public string Change;
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
        [JsonProperty("comment")]
        public string Comment;
        [JsonProperty("os")]
        public string Os;
    }

    public struct ApiGame
    {
        [JsonProperty("decision")]
        public string Decision;
        [JsonProperty("change")]
        public string Change;
        [JsonProperty("files")]
        public Dictionary<string, ApiFile> Files;
        [JsonProperty("registry")]
        public Dictionary<string, ApiRegistry> Registry;
        [JsonProperty("backups")]
        public List<ApiBackup> Backups;
        [JsonProperty("backupPath")]
        public string BackupPath;
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

    public class Manifest : Dictionary<string, ManifestGame>
    { }

    public struct ManifestGame
    {
        [JsonProperty("files")]
        public Dictionary<string, object> Files;
        [JsonProperty("registry")]
        public Dictionary<string, object> Registry;
        [JsonProperty("steam")]
        public ManifestSteam Steam;
    }

    public class ManifestSteam
    {
        [JsonProperty("id")]
        public int? Id;
    }
}
