﻿using Newtonsoft.Json;
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
                    translator.BackUpOneGame_Label(),
                    async () =>
                    {
                        if (!CanPerformOperationOnLastGamePlayed())
                        {
                            return;
                        }
                        if (UserConsents(translator.BackUpOneGame_Confirm(lastGamePlayed)))
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
                    translator.RestoreOneGame_Label(),
                    async () =>
                    {
                        if (!CanPerformOperationOnLastGamePlayed())
                        {
                            return;
                        }
                        if (UserConsents(translator.RestoreOneGame_Confirm(lastGamePlayed)))
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

            if (settings.DoBackupOnGameStopped)
            {
                if (!settings.AskBackupOnGameStopped || UserConsents(translator.BackUpOneGame_Confirm(game)))
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

        private (int, ApiResponse) InvokeLudusavi(string args)
        {
            var (code, stdout) = RunCommand(settings.ExecutablePath.Trim(), args + " --api");
            return (code, JsonConvert.DeserializeObject<ApiResponse>(stdout));
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

        private void BackUpOneGame(Game game)
        {
            pendingOperation = true;
            var (code, response) = InvokeLudusavi(string.Format("backup --merge --path \"{0}\" \"{1}\"", settings.BackupPath, game.Name));
            if (response.Errors.UnknownGames != null && game.Source.Name == "Steam")
            {
                (code, response) = InvokeLudusavi(string.Format("backup --merge --path \"{0}\" --by-steam-id \"{1}\"", settings.BackupPath, game.GameId));
            }
            var result = new OperationResult { Game = game, Response = response };

            if (code == 0)
            {
                if (response.Overall.TotalGames > 0)
                {
                    PlayniteApi.Notifications.Add(
                        Guid.NewGuid().ToString(),
                        translator.BackUpOneGame_Success(result),
                        NotificationType.Info
                    );
                }
                else
                {
                    PlayniteApi.Notifications.Add(
                        Guid.NewGuid().ToString(),
                        translator.BackUpOneGame_Empty(result),
                        NotificationType.Error
                    );
                }
            }
            else
            {
                PlayniteApi.Notifications.Add(
                    Guid.NewGuid().ToString(),
                    translator.BackUpOneGame_Failure(result),
                    NotificationType.Error
                );
            }
            pendingOperation = false;
        }

        private void BackUpAllGames()
        {
            pendingOperation = true;
            var (code, response) = InvokeLudusavi(string.Format("backup --merge --path \"{0}\"", settings.BackupPath));
            var result = new OperationResult { Response = response };

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
            pendingOperation = false;
        }

        private void RestoreOneGame(Game game)
        {
            pendingOperation = true;
            var (code, response) = InvokeLudusavi(string.Format("restore --force --path \"{0}\" \"{1}\"", settings.BackupPath, game.Name));
            if (response.Errors.UnknownGames != null && game.Source.Name == "Steam")
            {
                (code, response) = InvokeLudusavi(string.Format("restore --force --path \"{0}\" --by-steam-id \"{1}\"", settings.BackupPath, game.GameId));
            }
            var result = new OperationResult { Game = game, Response = response };

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
                if (response.Errors.UnknownGames != null)
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
            pendingOperation = false;
        }

        private void RestoreAllGames()
        {
            pendingOperation = true;
            var (code, response) = InvokeLudusavi(string.Format("restore --force --path \"{0}\"", settings.BackupPath));
            var result = new OperationResult { Response = response };

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
            pendingOperation = false;
        }
    }
}
