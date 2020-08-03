using Newtonsoft.Json;
using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private LudusaviPlayniteSettings settings { get; set; }
        public override Guid Id { get; } = Guid.Parse("72e2de43-d859-44d8-914e-4277741c8208");

        private Translator translator = new Translator();
        private bool pendingOperation { get; set; }
        private bool playedSomething { get; set; }
        private Game lastGamePlayed { get; set; }

        public LudusaviPlaynite(IPlayniteAPI api) : base(api)
        {
            settings = new LudusaviPlayniteSettings(this);
        }

        public override IEnumerable<ExtensionFunction> GetFunctions()
        {
            return new List<ExtensionFunction>
            {
                new ExtensionFunction(
                    translator.Launch_Label(),
                    () =>
                    {
                        LaunchLudusavi();
                    }
                ),
                new ExtensionFunction(
                    translator.BackUpLastGame_Label(),
                    async () =>
                    {
                        if (!CanPerformOperationOnLastGamePlayed())
                        {
                            return;
                        }
                        if (UserConsents(translator.BackUpOneGame_Confirm(GetGameName(lastGamePlayed))))
                        {
                            await Task.Run(() => BackUpOneGame(lastGamePlayed));
                        }
                    }
                ),
                new ExtensionFunction(
                    translator.BackUpAllGames_Label(),
                    async () =>
                    {
                        if (!CanPerformOperation())
                        {
                            return;
                        }
                        if (UserConsents(translator.BackUpAllGames_Confirm()))
                        {
                            await Task.Run(() => BackUpAllGames());
                        }
                    }
                ),
                new ExtensionFunction(
                    translator.RestoreLastGame_Label(),
                    async () =>
                    {
                        if (!CanPerformOperationOnLastGamePlayed())
                        {
                            return;
                        }
                        if (UserConsents(translator.RestoreOneGame_Confirm(GetGameName(lastGamePlayed))))
                        {
                            await Task.Run(() => RestoreOneGame(lastGamePlayed));
                        }
                    }
                ),
                new ExtensionFunction(
                    translator.RestoreAllGames_Label(),
                    async () =>
                    {
                        if (!CanPerformOperation())
                        {
                            return;
                        }
                        if (UserConsents(translator.RestoreAllGames_Confirm()))
                        {
                            await Task.Run(() => RestoreAllGames());
                        }
                    }
                ),
            };
        }

        public override void OnGameStopped(Game game, long elapsedSeconds)
        {
            playedSomething = true;
            lastGamePlayed = game;

            if (settings.DoBackupOnGameStopped && (game.Platform?.Name == "PC" || !settings.OnlyBackupOnGameStoppedIfPc))
            {
                if (!settings.AskBackupOnGameStopped || UserConsents(translator.BackUpOneGame_Confirm(GetGameName(game))))
                {
                    BackUpOneGame(game);
                }
            }
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new LudusaviPlayniteSettingsView();
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
            var (code, stdout) = RunCommand(settings.ExecutablePath.Trim(), fullArgs);

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

        string GetGameName(Game game)
        {
            if (game.Platform?.Name != "PC" && settings.AddSuffixForNonPcGameNames)
            {
                return string.Format("{0}{1}", game.Name, settings.SuffixForNonPcGameNames.Replace("<platform>", game.Platform?.Name));
            }
            else
            {
                return game.Name;
            }
        }

        private void BackUpOneGame(Game game)
        {
            pendingOperation = true;
            var name = GetGameName(game);

            var (code, response) = InvokeLudusavi(string.Format("backup --merge --try-update --path \"{0}\" \"{1}\"", settings.BackupPath, name));
            if (response?.Errors.UnknownGames != null && game.Source?.Name == "Steam")
            {
                (code, response) = InvokeLudusavi(string.Format("backup --merge --try-update --path \"{0}\" --by-steam-id \"{1}\"", settings.BackupPath, game.GameId));
            }

            if (response == null)
            {
                PlayniteApi.Notifications.Add(Guid.NewGuid().ToString(), translator.UnableToRunLudusavi(), NotificationType.Error);
            }
            else
            {
                var result = new OperationResult { Game = game, Name = name, Response = (ApiResponse)response };
                if (code == 0)
                {
                    PlayniteApi.Notifications.Add(
                        Guid.NewGuid().ToString(),
                        translator.BackUpOneGame_Success(result),
                        NotificationType.Info
                    );
                }
                else
                {
                    if (response?.Errors.UnknownGames != null)
                    {
                        PlayniteApi.Notifications.Add(
                            Guid.NewGuid().ToString(),
                            translator.BackUpOneGame_Empty(result),
                            NotificationType.Error
                        );
                    }
                    else
                    {
                        PlayniteApi.Notifications.Add(
                            Guid.NewGuid().ToString(),
                            translator.BackUpOneGame_Failure(result),
                            NotificationType.Error
                        );
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
                PlayniteApi.Notifications.Add(Guid.NewGuid().ToString(), translator.UnableToRunLudusavi(), NotificationType.Error);
            }
            else
            {
                var result = new OperationResult { Response = (ApiResponse)response };

                if (code == 0)
                {
                    PlayniteApi.Notifications.Add(
                        Guid.NewGuid().ToString(),
                        translator.BackUpAllGames_Success(result),
                        NotificationType.Info
                    );
                }
                else
                {
                    PlayniteApi.Notifications.Add(
                        Guid.NewGuid().ToString(),
                        translator.BackUpAllGames_Failure(result),
                        NotificationType.Error
                    );
                }
            }
            pendingOperation = false;
        }

        private void RestoreOneGame(Game game)
        {
            pendingOperation = true;
            var name = GetGameName(game);

            var (code, response) = InvokeLudusavi(string.Format("restore --force --path \"{0}\" \"{1}\"", settings.BackupPath, name));
            if (response?.Errors.UnknownGames != null && game.Source?.Name == "Steam")
            {
                (code, response) = InvokeLudusavi(string.Format("restore --force --path \"{0}\" --by-steam-id \"{1}\"", settings.BackupPath, game.GameId));
            }

            if (response == null)
            {
                PlayniteApi.Notifications.Add(Guid.NewGuid().ToString(), translator.UnableToRunLudusavi(), NotificationType.Error);
            }
            else
            {
                var result = new OperationResult { Game = game, Name = name, Response = (ApiResponse)response };
                if (code == 0)
                {
                    PlayniteApi.Notifications.Add(
                        Guid.NewGuid().ToString(),
                        translator.RestoreOneGame_Success(result),
                        NotificationType.Info
                    );
                }
                else
                {
                    if (response?.Errors.UnknownGames != null)
                    {
                        PlayniteApi.Notifications.Add(
                            Guid.NewGuid().ToString(),
                            translator.RestoreOneGame_Empty(result),
                            NotificationType.Error
                        );
                    }
                    else
                    {
                        PlayniteApi.Notifications.Add(
                            Guid.NewGuid().ToString(),
                            translator.RestoreOneGame_Failure(result),
                            NotificationType.Error
                        );
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
                PlayniteApi.Notifications.Add(Guid.NewGuid().ToString(), translator.UnableToRunLudusavi(), NotificationType.Error);
            }
            else
            {
                var result = new OperationResult { Response = (ApiResponse)response };

                if (code == 0)
                {
                    PlayniteApi.Notifications.Add(
                        Guid.NewGuid().ToString(),
                        translator.RestoreAllGames_Success(result),
                        NotificationType.Info
                    );
                }
                else
                {
                    PlayniteApi.Notifications.Add(
                        Guid.NewGuid().ToString(),
                        translator.RestoreAllGames_Failure(result),
                        NotificationType.Error
                    );
                }
            }

            pendingOperation = false;
        }
    }
}
