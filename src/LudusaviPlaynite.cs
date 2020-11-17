using ByteSizeLib;
using Newtonsoft.Json;
using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LudusaviPlaynite
{
    public class LudusaviPlaynite : Plugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        public LudusaviPlayniteSettings settings { get; set; }
        public override Guid Id { get; } = Guid.Parse("72e2de43-d859-44d8-914e-4277741c8208");

        private Translator translator;
        private bool pendingOperation { get; set; }
        private bool playedSomething { get; set; }
        private Game lastGamePlayed { get; set; }

        public LudusaviPlaynite(IPlayniteAPI api) : base(api)
        {
            translator = new Translator(DetermineLanguage());
            settings = new LudusaviPlayniteSettings(this, translator);
        }

        private Language DetermineLanguage()
        {
            switch (PlayniteApi.ApplicationSettings.Language)
            {
                case "en_US":
                default:
                    return Language.English;
            }
        }

        public override List<MainMenuItem> GetMainMenuItems(GetMainMenuItemsArgs menuArgs)
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
                            await Task.Run(() => BackUpOneGame(lastGamePlayed));
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

        public override List<GameMenuItem> GetGameMenuItems(GetGameMenuItemsArgs menuArgs)
        {
            return new List<GameMenuItem>
            {
                new GameMenuItem
                {
                    Description = translator.BackUpSelectedGames_Label(),
                    MenuSection = translator.Ludusavi(),
                    Action = async args => {
                        if (UserConsents(translator.BackUpSelectedGames_Confirm(args.Games.Select(x => (GetGameName(x), RequiresCustomEntry(x))).ToList())))
                        {
                            foreach (var game in args.Games)
                            {
                                {
                                    await Task.Run(() => BackUpOneGame(game));
                                }
                            }
                        }
                    }
                },
                new GameMenuItem
                {
                    Description = translator.RestoreSelectedGames_Label(),
                    MenuSection = translator.Ludusavi(),
                    Action = async args => {
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
            };
        }

        public override void OnGameStopped(Game game, long elapsedSeconds)
        {
            playedSomething = true;
            lastGamePlayed = game;

            if (settings.DoBackupOnGameStopped && !ShouldSkipGame(game) && (IsOnPc(game) || !settings.OnlyBackupOnGameStoppedIfPc))
            {
                if (!settings.AskBackupOnGameStopped || UserConsents(translator.BackUpOneGame_Confirm(GetGameName(game), RequiresCustomEntry(game))))
                {
                    Task.Run(() => BackUpOneGame(game));
                }
            }

            if (settings.DoPlatformBackupOnNonPcGameStopped && !ShouldSkipGame(game) && !IsOnPc(game))
            {
                if (!settings.AskPlatformBackupOnNonPcGameStopped || UserConsents(translator.BackUpOneGame_Confirm(game.Platform.Name, true)))
                {
                    Task.Run(() => BackUpOneGame(game, new BackupCriteria { ByPlatform = true }));
                }
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

        private void NotifyError(string message)
        {
            NotifyError(message, () => { });
        }

        private void NotifyError(string message, Action action)
        {
            PlayniteApi.Notifications.Add(new NotificationMessage(Guid.NewGuid().ToString(), message, NotificationType.Error, action));
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
            p.Start();

            var stdout = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return (p.ExitCode, stdout);
        }

        private (int, ApiResponse?) InvokeLudusavi(string args)
        {
            var fullArgs = string.Format("{0} --api", args);
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

        private bool ShouldSkipGame(Game game)
        {
            return game.Tags != null && game.Tags.Any(x => x.Name == "ludusavi-skip");
        }

        string GetGameName(Game game)
        {
            if (!IsOnPc(game) && settings.AddSuffixForNonPcGameNames)
            {
                return string.Format("{0}{1}", game.Name, settings.SuffixForNonPcGameNames.Replace("<platform>", game.Platform?.Name));
            }
            else
            {
                return game.Name;
            }
        }

        bool IsOnSteam(Game game)
        {
            return game.Source?.Name == "Steam";
        }

        bool IsOnPc(Game game)
        {
            return game.Platform?.Name == "PC";
        }

        bool RequiresCustomEntry(Game game)
        {
            return !IsOnPc(game);
        }

        private void BackUpOneGame(Game game)
        {
            this.BackUpOneGame(game, new BackupCriteria { ByPlatform = false });
        }

        private void BackUpOneGame(Game game, BackupCriteria criteria)
        {
            pendingOperation = true;
            var name = criteria.ByPlatform ? game.Platform.Name : GetGameName(game);

            var (code, response) = InvokeLudusavi(string.Format("backup --merge --try-update --path \"{0}\" \"{1}\"", settings.BackupPath, name));
            if (!criteria.ByPlatform)
            {
                if (response?.Errors.UnknownGames != null && IsOnSteam(game))
                {
                    (code, response) = InvokeLudusavi(string.Format("backup --merge --try-update --path \"{0}\" --by-steam-id \"{1}\"", settings.BackupPath, game.GameId));
                }
                if (response?.Errors.UnknownGames != null && !IsOnPc(game) && settings.RetryNonPcGamesWithoutSuffix && name != game.Name)
                {
                    (code, response) = InvokeLudusavi(string.Format("backup --merge --try-update --path \"{0}\" \"{1}\"", settings.BackupPath, game.Name));
                }
            }

            if (response == null)
            {
                NotifyError(translator.UnableToRunLudusavi());
            }
            else
            {
                var result = new OperationResult { Name = name, Response = (ApiResponse)response };
                if (code == 0)
                {
                    if (response?.Overall.TotalGames > 0)
                    {
                        NotifyInfo(translator.BackUpOneGame_Success(result));
                    }
                    else
                    {
                        NotifyError(translator.BackUpOneGame_Empty(result));
                    }
                }
                else
                {
                    if (response?.Errors.UnknownGames != null)
                    {
                        NotifyError(translator.BackUpOneGame_Empty(result));
                    }
                    else
                    {
                        NotifyError(translator.BackUpOneGame_Failure(result));
                    }
                }
            }

            pendingOperation = false;
        }

        private void BackUpAllGames()
        {
            pendingOperation = true;
            var (code, response) = InvokeLudusavi(string.Format("backup --merge --try-update --path \"{0}\"", settings.BackupPath));

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
            pendingOperation = false;
        }

        private void RestoreOneGame(Game game)
        {
            pendingOperation = true;
            var name = GetGameName(game);

            var (code, response) = InvokeLudusavi(string.Format("restore --force --path \"{0}\" \"{1}\"", settings.BackupPath, name));
            if (response?.Errors.UnknownGames != null && IsOnSteam(game))
            {
                (code, response) = InvokeLudusavi(string.Format("restore --force --path \"{0}\" --by-steam-id \"{1}\"", settings.BackupPath, game.GameId));
            }
            if (response?.Errors.UnknownGames != null && !IsOnPc(game) && settings.RetryNonPcGamesWithoutSuffix && name != game.Name)
            {
                (code, response) = InvokeLudusavi(string.Format("restore --force --path \"{0}\" \"{1}\"", settings.BackupPath, game.Name));
            }

            if (response == null)
            {
                NotifyError(translator.UnableToRunLudusavi());
            }
            else
            {
                var result = new OperationResult { Name = name, Response = (ApiResponse)response };
                if (code == 0)
                {
                    NotifyInfo(translator.RestoreOneGame_Success(result));
                }
                else
                {
                    if (response?.Errors.UnknownGames != null)
                    {
                        NotifyError(translator.RestoreOneGame_Empty(result));
                    }
                    else
                    {
                        NotifyError(translator.RestoreOneGame_Failure(result));
                    }
                }
            }

            pendingOperation = false;
        }

        private void RestoreAllGames()
        {
            pendingOperation = true;
            var (code, response) = InvokeLudusavi(string.Format("restore --force --path \"{0}\"", settings.BackupPath));

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
    }
}
