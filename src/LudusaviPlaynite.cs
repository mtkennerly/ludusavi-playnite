using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using Playnite.SDK.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LudusaviPlaynite
{
    public class LudusaviPlaynite : GenericPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        public LudusaviPlayniteSettings settings { get; set; }
        public override Guid Id { get; } = Guid.Parse("72e2de43-d859-44d8-914e-4277741c8208");

        public Cli.App app;
        public Interactor interactor;
        private Translator translator;
        private bool pendingOperation { get; set; }
        private bool playedSomething { get; set; }
        private Game lastGamePlayed { get; set; }
        private bool multipleGamesRunning { get; set; }
        private Timer duringPlayBackupTimer { get; set; }
        private int duringPlayBackupTotal { get; set; }
        private int duringPlayBackupFailed { get; set; }
        private Timer checkAppUpdateTimer { get; set; }

        public LudusaviPlaynite(IPlayniteAPI api) : base(api)
        {
            translator = new Translator(PlayniteApi.ApplicationSettings.Language);
            settings = new LudusaviPlayniteSettings(this, translator);
            app = new Cli.App(LudusaviPlaynite.logger, this.settings);
            interactor = new Interactor(api, settings, translator);
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };

            this.checkAppUpdateTimer = new Timer(
                x => CheckAppUpdate(),
                null,
                TimeSpan.FromHours(24.1),
                TimeSpan.FromHours(24.1)
            );
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
                        app.Launch();
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
                        if (interactor.UserConsents(translator.BackUpAllGames_Confirm()))
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
                        if (interactor.UserConsents(translator.RestoreAllGames_Confirm()))
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
                            if (interactor.UserConsents(translator.BackUpSelectedGames_Confirm(args.Games.Select(x => settings.GetGameName(x)).ToList())))
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
                    if (this.app.version.supportsEditBackup())
                    {
                        var section = string.Format("{0} | {1} | {2}", translator.Ludusavi(), translator.RestoreSelectedGames_Label(), Etc.GetBackupDisplayLine(backup));

                        items.Add(
                            new GameMenuItem
                            {
                                Description = translator.Restore(),
                                MenuSection = section,
                                Action = async args =>
                                {
                                    await InitiateOperation(game, Operation.Restore, OperationTiming.Free, BackupCriteria.Game, backup);
                                }
                            }
                        );

                        items.Add(
                            new GameMenuItem
                            {
                                Description = backup.Locked ? translator.Unlock() : translator.Lock(),
                                MenuSection = section,
                                Action = args =>
                                {
                                    var title = GetTitle(game);
                                    this.app.EditBackup(title, !backup.Locked, null);
                                    this.RefreshBackups();
                                }
                            }
                        );
                        items.Add(
                            new GameMenuItem
                            {
                                Description = translator.SetComment(),
                                MenuSection = section,
                                Action = args =>
                                {
                                    var comment = interactor.InputText(translator.SetComment(), backup.Comment);
                                    if (comment != null)
                                    {
                                        var title = GetTitle(game);
                                        this.app.EditBackup(title, null, comment);
                                        this.RefreshBackups();
                                    }
                                }
                            }
                        );
                    }
                    else
                    {
                        items.Add(
                            new GameMenuItem
                            {
                                Description = Etc.GetBackupDisplayLine(backup),
                                MenuSection = string.Format("{0} | {1}", translator.Ludusavi(), translator.RestoreSelectedGames_Label()),
                                Action = async args =>
                                {
                                    await InitiateOperation(game, Operation.Restore, OperationTiming.Free, BackupCriteria.Game, backup);
                                }
                            }
                        );
                    }
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
                            if (interactor.UserConsents(translator.RestoreSelectedGames_Confirm(args.Games.Select(x => settings.GetGameName(x)).ToList())))
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

            var backupPaths = menuArgs.Games.Select(GetBackupPath).Where(x => x != null);
            if (backupPaths.Any())
            {
                items.Add(
                    new GameMenuItem
                    {
                        Description = translator.OpenBackupDirectory(),
                        MenuSection = translator.Ludusavi(),
                        Action = args =>
                        {
                            var failed = new List<string>();

                            foreach (var backupPath in backupPaths)
                            {
                                if (!Etc.OpenDir(backupPath))
                                {
                                    failed.Add(backupPath);
                                }
                            }

                            if (failed.Any())
                            {
                                var message = this.translator.CannotOpenFolder();
                                var paths = string.Join("\n", failed);
                                var body = $"{message}\n\n{paths}";
                                interactor.ShowError(body);
                            }
                        }
                    }
                );
            }

            if (menuArgs.Games.Count == 1)
            {
                var title = menuArgs.Games[0].Name;
                string renamed = Etc.GetDictValue(settings.AlternativeTitles, title, null);

                items.Add(
                    new GameMenuItem
                    {
                        Description = translator.LookUpAsOtherTitle(renamed),
                        MenuSection = translator.Ludusavi(),
                        Action = args =>
                        {
                            GenericItemOption result = null;

                            if (this.app.version.supportsManifestShow() && this.app.manifestGames.Count > 0)
                            {
                                var options = this.app.manifestGames.Select(x => new GenericItemOption(x, "")).ToList();
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
                                            return this.app.manifestGames
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
                                Refresh(RefreshContext.ConfiguredTitle);
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
                                Refresh(RefreshContext.ConfiguredTitle);
                            }
                        }
                    );
                }

                if (this.app.version.supportsGuiCommand())
                {
                    items.Add(
                        new GameMenuItem
                        {
                            Description = translator.CustomizeInLudusavi(),
                            MenuSection = translator.Ludusavi(),
                            Action = args =>
                            {
                                if (!this.app.OpenCustomGame(renamed ?? title))
                                {
                                    interactor.NotifyError(translator.UnableToRunLudusavi(), OperationTiming.Free);
                                }
                            }
                        }
                    );
                }
            }

            foreach (var entry in Tags.CONFLICTS)
            {
                var candidate = entry.Key;
                var conflicts = entry.Value;

                if (menuArgs.Games.Any(x => !Etc.HasTag(x, candidate)))
                {
                    items.Add(
                        new GameMenuItem
                        {
                            Description = translator.AddTagForSelectedGames_Label(candidate),
                            MenuSection = translator.Ludusavi(),
                            Action = async args =>
                            {
                                if (interactor.UserConsents(translator.AddTagForSelectedGames_Confirm(candidate, args.Games.Select(x => x.Name))))
                                {
                                    using (PlayniteApi.Database.BufferedUpdate())
                                    {
                                        foreach (var game in args.Games)
                                        {
                                            {
                                                await Task.Run(() =>
                                                {
                                                    interactor.AddTag(game, candidate);
                                                    foreach (var conflict in conflicts)
                                                    {
                                                        var removed = interactor.RemoveTag(game, conflict);
                                                        string replacement;
                                                        if (removed && Tags.REPLACEMENTS.TryGetValue((candidate, conflict), out replacement))
                                                        {
                                                            interactor.AddTag(game, replacement);
                                                        }
                                                    }
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    );
                }

                if (menuArgs.Games.Any(x => Etc.HasTag(x, candidate)))
                {
                    items.Add(
                        new GameMenuItem
                        {
                            Description = translator.RemoveTagForSelectedGames_Label(candidate),
                            MenuSection = translator.Ludusavi(),
                            Action = async args =>
                            {
                                if (interactor.UserConsents(translator.RemoveTagForSelectedGames_Confirm(candidate, args.Games.Select(x => x.Name))))
                                {
                                    using (PlayniteApi.Database.BufferedUpdate())
                                    {
                                        foreach (var game in args.Games)
                                        {
                                            {
                                                await Task.Run(() =>
                                                {
                                                    interactor.RemoveTag(game, candidate);
                                                });
                                            }
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
                var oldTag = PlayniteApi.Database.Tags.FirstOrDefault(x => x.Name == Tags.LEGACY_SKIP);
                var newTagExists = PlayniteApi.Database.Tags.Any(x => x.Name == Tags.SKIP);
                if (oldTag != null && !newTagExists)
                {
                    oldTag.Name = Tags.SKIP;
                    PlayniteApi.Database.Tags.Update(oldTag);
                }
                settings.MigratedTags = true;
                SavePluginSettings(settings);
            }

            Task.Run(() =>
            {
                Refresh(RefreshContext.Startup);

                if (app.version.inner < Etc.RECOMMENDED_APP_VERSION && new Version(settings.SuggestedUpgradeTo) < Etc.RECOMMENDED_APP_VERSION)
                {
                    interactor.NotifyInfo(
                        translator.UpgradePrompt(Etc.RECOMMENDED_APP_VERSION.ToString()),
                        () =>
                        {
                            Etc.OpenLudusaviReleasePage();
                        }
                    );
                    settings.SuggestedUpgradeTo = Etc.RECOMMENDED_APP_VERSION.ToString();
                    SavePluginSettings(settings);
                }
            });
        }

        public override void OnGameStarting(OnGameStartingEventArgs args)
        {
            playedSomething = true;
            lastGamePlayed = args.Game;
            if (CountGamesRunning() > 0)
            {
                multipleGamesRunning = true;
            }
            Game game = args.Game;
            var prefs = settings.GetPlayPreferences(game, multipleGamesRunning);

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
            if (CountGamesRunning() == 0)
            {
                multipleGamesRunning = false;
            }
            Game game = arg.Game;
            var prefs = settings.GetPlayPreferences(game, multipleGamesRunning);

            if (this.duringPlayBackupTimer != null)
            {
                this.duringPlayBackupTimer.Change(Timeout.Infinite, Timeout.Infinite);
                if (this.duringPlayBackupFailed == 0)
                {
                    interactor.NotifyInfo(translator.BackUpDuringPlay_Success(game.Name, this.duringPlayBackupTotal));
                }
                else
                {
                    interactor.NotifyError(translator.BackUpDuringPlay_Failure(game.Name, this.duringPlayBackupTotal, this.duringPlayBackupFailed));
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

        public void Refresh(RefreshContext context)
        {
            switch (context)
            {
                case RefreshContext.Startup:
                    app.RefreshVersion();

                    if (!app.IsValid())
                    {
                        interactor.ShowError(translator.InitialSetupRequired());
                        Etc.OpenLudusaviMainPage();
                    }

                    app.RefreshTitles(PlayniteApi.Database.Games.ToList());
                    RefreshBackups();
                    RefreshGames();
                    CheckAppUpdate();
                    break;
                case RefreshContext.EditedConfig:
                    app.RefreshVersion();
                    app.RefreshTitles(PlayniteApi.Database.Games.ToList());
                    app.RefreshBackups();
                    break;
                case RefreshContext.ConfiguredTitle:
                    app.RefreshTitles(PlayniteApi.Database.Games.ToList());
                    RefreshBackups();
                    break;
                case RefreshContext.CreatedBackup:
                    RefreshBackups();
                    break;
            }
        }

        private void HandleSuccessDuringPlay()
        {
            this.duringPlayBackupTotal += 1;
        }

        private void HandleFailureDuringPlay()
        {
            this.duringPlayBackupTotal += 1;
            this.duringPlayBackupFailed += 1;
        }

        private void RefreshGames()
        {
            app.RefreshGames();
            if (this.settings.TagGamesWithUnknownSaveData)
            {
                TagGamesWithUnknownSaveData();
            }
        }

        private void RefreshBackups()
        {
            if (app.RefreshBackups() && this.settings.TagGamesWithBackups)
            {
                TagGamesWithBackups();
            }
        }

        private void CheckAppUpdate()
        {
            if (!(settings.CheckAppUpdate))
            {
                return;
            }

            if ((DateTime.UtcNow - settings.CheckedAppUpdate).TotalHours < 24)
            {
                return;
            }

            settings.CheckedAppUpdate = DateTime.UtcNow;

            var update = app.CheckAppUpdate();
            if (update != null && update?.version != settings.PresentedAppUpdate && update?.version != app.version.inner.ToString())
            {
                settings.PresentedAppUpdate = update?.version;
                interactor.NotifyInfo(
                    translator.UpgradeAvailable(update?.version),
                    () =>
                    {
                        Etc.OpenUrl(update?.url);
                    }
                );
            }

            SavePluginSettings(settings);
        }

        public void NotifyResponseErrors(Cli.Output.Response? response)
        {
            if (response?.Errors.CloudSyncFailed != null)
            {
                var prefix = translator.Ludusavi();
                var error = translator.CloudSyncFailed();
                interactor.NotifyError($"{prefix}: {error}");
            }
            if (response?.Errors.CloudConflict != null)
            {
                var prefix = translator.Ludusavi();
                var error = translator.CloudConflict();
                interactor.NotifyError($"{prefix}: {error}", () => app.Launch());
            }
        }

        private void ShowFullResults(Cli.Output.Response response)
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

        string GetTitle(Game game)
        {
            return Etc.GetDictValue(this.app.titles, Etc.GetTitleId(game), null);
        }

        private string FindGame(Game game, string name, OperationTiming timing, BackupCriteria criteria, Mode mode)
        {
            if (this.app.version.supportsApiCommand() && criteria.ByGame())
            {
                var title = GetTitle(game);
                if (title != null)
                {
                    return title;
                }
            }

            if (!this.app.version.supportsFindCommand())
            {
                return null;
            }

            var invocation = new Cli.Invocation(Mode.Find).PathIf(settings.BackupPath, settings.OverrideBackupPath).Game(name);

            if (mode == Mode.Backup)
            {
                invocation.FindBackup();
            }
            if (criteria.ByGame() && settings.AlternativeTitle(game) == null)
            {
                // There can't be an alt title because the Steam ID/etc would take priority over it.

                if (Etc.TrySteamId(game, out var id))
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

            var (code, response) = app.Invoke(invocation);
            if (response == null)
            {
                interactor.NotifyError(translator.UnableToRunLudusavi(), timing);
                HandleFailureDuringPlay();
                return null;
            }

            var officialName = response?.Games.Keys.FirstOrDefault();
            if (code != 0 || officialName == null)
            {
                interactor.NotifyError(translator.UnrecognizedGame(name), timing);
                HandleFailureDuringPlay();
                return null;
            }

            return officialName;
        }

        private void InitiateOperationSync(Game game, Operation operation, OperationTiming timing, BackupCriteria criteria, Cli.Output.Backup? backup = null)
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

            if (criteria.ByPlatform() && Etc.GetGamePlatform(game) == null)
            {
                return;
            }

            var prefs = settings.GetPlayPreferences(game, multipleGamesRunning);
            var ask = prefs.ShouldAsk(timing, criteria, operation);
            var displayName = settings.GetDisplayName(game, criteria);
            var consented = !ask;

            if (ask)
            {
                if (timing == OperationTiming.Free)
                {
                    switch (operation)
                    {
                        case Operation.Backup:
                            consented = interactor.UserConsents(translator.BackUpOneGame_Confirm(displayName));
                            break;
                        case Operation.Restore:
                            consented = interactor.UserConsents(translator.RestoreOneGame_Confirm(displayName));
                            break;
                    }
                }
                else
                {
                    var choice = Choice.No;
                    switch (operation)
                    {
                        case Operation.Backup:
                            choice = interactor.AskUser(translator.BackUpOneGame_Confirm(displayName), !multipleGamesRunning);
                            break;
                        case Operation.Restore:
                            choice = interactor.AskUser(translator.RestoreOneGame_Confirm(displayName), !multipleGamesRunning);
                            break;
                    }

                    consented = choice.Accepted();

                    switch (operation)
                    {
                        case Operation.Backup:
                            switch (criteria)
                            {
                                case BackupCriteria.Game:
                                    interactor.UpdateTagsForChoice(game, choice, Tags.GAME_BACKUP, Tags.GAME_NO_BACKUP);
                                    break;
                                case BackupCriteria.Platform:
                                    interactor.UpdateTagsForChoice(game, choice, Tags.PLATFORM_BACKUP, Tags.PLATFORM_NO_BACKUP);
                                    break;
                            }
                            break;
                        case Operation.Restore:
                            switch (criteria)
                            {
                                case BackupCriteria.Game:
                                    interactor.UpdateTagsForChoice(game, choice, Tags.GAME_BACKUP_AND_RESTORE, Tags.GAME_NO_RESTORE, Tags.GAME_BACKUP);
                                    break;
                                case BackupCriteria.Platform:
                                    interactor.UpdateTagsForChoice(game, choice, Tags.PLATFORM_BACKUP_AND_RESTORE, Tags.PLATFORM_NO_RESTORE, Tags.PLATFORM_BACKUP);
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
                        interactor.ShowError(error.Message);
                    }
                    break;
            }
        }

        private async Task InitiateOperation(Game game, Operation operation, OperationTiming timing, BackupCriteria criteria, Cli.Output.Backup? backup = null)
        {
            await Task.Run(() => InitiateOperationSync(game, operation, timing, criteria, backup));
        }

        private void BackUpOneGame(Game game, OperationTiming timing, BackupCriteria criteria)
        {
            pendingOperation = true;
            var name = criteria.ByPlatform() ? game.Platforms[0].Name : settings.GetGameNameWithAlt(game);
            var displayName = game.Name;
            var refresh = true;

            if (this.app.version.supportsFindCommand())
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

            var invocation = new Cli.Invocation(Mode.Backup).PathIf(settings.BackupPath, settings.OverrideBackupPath).Game(name);

            var (code, response) = app.Invoke(invocation);

            if (!this.app.version.supportsFindCommand() && criteria.ByGame() && settings.AlternativeTitle(game) == null)
            {
                if (response?.Errors.UnknownGames != null && Etc.IsOnSteam(game))
                {
                    (code, response) = app.Invoke(invocation.BySteamId(game.GameId));
                }
                if (response?.Errors.UnknownGames != null && !Etc.IsOnPc(game) && settings.RetryNonPcGamesWithoutSuffix)
                {
                    (code, response) = app.Invoke(invocation.Game(game.Name));
                }
            }

            if (response == null)
            {
                interactor.NotifyError(translator.UnableToRunLudusavi(), timing);
                HandleFailureDuringPlay();
            }
            else
            {
                var result = new OperationResult { Name = displayName, Response = (Cli.Output.Response)response };
                if (code == 0)
                {
                    if (response?.Overall.TotalGames > 0)
                    {
                        if ((response?.Overall.ChangedGames?.Same ?? 0) == 0)
                        {
                            interactor.NotifyInfo(translator.BackUpOneGame_Success(result), timing);
                            HandleSuccessDuringPlay();
                        }
                        else
                        {
                            refresh = false;
                            interactor.NotifyInfo(translator.BackUpOneGame_Unchanged(result), timing);
                            HandleSuccessDuringPlay();
                        }
                    }
                    else
                    {
                        refresh = false;
                        interactor.NotifyError(translator.BackUpOneGame_Empty(result), timing);
                        HandleFailureDuringPlay();
                    }
                }
                else
                {
                    if (response?.Errors.UnknownGames != null)
                    {
                        refresh = false;
                        interactor.NotifyError(translator.BackUpOneGame_Empty(result), timing);
                        HandleFailureDuringPlay();
                    }
                    else
                    {
                        interactor.NotifyError(translator.BackUpOneGame_Failure(result), timing);
                        HandleFailureDuringPlay();
                    }
                }
            }

            NotifyResponseErrors(response);
            if (refresh)
            {
                Refresh(RefreshContext.CreatedBackup);
            }
            pendingOperation = false;
        }

        private void BackUpAllGames()
        {
            pendingOperation = true;
            var (code, response) = app.Invoke(new Cli.Invocation(Mode.Backup).PathIf(settings.BackupPath, settings.OverrideBackupPath));

            if (response == null)
            {
                interactor.NotifyError(translator.UnableToRunLudusavi());
            }
            else
            {
                var result = new OperationResult { Response = (Cli.Output.Response)response };

                if (code == 0)
                {
                    interactor.NotifyInfo(translator.BackUpAllGames_Success(result), () => ShowFullResults(result.Response));
                }
                else
                {
                    interactor.NotifyError(translator.BackUpAllGames_Failure(result), () => ShowFullResults(result.Response));
                }
            }

            NotifyResponseErrors(response);
            Refresh(RefreshContext.CreatedBackup);
            pendingOperation = false;
        }

        private void BackUpOneGameDuringPlay(Game game)
        {
            if (!CanPerformOperationSuppressed())
            {
                return;
            }
            var prefs = settings.GetPlayPreferences(game, multipleGamesRunning);
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

        private RestorationError RestoreOneGame(Game game, Cli.Output.Backup? backup, BackupCriteria criteria)
        {
            RestorationError error = new RestorationError
            {
                Message = null,
                Empty = false,
            };

            pendingOperation = true;
            var name = criteria.ByPlatform() ? game.Platforms[0].Name : settings.GetGameNameWithAlt(game);
            var displayName = game.Name;

            if (this.app.version.supportsFindCommand())
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

            var invocation = new Cli.Invocation(Mode.Restore).PathIf(settings.BackupPath, settings.OverrideBackupPath).Game(name).Backup(backup?.Name);

            var (code, response) = app.Invoke(invocation);
            if (!this.app.version.supportsFindCommand() && criteria.ByGame() && settings.AlternativeTitle(game) == null)
            {
                if (response?.Errors.UnknownGames != null && Etc.IsOnSteam(game) && this.app.version.supportsRestoreBySteamId())
                {
                    (code, response) = app.Invoke(invocation.BySteamId(game.GameId));
                }
                if (response?.Errors.UnknownGames != null && !Etc.IsOnPc(game) && settings.RetryNonPcGamesWithoutSuffix)
                {
                    (code, response) = app.Invoke(invocation.Game(game.Name));
                }
            }

            if (response == null)
            {
                error.Message = translator.UnableToRunLudusavi();
                interactor.NotifyError(error.Message);
            }
            else
            {
                var result = new OperationResult { Name = displayName, Response = (Cli.Output.Response)response };
                if (code == 0)
                {
                    if (response?.Overall.TotalGames == 0)
                    {
                        // This applies to Ludusavi v0.23.0 and later
                        error.Message = translator.RestoreOneGame_Empty(result);
                        error.Empty = true;
                        interactor.NotifyError(error.Message);
                    }
                    else if ((response?.Overall.ChangedGames?.Same ?? 0) == 0)
                    {
                        interactor.NotifyInfo(translator.RestoreOneGame_Success(result));
                    }
                    else
                    {
                        interactor.NotifyInfo(translator.RestoreOneGame_Unchanged(result));
                    }
                }
                else
                {
                    if (response?.Errors.UnknownGames != null)
                    {
                        // This applies to Ludusavi versions before v0.23.0
                        error.Message = translator.RestoreOneGame_Empty(result);
                        error.Empty = true;
                        interactor.NotifyError(error.Message);
                    }
                    else
                    {
                        error.Message = translator.RestoreOneGame_Failure(result);
                        interactor.NotifyError(error.Message);
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
            var (code, response) = app.Invoke(new Cli.Invocation(Mode.Restore).PathIf(settings.BackupPath, settings.OverrideBackupPath));

            if (response == null)
            {
                interactor.NotifyError(translator.UnableToRunLudusavi());
            }
            else
            {
                var result = new OperationResult { Response = (Cli.Output.Response)response };

                if (code == 0)
                {
                    interactor.NotifyInfo(translator.RestoreAllGames_Success(result), () => ShowFullResults(result.Response));
                }
                else
                {
                    interactor.NotifyError(translator.RestoreAllGames_Failure(result), () => ShowFullResults(result.Response));
                }

            }

            NotifyResponseErrors(response);
            pendingOperation = false;
        }

        private void TagGamesWithBackups()
        {
            using (PlayniteApi.Database.BufferedUpdate())
            {
                foreach (var game in PlayniteApi.Database.Games)
                {
                    if (IsBackedUp(game))
                    {
                        interactor.AddTag(game, Tags.BACKED_UP);
                    }
                    else
                    {
                        interactor.RemoveTag(game, Tags.BACKED_UP);
                    }
                }
            }
        }

        private void TagGamesWithUnknownSaveData()
        {
            using (PlayniteApi.Database.BufferedUpdate())
            {
                foreach (var game in PlayniteApi.Database.Games)
                {
                    if (!GameHasKnownSaveData(game))
                    {
                        interactor.AddTag(game, Tags.UNKNOWN_SAVE_DATA);
                    }
                    else
                    {
                        interactor.RemoveTag(game, Tags.UNKNOWN_SAVE_DATA);
                    }
                }
            }
        }

        private int CountGamesRunning()
        {
            return PlayniteApi.Database.Games.Count(x => x.IsRunning);
        }

        private bool IsBackedUp(Game game)
        {
            return GetBackups(game).Count > 0;
        }

        private bool GameHasKnownSaveData(Game game)
        {
            string title;
            if (this.app.version.supportsApiCommand())
            {
                title = GetTitle(game);
            }
            else
            {
                // Ideally, we would use the `find` command, but that's too slow to run in bulk.
                title = settings.AlternativeTitle(game) ?? settings.GetGameName(game);
            }

            if (title != null && this.app.manifestGamesWithSaveDataByTitle.Contains(title))
            {
                return true;
            }

            if (Etc.TrySteamId(game, out var id) && this.app.manifestGamesWithSaveDataBySteamId.Contains(id))
            {
                return true;
            }

            return false;
        }

        private List<Cli.Output.Backup> GetBackups(Game game)
        {
            if (this.app.version.supportsApiCommand())
            {
                var title = GetTitle(game);
                var backups = Etc.GetDictValue(this.app.backups, title, new List<Cli.Output.Backup>());

                // Sort newest backups to the top.
                backups.Sort((x, y) => y.When.CompareTo(x.When));

                return backups;
            }

            var ret = new List<Cli.Output.Backup>();
            var alt = settings.AlternativeTitle(game);

            if (alt != null)
            {
                ret = Etc.GetDictValue(this.app.backups, alt, new List<Cli.Output.Backup>());
            }
            else
            {
                ret = Etc.GetDictValue(
                    this.app.backups,
                    settings.GetGameName(game),
                    Etc.GetDictValue(
                        this.app.backups,
                        game.Name,
                        new List<Cli.Output.Backup>()
                    )
                );
            }

            // Sort newest backups to the top.
            ret.Sort((x, y) => y.When.CompareTo(x.When));

            return ret;
        }

        private string GetBackupPath(Game game)
        {
            return Etc.GetDictValue(this.app.backupPaths, GetTitle(game) ?? settings.GetGameNameWithAlt(game), null);
        }
    }
}
