using ByteSizeLib;
using Newtonsoft.Json;
using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using Playnite.SDK.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LudusaviPlaynite
{
    public class LudusaviPlaynite : GenericPlugin
    {
        private readonly Version RECOMMENDED_APP_VERSION = new Version(0, 24, 0);

        private const string TAG_PREFIX = "[Ludusavi] ";

        private const string LEGACY_TAG_SKIP = "ludusavi-skip";
        private const string TAG_SKIP = TAG_PREFIX + "Skip";

        private const string TAG_GAME_BACKUP = TAG_PREFIX + "Game: backup";
        private const string TAG_GAME_NO_BACKUP = TAG_PREFIX + "Game: no backup";

        private const string TAG_GAME_BACKUP_AND_RESTORE = TAG_PREFIX + "Game: backup and restore";
        private const string TAG_GAME_NO_RESTORE = TAG_PREFIX + "Game: no restore";

        private const string TAG_PLATFORM_BACKUP = TAG_PREFIX + "Platform: backup";
        private const string TAG_PLATFORM_NO_BACKUP = TAG_PREFIX + "Platform: no backup";

        private const string TAG_PLATFORM_BACKUP_AND_RESTORE = TAG_PREFIX + "Platform: backup and restore";
        private const string TAG_PLATFORM_NO_RESTORE = TAG_PREFIX + "Platform: no restore";

        public const string TAG_BACKED_UP = TAG_PREFIX + "Backed up";
        public const string TAG_UNKNOWN_SAVE_DATA = TAG_PREFIX + "Unknown save data";

        // Format: {new tag, {conflicting tags}}
        private readonly Dictionary<string, string[]> TAGS_AND_CONFLICTS = new Dictionary<string, string[]> {
            {TAG_SKIP, new string[] {}},
            {TAG_GAME_BACKUP, new string[] {TAG_SKIP, TAG_GAME_NO_BACKUP}},
            {TAG_GAME_NO_BACKUP, new string[] {TAG_GAME_BACKUP, TAG_GAME_BACKUP_AND_RESTORE}},
            {TAG_GAME_BACKUP_AND_RESTORE, new string[] {TAG_SKIP, TAG_GAME_BACKUP, TAG_GAME_NO_BACKUP, TAG_GAME_NO_RESTORE}},
            {TAG_GAME_NO_RESTORE, new string[] {TAG_GAME_BACKUP_AND_RESTORE}},
            {TAG_PLATFORM_BACKUP, new string[] {TAG_SKIP, TAG_PLATFORM_NO_BACKUP}},
            {TAG_PLATFORM_NO_BACKUP, new string[] {TAG_PLATFORM_BACKUP, TAG_PLATFORM_BACKUP_AND_RESTORE}},
            {TAG_PLATFORM_BACKUP_AND_RESTORE, new string[] {TAG_SKIP, TAG_PLATFORM_BACKUP, TAG_PLATFORM_NO_BACKUP, TAG_PLATFORM_NO_RESTORE}},
            {TAG_PLATFORM_NO_RESTORE, new string[] {TAG_PLATFORM_BACKUP_AND_RESTORE}},
        };
        // Format: {(new tag, conflicting tag), conflict replacement}
        private readonly Dictionary<(string, string), string> TAG_REPLACEMENTS = new Dictionary<(string, string), string> {
            {(TAG_GAME_NO_RESTORE, TAG_GAME_BACKUP_AND_RESTORE), TAG_GAME_BACKUP},
            {(TAG_PLATFORM_NO_RESTORE, TAG_PLATFORM_BACKUP_AND_RESTORE), TAG_PLATFORM_BACKUP},
        };

        private static readonly ILogger logger = LogManager.GetLogger();
        public LudusaviPlayniteSettings settings { get; set; }
        public override Guid Id { get; } = Guid.Parse("72e2de43-d859-44d8-914e-4277741c8208");

        private Translator translator;
        private bool pendingOperation { get; set; }
        private bool playedSomething { get; set; }
        private Game lastGamePlayed { get; set; }
        private LudusaviVersion appVersion { get; set; } = new LudusaviVersion(new Version(0, 0, 0));
        private Dictionary<string, string> titles { get; set; } = new Dictionary<string, string>();
        private Dictionary<string, List<ApiBackup>> backups { get; set; } = new Dictionary<string, List<ApiBackup>>();
        private List<string> manifestGames { get; set; } = new List<string>();
        private List<string> manifestGamesWithSaveDataByTitle { get; set; } = new List<string>();
        private List<int> manifestGamesWithSaveDataBySteamId { get; set; } = new List<int>();
        private Timer duringPlayBackupTimer { get; set; }
        private int duringPlayBackupTotal { get; set; }
        private int duringPlayBackupFailed { get; set; }

        public LudusaviPlaynite(IPlayniteAPI api) : base(api)
        {
            translator = new Translator(PlayniteApi.ApplicationSettings.Language);
            settings = new LudusaviPlayniteSettings(this, translator);
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
        }

        public override IEnumerable<MainMenuItem> GetMainMenuItems(GetMainMenuItemsArgs menuArgs)
        {
            return new List<MainMenuItem>
            {
                new MainMenuItem
                {
                    Description = translator.Launch_Label(),
                    MenuSection = "@" + translator.Ludusavi(),
                    Action = args => {
                        LaunchLudusavi();
                    }
                },
                new MainMenuItem
                {
                    Description = translator.BackUpLastGame_Label(),
                    MenuSection = "@" + translator.Ludusavi(),
                    Action = async args => {
                        await InitiateOperation(null, Operation.Backup, OperationTiming.Free, BackupCriteria.Game);
                    }
                },
                new MainMenuItem
                {
                    Description = translator.BackUpAllGames_Label(),
                    MenuSection = "@" + translator.Ludusavi(),
                    Action = async args => {
                        if (!CanPerformOperation())
                        {
                            return;
                        }
                        if (UserConsents(translator.BackUpAllGames_Confirm()))
                        {
                            await Task.Run(() => BackUpAllGames());
                        }
                    }
                },
                new MainMenuItem
                {
                    Description = translator.RestoreLastGame_Label(),
                    MenuSection = "@" + translator.Ludusavi(),
                    Action = async args => {
                        await InitiateOperation(null, Operation.Restore, OperationTiming.Free, BackupCriteria.Game);
                    }
                },
                new MainMenuItem
                {
                    Description = translator.RestoreAllGames_Label(),
                    MenuSection = "@" + translator.Ludusavi(),
                    Action = async args => {
                        if (!CanPerformOperation())
                        {
                            return;
                        }
                        if (UserConsents(translator.RestoreAllGames_Confirm()))
                        {
                            await Task.Run(() => RestoreAllGames());
                        }
                    }
                },
            };
        }

        public override IEnumerable<GameMenuItem> GetGameMenuItems(GetGameMenuItemsArgs menuArgs)
        {
            var items = new List<GameMenuItem>
            {
                new GameMenuItem
                {
                    Description = translator.BackUpSelectedGames_Label(),
                    MenuSection = translator.Ludusavi(),
                    Action = async args => {
                        if (args.Games.Count == 1)
                        {
                            await InitiateOperation(args.Games[0], Operation.Backup, OperationTiming.Free, BackupCriteria.Game);
                        }
                        else
                        {
                            if (!CanPerformOperation())
                            {
                                return;
                            }
                            if (UserConsents(translator.BackUpSelectedGames_Confirm(args.Games.Select(x => GetGameName(x)).ToList())))
                            {
                                foreach (var game in args.Games)
                                {
                                    {
                                        await Task.Run(() => BackUpOneGame(game, OperationTiming.Free, BackupCriteria.Game));
                                    }
                                }
                            }
                        }
                    }
                },
            };

            if (menuArgs.Games.Count == 1 && IsBackedUp(menuArgs.Games[0]))
            {
                var game = menuArgs.Games[0];
                foreach (var backup in GetBackups(game))
                {
                    items.Add(
                        new GameMenuItem
                        {
                            Description = GetBackupDisplayLine(backup),
                            MenuSection = string.Format("{0} | {1}", translator.Ludusavi(), translator.RestoreSelectedGames_Label()),
                            Action = async args =>
                            {
                                await InitiateOperation(game, Operation.Restore, OperationTiming.Free, BackupCriteria.Game, backup);
                            }
                        }
                    );
                }
            }
            else
            {
                items.Add(
                    new GameMenuItem
                    {
                        Description = translator.RestoreSelectedGames_Label(),
                        MenuSection = translator.Ludusavi(),
                        Action = async args =>
                        {
                            if (!CanPerformOperation())
                            {
                                return;
                            }
                            if (UserConsents(translator.RestoreSelectedGames_Confirm(args.Games.Select(x => GetGameName(x)).ToList())))
                            {
                                foreach (var game in args.Games)
                                {
                                    {
                                        await Task.Run(() => RestoreOneGame(game, null, BackupCriteria.Game));
                                    }
                                }
                            }
                        }
                    }
                );
            }

            if (menuArgs.Games.Count == 1)
            {
                var title = menuArgs.Games[0].Name;
                string renamed = GetDictValue(settings.AlternativeTitles, title, null);

                items.Add(
                    new GameMenuItem
                    {
                        Description = translator.LookUpAsOtherTitle(renamed),
                        MenuSection = translator.Ludusavi(),
                        Action = args =>
                        {
                            GenericItemOption result = null;

                            if (this.appVersion.supportsManifestShow() && this.manifestGames.Count > 0)
                            {
                                var options = this.manifestGames.Select(x => new GenericItemOption(x, "")).ToList();
                                result = PlayniteApi.Dialogs.ChooseItemWithSearch(
                                    new List<GenericItemOption>(options),
                                    (query) =>
                                    {
                                        if (query == null)
                                        {
                                            return options;
                                        }
                                        else
                                        {
                                            return this.manifestGames
                                                .Where(x => x.ToLower().Contains(query.ToLower()))
                                                .Select(x => new GenericItemOption(x, "")).ToList();
                                        }
                                    }
                                );
                            }
                            else
                            {
                                var input = PlayniteApi.Dialogs.SelectString(translator.LookUpAsOtherTitle(null), "", "");
                                if (!string.IsNullOrEmpty(input?.SelectedString?.Trim()))
                                {
                                    result = new GenericItemOption(input.SelectedString.Trim(), "");
                                }
                            }

                            if (result != null)
                            {
                                settings.AlternativeTitles[title] = result.Name;
                                SavePluginSettings(settings);
                                RefreshLudusaviTitles();
                                RefreshLudusaviBackups();
                            }
                        }
                    }
                );

                if (settings.AlternativeTitles.ContainsKey(menuArgs.Games[0].Name))
                {
                    items.Add(
                        new GameMenuItem
                        {
                            Description = translator.LookUpAsNormalTitle(),
                            MenuSection = translator.Ludusavi(),
                            Action = args =>
                            {
                                settings.AlternativeTitles.Remove(title);
                                SavePluginSettings(settings);
                                RefreshLudusaviTitles();
                                RefreshLudusaviBackups();
                            }
                        }
                    );
                }
            }

            foreach (var entry in TAGS_AND_CONFLICTS)
            {
                var candidate = entry.Key;
                var conflicts = entry.Value;

                if (menuArgs.Games.Any(x => !HasTag(x, candidate)))
                {
                    items.Add(
                        new GameMenuItem
                        {
                            Description = translator.AddTagForSelectedGames_Label(candidate),
                            MenuSection = translator.Ludusavi(),
                            Action = async args =>
                            {
                                if (UserConsents(translator.AddTagForSelectedGames_Confirm(candidate, args.Games.Select(x => x.Name))))
                                {
                                    foreach (var game in args.Games)
                                    {
                                        {
                                            await Task.Run(() =>
                                            {
                                                AddTag(game, candidate);
                                                foreach (var conflict in conflicts)
                                                {
                                                    var removed = RemoveTag(game, conflict);
                                                    string replacement;
                                                    if (removed && TAG_REPLACEMENTS.TryGetValue((candidate, conflict), out replacement))
                                                    {
                                                        AddTag(game, replacement);
                                                    }
                                                }
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    );
                }

                if (menuArgs.Games.Any(x => HasTag(x, candidate)))
                {
                    items.Add(
                        new GameMenuItem
                        {
                            Description = translator.RemoveTagForSelectedGames_Label(candidate),
                            MenuSection = translator.Ludusavi(),
                            Action = async args =>
                            {
                                if (UserConsents(translator.RemoveTagForSelectedGames_Confirm(candidate, args.Games.Select(x => x.Name))))
                                {
                                    foreach (var game in args.Games)
                                    {
                                        {
                                            await Task.Run(() =>
                                            {
                                                RemoveTag(game, candidate);
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    );
                }
            }

            return items;
        }

        public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
        {
            if (!settings.MigratedTags)
            {
                var oldTag = PlayniteApi.Database.Tags.FirstOrDefault(x => x.Name == LEGACY_TAG_SKIP);
                var newTagExists = PlayniteApi.Database.Tags.Any(x => x.Name == TAG_SKIP);
                if (oldTag != null && !newTagExists)
                {
                    oldTag.Name = TAG_SKIP;
                    PlayniteApi.Database.Tags.Update(oldTag);
                }
                settings.MigratedTags = true;
                SavePluginSettings(settings);
            }

            Task.Run(() =>
            {
                RefreshLudusaviVersion();
                RefreshLudusaviTitles();
                RefreshLudusaviBackups();
                RefreshLudusaviGames();

                if (appVersion.version < RECOMMENDED_APP_VERSION && new Version(settings.SuggestedUpgradeTo) < RECOMMENDED_APP_VERSION)
                {
                    NotifyInfo(
                        translator.UpgradePrompt(RECOMMENDED_APP_VERSION.ToString()),
                        () =>
                        {
                            try
                            {
                                RunCommand("cmd.exe", "/c \"start https://github.com/mtkennerly/ludusavi/releases\"");
                            }
                            catch
                            { }
                        }
                    );
                    settings.SuggestedUpgradeTo = RECOMMENDED_APP_VERSION.ToString();
                    SavePluginSettings(settings);
                }
            });
        }

        public override void OnGameStarting(OnGameStartingEventArgs args)
        {
            playedSomething = true;
            lastGamePlayed = args.Game;
            Game game = args.Game;
            var prefs = GetPlayPreferences(game);

            if (prefs.Game.Restore.Do)
            {
                InitiateOperationSync(game, Operation.Restore, OperationTiming.BeforePlay, BackupCriteria.Game);
            }

            if (prefs.Platform.Restore.Do)
            {
                InitiateOperationSync(game, Operation.Restore, OperationTiming.BeforePlay, BackupCriteria.Platform);
            }

            if (settings.DoBackupDuringPlay)
            {
                this.duringPlayBackupTotal = 0;
                this.duringPlayBackupFailed = 0;
                this.duringPlayBackupTimer = new Timer(
                    x => BackUpOneGameDuringPlay((Game)x),
                    game,
                    TimeSpan.FromMinutes(settings.BackupDuringPlayInterval),
                    TimeSpan.FromMinutes(settings.BackupDuringPlayInterval)
                );
            }
        }

        public override void OnGameStopped(OnGameStoppedEventArgs arg)
        {
            playedSomething = true;
            lastGamePlayed = arg.Game;
            Game game = arg.Game;
            var prefs = GetPlayPreferences(game);

            if (this.duringPlayBackupTimer != null)
            {
                this.duringPlayBackupTimer.Change(Timeout.Infinite, Timeout.Infinite);
                if (this.duringPlayBackupFailed == 0)
                {
                    NotifyInfo(translator.BackUpDuringPlay_Success(game.Name, this.duringPlayBackupTotal));
                }
                else
                {
                    NotifyError(translator.BackUpDuringPlay_Failure(game.Name, this.duringPlayBackupTotal, this.duringPlayBackupFailed));
                }
            }

            Task.Run(async () =>
            {
                if (prefs.Game.Backup.Do)
                {
                    await InitiateOperation(game, Operation.Backup, OperationTiming.AfterPlay, BackupCriteria.Game);
                }

                if (prefs.Platform.Backup.Do)
                {
                    await InitiateOperation(game, Operation.Backup, OperationTiming.AfterPlay, BackupCriteria.Platform);
                }
            });
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new LudusaviPlayniteSettingsView(this, this.translator);
        }

        public void RefreshLudusaviVersion()
        {
            this.appVersion = new LudusaviVersion(GetLudusaviVersion());
        }

        public void RefreshLudusaviBackups()
        {
            if (!(this.appVersion.supportsMultiBackup()))
            {
                return;
            }

            var (code, response) = InvokeLudusavi(new Invocation(Mode.Backups).PathIf(settings.BackupPath, settings.OverrideBackupPath));
            if (response?.Games != null)
            {
                this.backups = response?.Games.ToDictionary(pair => pair.Key, pair => pair.Value.Backups);
            }

            if (this.settings.TagGamesWithBackups)
            {
                TagGamesWithBackups();
            }
        }

        public void RefreshLudusaviTitles()
        {
            if (!(this.appVersion.supportsApiCommand()))
            {
                return;
            }

            var runner = new Api.Runner(LudusaviPlaynite.logger, this.settings);
            var games = this.PlayniteApi.Database.Games.ToList();
            foreach (var game in games)
            {
                runner.FindTitle(game, GetGameNameWithAlt(game), AlternativeTitle(game) != null);
            }

            var (code, output) = runner.Invoke();

            var i = 0;
            if (output?.responses != null)
            {
                foreach (var response in output?.responses)
                {
                    if (response.findTitle?.titles.Count() == 1)
                    {
                        this.titles.Add(GetTitleId(games[i]), response.findTitle?.titles[0]);
                    }

                    i += 1;
                }
            }
        }

        public void RefreshLudusaviGames()
        {
            if (!this.appVersion.supportsManifestShow())
            {
                return;
            }

            var (code, stdout) = InvokeLudusaviDirect(new Invocation(Mode.ManifestShow), true);
            if (code == 0 && stdout != null)
            {
                var manifest = JsonConvert.DeserializeObject<Manifest>(stdout);
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

                if (this.settings.TagGamesWithUnknownSaveData)
                {
                    TagGamesWithUnknownSaveData();
                }
            }
        }

        private void NotifyInfo(string message)
        {
            NotifyInfo(message, () => { });
        }

        private void NotifyInfo(string message, Action action)
        {
            if (settings.IgnoreBenignNotifications)
            {
                return;
            }
            PlayniteApi.Notifications.Add(new NotificationMessage(Guid.NewGuid().ToString(), message, NotificationType.Info, action));
        }

        private void NotifyInfo(string message, OperationTiming timing)
        {
            if (timing == OperationTiming.DuringPlay)
            {
                this.duringPlayBackupTotal += 1;
            }
            else
            {
                NotifyInfo(message);
            }
        }

        private void NotifyError(string message)
        {
            NotifyError(message, () => { });
        }

        private void NotifyError(string message, Action action)
        {
            PlayniteApi.Notifications.Add(new NotificationMessage(Guid.NewGuid().ToString(), message, NotificationType.Error, action));
        }

        private void NotifyError(string message, OperationTiming timing)
        {
            if (timing == OperationTiming.DuringPlay)
            {
                this.duringPlayBackupTotal += 1;
                this.duringPlayBackupFailed += 1;
            }
            else
            {
                NotifyError(message);
            }
        }

        public void ShowError(string message)
        {
            PlayniteApi.Dialogs.ShowErrorMessage(message, translator.Ludusavi());
        }

        public void NotifyResponseErrors(ApiResponse? response)
        {
            if (response?.Errors.CloudSyncFailed != null)
            {
                var prefix = translator.Ludusavi();
                var error = translator.CloudSyncFailed();
                NotifyError($"{prefix}: {error}");
            }
            if (response?.Errors.CloudConflict != null)
            {
                var prefix = translator.Ludusavi();
                var error = translator.CloudConflict();
                NotifyError($"{prefix}: {error}", () => LaunchLudusavi());
            }
        }

        private void ShowFullResults(ApiResponse response)
        {
            var tempFile = Path.GetTempPath() + Guid.NewGuid().ToString() + ".html";
            using (StreamWriter sw = File.CreateText(tempFile))
            {
                sw.WriteLine("<html><head><style>body { background-color: black; color: white; font-family: sans-serif; }</style></head><body><ul>");
                foreach (var game in response.Games)
                {
                    sw.WriteLine(string.Format("<li>{0}</li>", translator.FullListGameLineItem(game.Key, game.Value)));
                }
                sw.WriteLine("</ul></body></html>");
            }

            var webview = PlayniteApi.WebViews.CreateView(640, 480);
            webview.Navigate(tempFile);
            webview.OpenDialog();

            try
            {
                File.Delete(tempFile);
            }
            catch
            { }
        }

        private (int, string) RunCommand(string command, string args)
        {
            var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = command;
            p.StartInfo.Arguments = args;
            p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            p.Start();

            var stdout = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return (p.ExitCode, stdout);
        }

        private Version GetLudusaviVersion()
        {
            int code;
            string stdout;
            try
            {
                (code, stdout) = RunCommand(settings.ExecutablePath.Trim(), "--version");
                var version = stdout.Trim().Split(' ').Last();
                return new Version(version);
            }
            catch (Exception e)
            {
                logger.Debug(e, "Could not determine Ludusavi version");
                return new Version(0, 0, 0);
            }
        }

        private (int, string) InvokeLudusaviDirect(Invocation invocation, bool standalone = false)
        {
            var fullArgs = invocation.Render(settings, appVersion);
            logger.Debug(string.Format("Running Ludusavi: {0}", fullArgs));

            try
            {
                var (code, stdout) = RunCommand(settings.ExecutablePath.Trim(), fullArgs);
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

        private (int, ApiResponse?) InvokeLudusavi(Invocation invocation)
        {
            var (code, stdout) = InvokeLudusaviDirect(invocation);

            ApiResponse? response;
            try
            {
                response = JsonConvert.DeserializeObject<ApiResponse>(stdout);
                logger.Debug(string.Format("Ludusavi exited with {0} and valid JSON content", code));
            }
            catch (Exception e)
            {
                response = null;
                logger.Debug(e, string.Format("Ludusavi exited with {0} and invalid JSON content", code));
            }

            return (code, response);
        }

        private void LaunchLudusavi()
        {
            var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = settings.ExecutablePath.Trim();
            p.Start();
        }

        private bool CanPerformOperation()
        {
            if (pendingOperation)
            {
                PlayniteApi.Dialogs.ShowMessage(translator.OperationStillPending());
                return false;
            }
            return true;
        }

        private bool CanPerformOperationSuppressed()
        {
            return !pendingOperation;
        }

        private bool CanPerformOperationOnLastGamePlayed()
        {
            if (!playedSomething)
            {
                PlayniteApi.Dialogs.ShowMessage(translator.NoGamePlayedYet());
                return false;
            }
            return CanPerformOperation();
        }

        private bool UserConsents(string message)
        {
            var choice = PlayniteApi.Dialogs.ShowMessage(message, "", System.Windows.MessageBoxButton.YesNo);
            return choice == MessageBoxResult.Yes;
        }

        private Choice AskUser(string message)
        {
            var yes = new MessageBoxOption(translator.YesButton(), true, false);
            var always = new MessageBoxOption(translator.YesRememberedButton(), false, false);
            var no = new MessageBoxOption(translator.NoButton(), false, false);
            var never = new MessageBoxOption(translator.NoRememberedButton(), false, false);

            var choice = PlayniteApi.Dialogs.ShowMessage(
                message,
                "",
                MessageBoxImage.None,
                new List<MessageBoxOption> { always, never, yes, no }
            );

            if (choice == yes)
            {
                return Choice.Yes;
            }
            else if (choice == always)
            {
                return Choice.Always;
            }
            else if (choice == no)
            {
                return Choice.No;
            }
            else if (choice == never)
            {
                return Choice.Never;
            }
            else
            {
                throw new InvalidOperationException(String.Format("AskUser got unexpected answer: {0}", choice.Title));
            }
        }

        private bool ShouldSkipGame(Game game)
        {
            return HasTag(game, TAG_SKIP);
        }

        string GetTitleId(Game game)
        {
            return string.Format("{0}:{1}", game.PluginId, game.GameId);
        }

        string GetTitle(Game game)
        {
            return GetDictValue(this.titles, GetTitleId(game), null);
        }

        string GetGameName(Game game)
        {
            if (!Etc.IsOnPc(game) && settings.AddSuffixForNonPcGameNames)
            {
                return string.Format("{0}{1}", game.Name, settings.SuffixForNonPcGameNames.Replace("<platform>", game.Platforms[0].Name));
            }
            else
            {
                return game.Name;
            }
        }

        string GetGameNameWithAlt(Game game)
        {
            var alt = AlternativeTitle(game);
            if (alt != null)
            {
                return alt;
            }
            else
            {
                return GetGameName(game);
            }
        }

        private string AlternativeTitle(Game game)
        {
            return GetDictValue(settings.AlternativeTitles, game.Name, null);
        }

        private Platform GetGamePlatform(Game game)
        {
            return game?.Platforms?.ElementAtOrDefault(0);
        }

        private string GetDisplayName(Game game, BackupCriteria criteria)
        {
            switch (criteria)
            {
                case BackupCriteria.Game:
                    return GetGameName(game);
                case BackupCriteria.Platform:
                    return GetGamePlatform(game)?.Name ?? "unknown platform";
                default:
                    throw new InvalidOperationException(String.Format("GetDisplayName got unexpected criteria: {0}", criteria));
            }
        }

        private string FindGame(Game game, string name, OperationTiming timing, BackupCriteria criteria, Mode mode)
        {
            if (this.appVersion.supportsApiCommand() && criteria.ByGame())
            {
                var title = GetTitle(game);
                if (title != null)
                {
                    return title;
                }
            }

            if (!this.appVersion.supportsFindCommand())
            {
                return null;
            }

            var invocation = new Invocation(Mode.Find).PathIf(settings.BackupPath, settings.OverrideBackupPath).Game(name);

            if (mode == Mode.Backup)
            {
                invocation.FindBackup();
            }
            if (criteria.ByGame() && AlternativeTitle(game) == null)
            {
                // There can't be an alt title because the Steam ID/etc would take priority over it.

                if (Etc.IsOnSteam(game) && int.TryParse(game.GameId, out var id))
                {
                    invocation.SteamId(id);
                }
                if (!Etc.IsOnPc(game) && settings.RetryNonPcGamesWithoutSuffix)
                {
                    invocation.AddGame(game.Name);
                }
                if (settings.RetryUnrecognizedGameWithNormalization)
                {
                    invocation.Normalized();
                }
            }

            var (code, response) = InvokeLudusavi(invocation);
            if (response == null)
            {
                NotifyError(translator.UnableToRunLudusavi(), timing);
                return null;
            }

            var officialName = response?.Games.Keys.FirstOrDefault();
            if (code != 0 || officialName == null)
            {
                NotifyError(translator.UnrecognizedGame(name), timing);
                return null;
            }

            return officialName;
        }

        private void InitiateOperationSync(Game game, Operation operation, OperationTiming timing, BackupCriteria criteria, ApiBackup? backup = null)
        {
            if (game == null)
            {
                if (!CanPerformOperationOnLastGamePlayed())
                {
                    return;
                }
                game = this.lastGamePlayed;
            }
            else
            {
                if (!CanPerformOperation())
                {
                    return;
                }
            }

            if (criteria.ByPlatform() && GetGamePlatform(game) == null)
            {
                return;
            }

            var prefs = GetPlayPreferences(game);
            var ask = prefs.ShouldAsk(timing, criteria, operation);
            var displayName = GetDisplayName(game, criteria);
            var consented = !ask;

            if (ask)
            {
                if (timing == OperationTiming.Free)
                {
                    switch (operation)
                    {
                        case Operation.Backup:
                            consented = UserConsents(translator.BackUpOneGame_Confirm(displayName));
                            break;
                        case Operation.Restore:
                            consented = UserConsents(translator.RestoreOneGame_Confirm(displayName));
                            break;
                    }
                }
                else
                {
                    var choice = Choice.No;
                    switch (operation)
                    {
                        case Operation.Backup:
                            choice = AskUser(translator.BackUpOneGame_Confirm(displayName));
                            break;
                        case Operation.Restore:
                            choice = AskUser(translator.RestoreOneGame_Confirm(displayName));
                            break;
                    }

                    consented = choice.Accepted();

                    switch (operation)
                    {
                        case Operation.Backup:
                            switch (criteria)
                            {
                                case BackupCriteria.Game:
                                    UpdateTagsForChoice(game, choice, TAG_GAME_BACKUP, TAG_GAME_NO_BACKUP);
                                    break;
                                case BackupCriteria.Platform:
                                    UpdateTagsForChoice(game, choice, TAG_PLATFORM_BACKUP, TAG_PLATFORM_NO_BACKUP);
                                    break;
                            }
                            break;
                        case Operation.Restore:
                            switch (criteria)
                            {
                                case BackupCriteria.Game:
                                    UpdateTagsForChoice(game, choice, TAG_GAME_BACKUP_AND_RESTORE, TAG_GAME_NO_RESTORE, TAG_GAME_BACKUP);
                                    break;
                                case BackupCriteria.Platform:
                                    UpdateTagsForChoice(game, choice, TAG_PLATFORM_BACKUP_AND_RESTORE, TAG_PLATFORM_NO_RESTORE, TAG_PLATFORM_BACKUP);
                                    break;
                            }
                            break;
                    }
                }
            }

            if (!consented)
            {
                return;
            }

            switch (operation)
            {
                case Operation.Backup:
                    BackUpOneGame(game, timing, criteria);
                    break;
                case Operation.Restore:
                    var error = RestoreOneGame(game, backup, criteria);
                    if (timing == OperationTiming.BeforePlay && !String.IsNullOrEmpty(error.Message) && !error.Empty)
                    {
                        ShowError(error.Message);
                    }
                    break;
            }
        }

        private async Task InitiateOperation(Game game, Operation operation, OperationTiming timing, BackupCriteria criteria, ApiBackup? backup = null)
        {
            await Task.Run(() => InitiateOperationSync(game, operation, timing, criteria, backup));
        }

        private void BackUpOneGame(Game game, OperationTiming timing, BackupCriteria criteria)
        {
            pendingOperation = true;
            var name = criteria.ByPlatform() ? game.Platforms[0].Name : GetGameNameWithAlt(game);
            var displayName = game.Name;
            var refresh = true;

            if (this.appVersion.supportsFindCommand())
            {
                var found = FindGame(game, name, timing, criteria, Mode.Backup);
                if (found == null)
                {
                    pendingOperation = false;
                    return;
                }
                name = found;
                if (name != displayName)
                {
                    displayName = $"{displayName} (↪ {name})";
                }
            }

            var invocation = new Invocation(Mode.Backup).PathIf(settings.BackupPath, settings.OverrideBackupPath).Game(name);

            var (code, response) = InvokeLudusavi(invocation);

            if (!this.appVersion.supportsFindCommand() && criteria.ByGame() && AlternativeTitle(game) == null)
            {
                if (response?.Errors.UnknownGames != null && Etc.IsOnSteam(game))
                {
                    (code, response) = InvokeLudusavi(invocation.BySteamId(game.GameId));
                }
                if (response?.Errors.UnknownGames != null && !Etc.IsOnPc(game) && settings.RetryNonPcGamesWithoutSuffix)
                {
                    (code, response) = InvokeLudusavi(invocation.Game(game.Name));
                }
            }

            if (response == null)
            {
                NotifyError(translator.UnableToRunLudusavi(), timing);
            }
            else
            {
                var result = new OperationResult { Name = displayName, Response = (ApiResponse)response };
                if (code == 0)
                {
                    if (response?.Overall.TotalGames > 0)
                    {
                        if ((response?.Overall.ChangedGames?.Same ?? 0) == 0)
                        {
                            NotifyInfo(translator.BackUpOneGame_Success(result), timing);
                        }
                        else
                        {
                            refresh = false;
                            NotifyInfo(translator.BackUpOneGame_Unchanged(result), timing);
                        }
                    }
                    else
                    {
                        refresh = false;
                        NotifyError(translator.BackUpOneGame_Empty(result), timing);
                    }
                }
                else
                {
                    if (response?.Errors.UnknownGames != null)
                    {
                        refresh = false;
                        NotifyError(translator.BackUpOneGame_Empty(result), timing);
                    }
                    else
                    {
                        NotifyError(translator.BackUpOneGame_Failure(result), timing);
                    }
                }
            }

            NotifyResponseErrors(response);
            if (refresh)
            {
                RefreshLudusaviBackups();
            }
            pendingOperation = false;
        }

        private void BackUpAllGames()
        {
            pendingOperation = true;
            var (code, response) = InvokeLudusavi(new Invocation(Mode.Backup).PathIf(settings.BackupPath, settings.OverrideBackupPath));

            if (response == null)
            {
                NotifyError(translator.UnableToRunLudusavi());
            }
            else
            {
                var result = new OperationResult { Response = (ApiResponse)response };

                if (code == 0)
                {
                    NotifyInfo(translator.BackUpAllGames_Success(result), () => ShowFullResults(result.Response));
                }
                else
                {
                    NotifyError(translator.BackUpAllGames_Failure(result), () => ShowFullResults(result.Response));
                }
            }

            NotifyResponseErrors(response);
            RefreshLudusaviBackups();
            pendingOperation = false;
        }

        private void BackUpOneGameDuringPlay(Game game)
        {
            if (!CanPerformOperationSuppressed())
            {
                return;
            }
            var prefs = GetPlayPreferences(game);
            Task.Run(() =>
            {
                if (prefs.Game.Backup.Do && !prefs.Game.Backup.Ask && settings.DoBackupDuringPlay)
                {
                    BackUpOneGame(game, OperationTiming.DuringPlay, BackupCriteria.Game);
                }

                if (prefs.Platform.Backup.Do && !prefs.Platform.Backup.Ask && settings.DoBackupDuringPlay)
                {
                    BackUpOneGame(game, OperationTiming.DuringPlay, BackupCriteria.Platform);
                }
            });
        }

        private RestorationError RestoreOneGame(Game game, ApiBackup? backup, BackupCriteria criteria)
        {
            RestorationError error = new RestorationError
            {
                Message = null,
                Empty = false,
            };

            pendingOperation = true;
            var name = criteria.ByPlatform() ? game.Platforms[0].Name : GetGameNameWithAlt(game);
            var displayName = game.Name;

            if (this.appVersion.supportsFindCommand())
            {
                var found = FindGame(game, name, OperationTiming.Free, criteria, Mode.Restore);
                if (found == null)
                {
                    pendingOperation = false;
                    error.Message = translator.UnrecognizedGame(name);
                    error.Empty = true;
                    return error;
                }
                name = found;
                if (name != displayName)
                {
                    displayName = $"{displayName} (↪ {name})";
                }
            }

            var invocation = new Invocation(Mode.Restore).PathIf(settings.BackupPath, settings.OverrideBackupPath).Game(name).Backup(backup?.Name);

            var (code, response) = InvokeLudusavi(invocation);
            if (!this.appVersion.supportsFindCommand() && criteria.ByGame() && AlternativeTitle(game) == null)
            {
                if (response?.Errors.UnknownGames != null && Etc.IsOnSteam(game) && this.appVersion.supportsRestoreBySteamId())
                {
                    (code, response) = InvokeLudusavi(invocation.BySteamId(game.GameId));
                }
                if (response?.Errors.UnknownGames != null && !Etc.IsOnPc(game) && settings.RetryNonPcGamesWithoutSuffix)
                {
                    (code, response) = InvokeLudusavi(invocation.Game(game.Name));
                }
            }

            if (response == null)
            {
                error.Message = translator.UnableToRunLudusavi();
                NotifyError(error.Message);
            }
            else
            {
                var result = new OperationResult { Name = displayName, Response = (ApiResponse)response };
                if (code == 0)
                {
                    if (response?.Overall.TotalGames == 0)
                    {
                        // This applies to Ludusavi v0.23.0 and later
                        error.Message = translator.RestoreOneGame_Empty(result);
                        error.Empty = true;
                        NotifyError(error.Message);
                    }
                    else if ((response?.Overall.ChangedGames?.Same ?? 0) == 0)
                    {
                        NotifyInfo(translator.RestoreOneGame_Success(result));
                    }
                    else
                    {
                        NotifyInfo(translator.RestoreOneGame_Unchanged(result));
                    }
                }
                else
                {
                    if (response?.Errors.UnknownGames != null)
                    {
                        // This applies to Ludusavi versions before v0.23.0
                        error.Message = translator.RestoreOneGame_Empty(result);
                        error.Empty = true;
                        NotifyError(error.Message);
                    }
                    else
                    {
                        error.Message = translator.RestoreOneGame_Failure(result);
                        NotifyError(error.Message);
                    }
                }
            }

            NotifyResponseErrors(response);
            pendingOperation = false;
            return error;
        }

        private void RestoreAllGames()
        {
            pendingOperation = true;
            var (code, response) = InvokeLudusavi(new Invocation(Mode.Restore).PathIf(settings.BackupPath, settings.OverrideBackupPath));

            if (response == null)
            {
                NotifyError(translator.UnableToRunLudusavi());
            }
            else
            {
                var result = new OperationResult { Response = (ApiResponse)response };

                if (code == 0)
                {
                    NotifyInfo(translator.RestoreAllGames_Success(result), () => ShowFullResults(result.Response));
                }
                else
                {
                    NotifyError(translator.RestoreAllGames_Failure(result), () => ShowFullResults(result.Response));
                }

            }

            NotifyResponseErrors(response);
            pendingOperation = false;
        }

        private bool HasTag(Game game, string tagName)
        {
            return game.Tags?.Any(tag => tag.Name == tagName) ?? false;
        }

        private bool AddTag(Game game, string tagName)
        {
            var dbTag = PlayniteApi.Database.Tags.FirstOrDefault(tag => tag.Name == tagName);
            if (dbTag == null)
            {
                dbTag = PlayniteApi.Database.Tags.Add(tagName);
            }

            var dbGame = PlayniteApi.Database.Games[game.Id];
            if (dbGame.TagIds == null)
            {
                dbGame.TagIds = new List<Guid>();
            }
            var added = dbGame.TagIds.AddMissing(dbTag.Id);
            PlayniteApi.Database.Games.Update(dbGame);
            return added;
        }

        private bool RemoveTag(Game game, string tagName)
        {
            if (game.Tags == null || game.Tags.All(tag => tag.Name != tagName))
            {
                return false;
            }

            var dbTag = PlayniteApi.Database.Tags.FirstOrDefault(tag => tag.Name == tagName);
            if (dbTag == null)
            {
                return false;
            }

            var dbGame = PlayniteApi.Database.Games[game.Id];
            if (dbGame.TagIds == null)
            {
                return false;
            }
            var removed = dbGame.TagIds.RemoveAll(id => id == dbTag.Id);
            PlayniteApi.Database.Games.Update(dbGame);
            return removed > 0;
        }

        private void UpdateTagsForChoice(Game game, Choice choice, string alwaysTag, string neverTag, string fallbackTag = null)
        {
            if (choice == Choice.Always)
            {
                if (fallbackTag != null)
                {
                    RemoveTag(game, fallbackTag);
                }
                AddTag(game, alwaysTag);
            }
            else if (choice == Choice.Never)
            {
                if (fallbackTag != null && HasTag(game, alwaysTag))
                {
                    AddTag(game, fallbackTag);
                }
                RemoveTag(game, alwaysTag);
                AddTag(game, neverTag);
            }
        }

        private void TagGamesWithBackups()
        {
            foreach (var game in PlayniteApi.Database.Games)
            {
                if (IsBackedUp(game))
                {
                    AddTag(game, TAG_BACKED_UP);
                }
                else
                {
                    RemoveTag(game, TAG_BACKED_UP);
                }
            }
        }

        private void TagGamesWithUnknownSaveData()
        {
            foreach (var game in PlayniteApi.Database.Games)
            {
                if (!GameHasKnownSaveData(game))
                {
                    AddTag(game, TAG_UNKNOWN_SAVE_DATA);
                }
                else
                {
                    RemoveTag(game, TAG_UNKNOWN_SAVE_DATA);
                }
            }
        }

        private PlayPreferences GetPlayPreferences(Game game)
        {
            if (ShouldSkipGame(game))
            {
                return new PlayPreferences();
            }

            var gameBackupDo = (settings.DoBackupOnGameStopped || HasTag(game, TAG_GAME_BACKUP) || HasTag(game, TAG_GAME_BACKUP_AND_RESTORE))
                && !HasTag(game, TAG_GAME_NO_BACKUP)
                && (Etc.IsOnPc(game) || !settings.OnlyBackupOnGameStoppedIfPc || HasTag(game, TAG_GAME_BACKUP) || HasTag(game, TAG_GAME_BACKUP_AND_RESTORE));
            var platformBackupDo = (settings.DoPlatformBackupOnNonPcGameStopped || HasTag(game, TAG_PLATFORM_BACKUP) || HasTag(game, TAG_PLATFORM_BACKUP_AND_RESTORE))
                && !HasTag(game, TAG_PLATFORM_NO_BACKUP)
                && !Etc.IsOnPc(game)
                && GetGamePlatform(game) != null;

            var prefs = new PlayPreferences
            {
                Game = new OperationPreferences
                {
                    Backup = new OperationPreference
                    {
                        Do = gameBackupDo,
                        Ask = settings.AskBackupOnGameStopped && !HasTag(game, TAG_GAME_BACKUP) && !HasTag(game, TAG_GAME_BACKUP_AND_RESTORE),
                    },
                    Restore = new OperationPreference
                    {
                        Do = gameBackupDo
                            && (settings.DoRestoreOnGameStarting || HasTag(game, TAG_GAME_BACKUP_AND_RESTORE))
                            && !HasTag(game, TAG_GAME_NO_RESTORE),
                        Ask = settings.AskBackupOnGameStopped && !HasTag(game, TAG_GAME_BACKUP_AND_RESTORE),
                    },
                },
                Platform = new OperationPreferences
                {
                    Backup = new OperationPreference
                    {
                        Do = platformBackupDo,
                        Ask = settings.AskPlatformBackupOnNonPcGameStopped && !HasTag(game, TAG_PLATFORM_BACKUP) && !HasTag(game, TAG_PLATFORM_BACKUP_AND_RESTORE),
                    },
                    Restore = new OperationPreference
                    {
                        Do = platformBackupDo
                            && (settings.DoPlatformRestoreOnNonPcGameStarting || HasTag(game, TAG_PLATFORM_BACKUP_AND_RESTORE))
                            && !HasTag(game, TAG_PLATFORM_NO_RESTORE),
                        Ask = settings.AskPlatformBackupOnNonPcGameStopped && !HasTag(game, TAG_PLATFORM_BACKUP_AND_RESTORE),
                    },
                },
            };

            return prefs;
        }

        private bool IsBackedUp(Game game)
        {
            return GetBackups(game).Count > 0;
        }

        private bool GameHasKnownSaveData(Game game)
        {
            string title;
            if (this.appVersion.supportsApiCommand())
            {
                title = GetTitle(game);
            }
            else
            {
                // Ideally, we would use the `find` command, but that's too slow to run in bulk.
                title = AlternativeTitle(game) ?? GetGameName(game);
            }

            if (title != null && this.manifestGamesWithSaveDataByTitle.Contains(title))
            {
                return true;
            }

            if (Etc.IsOnSteam(game) && int.TryParse(game.GameId, out var id) && this.manifestGamesWithSaveDataBySteamId.Contains(id))
            {
                return true;
            }

            return false;
        }

        private List<ApiBackup> GetBackups(Game game)
        {
            if (this.appVersion.supportsApiCommand())
            {
                var title = GetTitle(game);
                var backups = GetDictValue(this.backups, title, new List<ApiBackup>());

                // Sort newest backups to the top.
                backups.Sort((x, y) => y.When.CompareTo(x.When));

                return backups;
            }

            var ret = new List<ApiBackup>();
            var alt = AlternativeTitle(game);

            if (alt != null)
            {
                ret = GetDictValue(this.backups, alt, new List<ApiBackup>());
            }
            else
            {
                ret = GetDictValue(
                    this.backups,
                    GetGameName(game),
                    GetDictValue(
                        this.backups,
                        game.Name,
                        new List<ApiBackup>()
                    )
                );
            }

            // Sort newest backups to the top.
            ret.Sort((x, y) => y.When.CompareTo(x.When));

            return ret;
        }

        private V GetDictValue<K, V>(Dictionary<K, V> dict, K key, V fallback)
        {
            if (dict == null || key == null)
            {
                return fallback;
            }

            V result;
            var found = dict.TryGetValue(key, out result);
            if (found)
            {
                return result;
            }
            else
            {
                return fallback;
            }
        }

        private string GetBackupDisplayLine(ApiBackup backup)
        {
            var ret = backup.When.ToLocalTime().ToString();

            if (!string.IsNullOrEmpty(backup.Os) && backup.Os != "windows")
            {
                ret += string.Format(" [{0}]", backup.Os);
            }
            if (!string.IsNullOrEmpty(backup.Comment))
            {
                var line = "";
                var parts = backup.Comment.Split();

                foreach (var part in backup.Comment.Split())
                {
                    if (line != "")
                    {
                        line += " ";
                    }
                    line += part;
                    if (line.Length > 60)
                    {
                        ret += string.Format("\n    {0}", line);
                        line = "";
                    }
                }
                if (line != "")
                {
                    ret += string.Format("\n    {0}", line);
                }
            }

            return ret;
        }
    }
}
