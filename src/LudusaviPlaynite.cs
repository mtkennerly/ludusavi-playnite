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
        private readonly Version RECOMMENDED_APP_VERSION = new Version(0, 14, 0);

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
        private Dictionary<string, List<ApiBackup>> backups { get; set; } = new Dictionary<string, List<ApiBackup>>();
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
                        if (!CanPerformOperationOnLastGamePlayed())
                        {
                            return;
                        }
                        if (UserConsents(translator.BackUpOneGame_Confirm(GetGameName(lastGamePlayed), RequiresCustomEntry(lastGamePlayed))))
                        {
                            await Task.Run(() => BackUpOneGame(lastGamePlayed, OperationTiming.Free));
                        }
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
                        if (!CanPerformOperationOnLastGamePlayed())
                        {
                            return;
                        }
                        if (UserConsents(translator.RestoreOneGame_Confirm(GetGameName(lastGamePlayed), RequiresCustomEntry(lastGamePlayed))))
                        {
                            await Task.Run(() => RestoreOneGame(lastGamePlayed));
                        }
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
                        if (!CanPerformOperation())
                        {
                            return;
                        }
                        if (UserConsents(translator.BackUpSelectedGames_Confirm(args.Games.Select(x => (GetGameName(x), RequiresCustomEntry(x))).ToList())))
                        {
                            foreach (var game in args.Games)
                            {
                                {
                                    await Task.Run(() => BackUpOneGame(game, OperationTiming.Free));
                                }
                            }
                        }
                    }
                },
            };

            if (menuArgs.Games.Count == 1 && this.backups.ContainsKey(menuArgs.Games[0].Name))
            {
                var game = menuArgs.Games[0];
                foreach (var backup in this.backups[game.Name])
                {
                    items.Add(
                        new GameMenuItem
                        {
                            Description = backup.When.ToLocalTime().ToString(),
                            MenuSection = string.Format("{0} | {1}", translator.Ludusavi(), translator.RestoreSelectedGames_Label()),
                            Action = async args =>
                            {
                                if (!CanPerformOperation())
                                {
                                    return;
                                }
                                if (UserConsents(translator.RestoreSelectedGames_Confirm(args.Games.Select(x => (GetGameName(x), RequiresCustomEntry(x))).ToList())))
                                {
                                    await Task.Run(() => RestoreOneGame(game, backup.Name));
                                }
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
                            if (UserConsents(translator.RestoreSelectedGames_Confirm(args.Games.Select(x => (GetGameName(x), RequiresCustomEntry(x))).ToList())))
                            {
                                foreach (var game in args.Games)
                                {
                                    {
                                        await Task.Run(() => RestoreOneGame(game));
                                    }
                                }
                            }
                        }
                    }
                );
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
                                if (UserConsents(translator.AddTagForSelectedGames_Confirm(candidate, args.Games.Select(x => GetGameName(x)))))
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
                                if (UserConsents(translator.RemoveTagForSelectedGames_Confirm(candidate, args.Games.Select(x => GetGameName(x)))))
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
                RefreshLudusaviBackups();

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

            var gameError = new RestorationError
            {
                Message = null,
                Empty = false,
            };
            var platformError = new RestorationError
            {
                Message = null,
                Empty = false,
            };

            if (prefs.Game.Restore.Do)
            {
                var choice = Choice.No;
                if (!prefs.Game.Restore.Ask || (choice = AskUser(translator.RestoreOneGame_Confirm(GetGameName(game), RequiresCustomEntry(game)))).Accepted())
                {
                    gameError = RestoreOneGame(game);
                }
                UpdateTagsForChoice(game, choice, TAG_GAME_BACKUP_AND_RESTORE, TAG_GAME_NO_RESTORE, TAG_GAME_BACKUP);
            }

            if (prefs.Platform.Restore.Do)
            {
                var choice = Choice.No;
                if (!prefs.Platform.Restore.Ask || (choice = AskUser(translator.RestoreOneGame_Confirm(game.Platforms[0].Name, true))).Accepted())
                {
                    platformError = RestoreOneGame(game, null, new BackupCriteria { ByPlatform = true });
                }
                UpdateTagsForChoice(game, choice, TAG_PLATFORM_BACKUP_AND_RESTORE, TAG_PLATFORM_NO_RESTORE, TAG_PLATFORM_BACKUP);
            }

            if (!String.IsNullOrEmpty(gameError.Message) && !gameError.Empty)
            {
                ShowError(gameError.Message);
            }
            else if (!String.IsNullOrEmpty(platformError.Message) && !platformError.Empty)
            {
                ShowError(platformError.Message);
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

            if (prefs.Game.Backup.Do)
            {
                var choice = Choice.No;
                if (!prefs.Game.Backup.Ask || (choice = AskUser(translator.BackUpOneGame_Confirm(GetGameName(game), RequiresCustomEntry(game)))).Accepted())
                {
                    Task.Run(() => BackUpOneGame(game, OperationTiming.AfterPlay));
                }
                UpdateTagsForChoice(game, choice, TAG_GAME_BACKUP, TAG_GAME_NO_BACKUP);
            }

            if (prefs.Platform.Backup.Do)
            {
                var choice = Choice.No;
                if (!prefs.Platform.Backup.Ask || (choice = AskUser(translator.BackUpOneGame_Confirm(game.Platforms[0].Name, true))).Accepted())
                {
                    Task.Run(() => BackUpOneGame(game, OperationTiming.AfterPlay, new BackupCriteria { ByPlatform = true }));
                }
                UpdateTagsForChoice(game, choice, TAG_PLATFORM_BACKUP, TAG_PLATFORM_NO_BACKUP);
            }
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
            if (this.appVersion.supportsMultiBackup())
            {
                var (code, response) = InvokeLudusavi(new Invocation(Mode.Backups).Path(settings.BackupPath));
                if (response?.Games != null)
                {
                    this.backups = response?.Games.ToDictionary(pair => pair.Key, pair => pair.Value.Backups);
                }
            }

            if (this.settings.TagGamesWithBackups)
            {
                TagGamesWithBackups();
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

        private (int, ApiResponse?) InvokeLudusavi(Invocation invocation)
        {
            var fullArgs = invocation.Render(settings, appVersion);
            logger.Debug(string.Format("Running Ludusavi: {0}", fullArgs));

            int code;
            string stdout;
            try
            {
                (code, stdout) = RunCommand(settings.ExecutablePath.Trim(), fullArgs);
            }
            catch (Exception e)
            {
                logger.Debug(e, "Ludusavi could not be executed");
                return (-1, null);
            }

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
            return HasTag(game, TAG_SKIP) || (game.Platforms != null && game.Platforms.Count > 1);
        }

        string GetGameName(Game game)
        {
            if (!IsOnPc(game) && settings.AddSuffixForNonPcGameNames)
            {
                return string.Format("{0}{1}", game.Name, settings.SuffixForNonPcGameNames.Replace("<platform>", game.Platforms[0].Name));
            }
            else
            {
                return game.Name;
            }
        }

        bool IsOnSteam(Game game)
        {
            return game.Source?.Name == "Steam"
                || game.PluginId == Guid.Parse("cb91dfc9-b977-43bf-8e70-55f46e410fab");
        }

        bool IsOnPc(Game game)
        {
            var pcSpecs = new List<string> { "macintosh", "pc_dos", "pc_linux", "pc_windows" };
            var pcNames = new List<string> { "Macintosh", "PC", "PC (DOS)", "PC (Linux)", "PC (Windows)" };
            return game.Platforms == null
                || game.Platforms.Count == 0
                || pcSpecs.Any(x => x == game.Platforms[0].SpecificationId)
                || pcNames.Any(x => x == game.Platforms[0].Name);
        }

        bool RequiresCustomEntry(Game game)
        {
            return !IsOnPc(game);
        }

        private string FindGame(Game game, string name, OperationTiming timing, BackupCriteria criteria, Mode mode)
        {
            if (!this.appVersion.supportsFindCommand())
            {
                return null;
            }

            var invocation = new Invocation(Mode.Find).Path(settings.BackupPath).Game(name);
            if (mode == Mode.Backup)
            {
                invocation.FindBackup();
            }
            if (!criteria.ByPlatform)
            {
                if (IsOnSteam(game) && int.TryParse(game.GameId, out var id))
                {
                    invocation.SteamId(id);
                }
                if (!IsOnPc(game) && settings.RetryNonPcGamesWithoutSuffix && name != game.Name)
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

        private void BackUpOneGame(Game game, OperationTiming timing)
        {
            this.BackUpOneGame(game, timing, new BackupCriteria { ByPlatform = false });
        }

        private void BackUpOneGame(Game game, OperationTiming timing, BackupCriteria criteria)
        {
            pendingOperation = true;
            var name = criteria.ByPlatform ? game.Platforms[0].Name : GetGameName(game);
            var displayName = name;
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

            var invocation = new Invocation(Mode.Backup).Path(settings.BackupPath).Game(name);

            var (code, response) = InvokeLudusavi(invocation);

            if (!this.appVersion.supportsFindCommand() && !criteria.ByPlatform)
            {
                if (response?.Errors.UnknownGames != null && IsOnSteam(game))
                {
                    (code, response) = InvokeLudusavi(invocation.BySteamId(game.GameId));
                }
                if (response?.Errors.UnknownGames != null && !IsOnPc(game) && settings.RetryNonPcGamesWithoutSuffix && name != game.Name)
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

            if (refresh)
            {
                RefreshLudusaviBackups();
            }
            pendingOperation = false;
        }

        private void BackUpAllGames()
        {
            pendingOperation = true;
            var (code, response) = InvokeLudusavi(new Invocation(Mode.Backup).Path(settings.BackupPath));

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

            RefreshLudusaviBackups();
            pendingOperation = false;
        }

        private void BackUpOneGameDuringPlay(Game game)
        {
            var prefs = GetPlayPreferences(game);

            if (prefs.Game.Backup.Do && !prefs.Game.Backup.Ask && settings.DoBackupDuringPlay)
            {
                Task.Run(() => BackUpOneGame(game, OperationTiming.DuringPlay));
            }

            if (prefs.Platform.Backup.Do && !prefs.Platform.Backup.Ask && settings.DoBackupDuringPlay)
            {
                Task.Run(() => BackUpOneGame(game, OperationTiming.DuringPlay, new BackupCriteria { ByPlatform = true }));
            }
        }

        private RestorationError RestoreOneGame(Game game)
        {
            return this.RestoreOneGame(game, null);
        }

        private RestorationError RestoreOneGame(Game game, string backup)
        {
            return this.RestoreOneGame(game, backup, new BackupCriteria { ByPlatform = false });
        }

        private RestorationError RestoreOneGame(Game game, string backup, BackupCriteria criteria)
        {
            RestorationError error = new RestorationError
            {
                Message = null,
                Empty = false,
            };
            pendingOperation = true;
            var name = criteria.ByPlatform ? game.Platforms[0].Name : GetGameName(game);
            var displayName = name;

            if (this.appVersion.supportsFindCommand())
            {
                var found = FindGame(game, name, OperationTiming.Free, criteria, Mode.Restore);
                if (found == null)
                {
                    pendingOperation = false;
                    error.Message = translator.UnrecognizedGame(name);
                    error.Empty = true;
                    NotifyError(error.Message);
                    return error;
                }
                name = found;
                if (name != displayName)
                {
                    displayName = $"{displayName} (↪ {name})";
                }
            }

            var invocation = new Invocation(Mode.Restore).Path(settings.BackupPath).Game(name).Backup(backup);

            var (code, response) = InvokeLudusavi(invocation);
            if (!this.appVersion.supportsFindCommand() && !criteria.ByPlatform)
            {
                if (response?.Errors.UnknownGames != null && IsOnSteam(game) && this.appVersion.supportsRestoreBySteamId())
                {
                    (code, response) = InvokeLudusavi(invocation.BySteamId(game.GameId));
                }
                if (response?.Errors.UnknownGames != null && !IsOnPc(game) && settings.RetryNonPcGamesWithoutSuffix && name != game.Name)
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
                    if ((response?.Overall.ChangedGames?.Same ?? 0) == 0)
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

            pendingOperation = false;
            return error;
        }

        private void RestoreAllGames()
        {
            pendingOperation = true;
            var (code, response) = InvokeLudusavi(new Invocation(Mode.Restore).Path(settings.BackupPath));

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
                if (this.backups.ContainsKey(game.Name))
                {
                    AddTag(game, TAG_BACKED_UP);
                }
                else
                {
                    RemoveTag(game, TAG_BACKED_UP);
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
                && (IsOnPc(game) || !settings.OnlyBackupOnGameStoppedIfPc || HasTag(game, TAG_GAME_BACKUP) || HasTag(game, TAG_GAME_BACKUP_AND_RESTORE));
            var platformBackupDo = (settings.DoPlatformBackupOnNonPcGameStopped || HasTag(game, TAG_PLATFORM_BACKUP) || HasTag(game, TAG_PLATFORM_BACKUP_AND_RESTORE))
                && !HasTag(game, TAG_PLATFORM_NO_BACKUP)
                && !IsOnPc(game);

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
    }
}
