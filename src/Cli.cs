using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Playnite.SDK;
using Playnite.SDK.Models;
using System.Linq;
using System.Diagnostics;

namespace LudusaviPlaynite.Cli
{
    /// <summary>
    /// Manages Ludusavi CLI state and invocations.
    /// </summary>
    public class App
    {
        private ILogger logger { get; set; }
        private LudusaviPlayniteSettings settings { get; set; }

        public LudusaviVersion version { get; private set; } = new LudusaviVersion(new Version(0, 0, 0));
        public Dictionary<string, string> titles { get; private set; } = new Dictionary<string, string>();
        public Dictionary<string, List<Output.Backup>> backups { get; private set; } = new Dictionary<string, List<Output.Backup>>();
        public Dictionary<string, string> backupPaths { get; private set; } = new Dictionary<string, string>();
        public List<string> manifestGames { get; private set; } = new List<string>();
        public List<string> manifestGamesWithSaveDataByTitle { get; private set; } = new List<string>();
        public List<int> manifestGamesWithSaveDataBySteamId { get; private set; } = new List<int>();

        public App(ILogger logger, LudusaviPlayniteSettings settings)
        {
            this.logger = logger;
            this.settings = settings;
        }

        public bool IsValid()
        {
            return version.inner != new Version(0, 0, 0);
        }

        public void Launch()
        {
            var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = settings.ExecutablePath.Trim();
            p.Start();
        }

        private (int, string) InvokeDirect(Invocation invocation, bool standalone = false)
        {
            var fullArgs = invocation.Render(settings, version);
            logger.Debug(string.Format("Running Ludusavi: {0}", fullArgs));

            try
            {
                var (code, stdout) = Etc.RunCommand(settings.ExecutablePath.Trim(), fullArgs);
                if (standalone)
                {
                    logger.Debug(string.Format("Ludusavi exited with {0}", code));
                }
                return (code, stdout);
            }
            catch (Exception e)
            {
                logger.Debug(e, "Ludusavi could not be executed");
                return (-1, null);
            }
        }

        private bool InvokeDirectGui(Invocation invocation, bool standalone = false)
        {
            var fullArgs = invocation.Render(settings, version);
            logger.Debug(string.Format("Running Ludusavi: {0}", fullArgs));

            try
            {
                Etc.RunCommandGui(settings.ExecutablePath.Trim(), fullArgs);
                return true;
            }
            catch (Exception e)
            {
                logger.Debug(e, "Ludusavi could not be executed");
                return false;
            }
        }

        public (int, Output.Response?) Invoke(Invocation invocation)
        {
            var (code, stdout) = InvokeDirect(invocation);

            Output.Response? response;
            try
            {
                response = JsonConvert.DeserializeObject<Output.Response>(stdout);
                logger.Debug(string.Format("Ludusavi exited with {0} and valid JSON content", code));
            }
            catch (Exception e)
            {
                response = null;
                logger.Debug(e, string.Format("Ludusavi exited with {0} and invalid JSON content", code));
            }

            return (code, response);
        }

        public Version GetVersion()
        {
            int code;
            string stdout;
            try
            {
                (code, stdout) = Etc.RunCommand(settings.ExecutablePath.Trim(), "--version");
                var version = stdout.Trim().Split(' ').Last();
                return new Version(version);
            }
            catch (Exception e)
            {
                logger.Debug(e, "Could not determine Ludusavi version");
                return new Version(0, 0, 0);
            }
        }

        public void RefreshVersion()
        {
            this.version = new LudusaviVersion(GetVersion());
        }

        public bool RefreshBackups()
        {
            if (!(this.version.supportsMultiBackup()))
            {
                return false;
            }

            this.backups.Clear();
            this.backupPaths.Clear();

            var (code, response) = Invoke(new Invocation(Mode.Backups).PathIf(settings.BackupPath, settings.OverrideBackupPath));
            if (response?.Games != null)
            {
                foreach (var pair in response?.Games)
                {
                    this.backups[pair.Key] = pair.Value.Backups;
                    if (!string.IsNullOrEmpty(pair.Value.BackupPath))
                    {
                        this.backupPaths[pair.Key] = pair.Value.BackupPath;
                    }
                }
            }

            return true;
        }

        public void RefreshTitles(List<Game> games)
        {
            this.titles.Clear();

            if (!(this.version.supportsApiCommand()))
            {
                return;
            }

            var runner = new Api.Runner(logger, this.settings);
            foreach (var game in games)
            {
                runner.FindTitle(game);
            }

            var (code, output) = runner.Invoke();

            var i = 0;
            if (output?.responses != null)
            {
                foreach (var response in output?.responses)
                {
                    if (response.findTitle?.titles.Count() == 1)
                    {
                        this.titles[Etc.GetTitleId(games[i])] = response.findTitle?.titles[0];
                    }

                    i += 1;
                }
            }
        }

        public void RefreshGames()
        {
            if (!this.version.supportsManifestShow())
            {
                return;
            }

            var (code, stdout) = InvokeDirect(new Invocation(Mode.ManifestShow), true);
            if (code == 0 && stdout != null)
            {
                var manifest = JsonConvert.DeserializeObject<Manifest.Data>(stdout);
                this.manifestGames = manifest.Keys.ToList();

                this.manifestGamesWithSaveDataByTitle = new List<string>();
                this.manifestGamesWithSaveDataBySteamId = new List<int>();
                foreach (var game in manifest)
                {
                    var files = game.Value.Files != null && game.Value.Files.Count > 0;
                    var registry = game.Value.Registry != null && game.Value.Registry.Count > 0;
                    var steamId = game.Value.Steam?.Id ?? 0;
                    if (files || registry)
                    {
                        this.manifestGamesWithSaveDataByTitle.Add(game.Key);
                        if (steamId != 0)
                        {
                            this.manifestGamesWithSaveDataBySteamId.Add(steamId);
                        }
                    }
                }
            }
        }

        public Api.Responses.AppUpdate? CheckAppUpdate()
        {
            if (!(this.version.supportsApiRequestCheckAppUpdate()))
            {
                return null;
            }

            var runner = new Api.Runner(logger, this.settings);
            runner.CheckAppUpdate();

            var (code, output) = runner.Invoke();

            if (output?.responses != null)
            {
                foreach (var response in output?.responses)
                {
                    var info = response.checkAppUpdate;
                    if (info != null)
                    {
                        return info?.update;
                    }
                }
            }

            return null;
        }

        public bool EditBackup(string game, bool? locked, string comment)
        {
            if (!(this.version.supportsEditBackup()))
            {
                return false;
            }

            var runner = new Api.Runner(logger, this.settings);
            runner.EditBackup(game, locked, comment);

            var (code, output) = runner.Invoke();

            if (output?.responses != null)
            {
                foreach (var response in output?.responses)
                {
                    var info = response.editBackup;
                    if (info != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool OpenCustomGame(string name)
        {
            if (!this.version.supportsGuiCommand())
            {
                return false;
            }

            var invocation = new Cli.Invocation(Mode.Gui);
            invocation.Raw();
            invocation.CustomGame(name);

            return InvokeDirectGui(invocation, true);
        }
    }

    public class LudusaviVersion
    {
        public Version inner;

        public LudusaviVersion(Version version)
        {
            this.inner = version;
        }

        public bool supportsMultiBackup()
        {
            return this.inner >= new Version(0, 12, 0);
        }

        public bool supportsRestoreBySteamId()
        {
            // This version fixed a defect when restoring by Steam ID.
            return this.inner >= new Version(0, 12, 0);
        }

        public bool supportsFindCommand()
        {
            return this.inner >= new Version(0, 14, 0);
        }

        public bool supportsApiCommand()
        {
            return this.inner >= new Version(0, 24, 0);
        }

        public bool supportsApiRequestCheckAppUpdate()
        {
            return this.inner >= new Version(0, 25, 0);
        }

        public bool supportsCustomizingBackupFormat()
        {
            return this.inner >= new Version(0, 14, 0);
        }

        public bool supportsManifestShow()
        {
            return this.inner >= new Version(0, 16, 0);
        }

        public bool requiresMergeFlag()
        {
            return this.inner < new Version(0, 18, 0);
        }

        public bool hasGlobalManifestUpdateFlag()
        {
            return this.inner >= new Version(0, 18, 0);
        }

        public bool supportsGuiCommand()
        {
            return this.inner >= new Version(0, 30, 0);
        }

        public bool supportsEditBackup()
        {
            return this.inner >= new Version(0, 30, 0);
        }
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
        private string customGame;
        private bool raw;

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

        public Invocation CustomGame(string name)
        {
            this.customGame = name;
            return this;
        }

        public Invocation Raw()
        {
            this.raw = true;
            return this;
        }

        private string Quote(string text)
        {
            return string.Format("\"{0}\"", text);
        }

        public string Render(LudusaviPlayniteSettings settings, Cli.LudusaviVersion version)
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
                case Mode.Gui:
                    parts.Add("gui");
                    break;
            }

            if (!this.raw)
            {
                parts.Add("--api");
            }

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

            if (this.customGame != null)
            {
                parts.Add("--custom-game");
                parts.Add(Quote(this.customGame));
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
}

namespace LudusaviPlaynite.Cli.Output
{
    public struct EmptyConcern
    { }

    public struct Errors
    {
        [JsonProperty("someGamesFailed")]
        public bool SomeGamesFailed;
        [JsonProperty("unknownGames")]
        public List<string> UnknownGames;
        [JsonProperty("cloudConflict")]
        public EmptyConcern? CloudConflict;
        [JsonProperty("cloudSyncFailed")]
        public EmptyConcern? CloudSyncFailed;
    }

    public struct Overall
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
        public ChangeCount? ChangedGames;
    }

    public struct ChangeCount
    {
        [JsonProperty("new")]
        public int New;
        [JsonProperty("different")]
        public ulong Different;
        [JsonProperty("same")]
        public int Same;
    }

    public struct File
    {
        [JsonProperty("failed")]
        public bool Failed;
        [JsonProperty("bytes")]
        public ulong Bytes;
        [JsonProperty("change")]
        public string Change;
    }

    public struct Registry
    {
        [JsonProperty("failed")]
        public bool Failed;
    }

    public struct Backup
    {
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("when")]
        public DateTime When;
        [JsonProperty("comment")]
        public string Comment;
        [JsonProperty("os")]
        public string Os;
        [JsonProperty("locked")]
        public bool Locked;
    }

    public struct Game
    {
        [JsonProperty("decision")]
        public string Decision;
        [JsonProperty("change")]
        public string Change;
        [JsonProperty("files")]
        public Dictionary<string, File> Files;
        [JsonProperty("registry")]
        public Dictionary<string, Registry> Registry;
        [JsonProperty("backups")]
        public List<Backup> Backups;
        [JsonProperty("backupPath")]
        public string BackupPath;
    }

    public struct Response
    {
        [JsonProperty("errors")]
        public Errors Errors;
        [JsonProperty("overall")]
        public Overall Overall;
        [JsonProperty("games")]
        public Dictionary<string, Game> Games;
    }
}
