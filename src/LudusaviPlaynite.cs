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

        private bool PendingOperation { get; set; }
        private bool PlayedSomething { get; set; }
        private Game LastGamePlayed { get; set; }

        public LudusaviPlaynite(IPlayniteAPI api) : base(api)
        {
            settings = new LudusaviPlayniteSettings(this);
        }

        public override IEnumerable<ExtensionFunction> GetFunctions()
        {
            return new List<ExtensionFunction>
            {
                new ExtensionFunction(
                    "Ludusavi: Launch",
                    () =>
                    {
                        LaunchLudusavi();
                    }
                ),
                new ExtensionFunction(
                    "Ludusavi: Back up save data for last game played",
                    async () =>
                    {
                        if (!CanPerformOperationOnLastGamePlayed())
                        {
                            return;
                        }
                        if (UserConsents(String.Format("Back up save data for {0}?", LastGamePlayed.Name)))
                        {
                            await Task.Run(() => BackUpOneGame(LastGamePlayed));
                        }
                    }
                ),
                new ExtensionFunction(
                    "Ludusavi: Back up save data for all games",
                    async () =>
                    {
                        if (!CanPerformOperation())
                        {
                            return;
                        }
                        if (UserConsents("Back up save data for all games that Ludusavi can find?"))
                        {
                            await Task.Run(() => BackUpAllGames());
                        }
                    }
                ),
                new ExtensionFunction(
                    "Ludusavi: Restore save data for last game played",
                    async () =>
                    {
                        if (!CanPerformOperationOnLastGamePlayed())
                        {
                            return;
                        }
                        if (UserConsents(String.Format("Restore save data for {0}?", LastGamePlayed.Name)))
                        {
                            await Task.Run(() => RestoreOneGame(LastGamePlayed));
                        }
                    }
                ),
                new ExtensionFunction(
                    "Ludusavi: Restore save data for all games",
                    async () =>
                    {
                        if (!CanPerformOperation())
                        {
                            return;
                        }
                        if (UserConsents("Restore save data for all games that Ludusavi can find?"))
                        {
                            await Task.Run(() => RestoreAllGames());
                        }
                    }
                ),
            };
        }

        public override void OnGameStopped(Game game, long elapsedSeconds)
        {
            PlayedSomething = true;
            LastGamePlayed = game;

            if (settings.DoBackupOnGameStopped)
            {
                if (!settings.AskBackupOnGameStopped || UserConsents(String.Format("Back up save data for {0}?", game.Name)))
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

        private (int, String) RunCommand(String command, String args)
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

        private (int, String) InvokeLudusavi(String args)
        {
            return RunCommand(settings.ExecutablePath.Trim(), args);
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
            if (PendingOperation)
            {
                PlayniteApi.Dialogs.ShowMessage("Ludusavi is still working on a previous request. Please try again when it's done.");
                return false;
            }
            return true;
        }

        private bool CanPerformOperationOnLastGamePlayed()
        {
            if (!PlayedSomething)
            {
                PlayniteApi.Dialogs.ShowMessage("You haven't played anything yet in this session.");
                return false;
            }
            return CanPerformOperation();
        }

        private bool UserConsents(String message)
        {
            var choice = PlayniteApi.Dialogs.ShowMessage(message, "", System.Windows.MessageBoxButton.YesNo);
            return choice == MessageBoxResult.Yes;
        }

        private void BackUpOneGame(Game game)
        {
            PendingOperation = true;
            var (code, stdout) = InvokeLudusavi(String.Format("backup --force --path \"{0}\" \"{1}\"", settings.BackupPath, game.Name));
            if (code == 0)
            {
                PlayniteApi.Notifications.Add(
                    String.Format("ludusavi-backup-success-for-{0}", game.Name),
                    String.Format("Backed up saves for {0}", game.Name),
                    NotificationType.Info
                );
            }
            else
            {
                PlayniteApi.Notifications.Add(
                    String.Format("ludusavi-backup-failure-for-{0}", game.Name),
                    String.Format("Unable to back up saves for {0}", game.Name),
                    NotificationType.Error
                );
            }
            PendingOperation = false;
        }

        private void BackUpAllGames()
        {
            PendingOperation = true;
            var (code, stdout) = InvokeLudusavi(String.Format("backup --force --path \"{0}\"", settings.BackupPath));
            if (code == 0)
            {
                PlayniteApi.Notifications.Add(
                    "ludusavi-backup-success-all",
                    "Backed up saves for all games",
                    NotificationType.Info
                );
            }
            else
            {
                PlayniteApi.Notifications.Add(
                    "ludusavi-backup-failure-all",
                    "Unable to back up saves for some games",
                    NotificationType.Error
                );
            }
            PendingOperation = false;
        }

        private void RestoreOneGame(Game game)
        {
            PendingOperation = true;
            var (code, stdout) = InvokeLudusavi(String.Format("restore --force --path \"{0}\" \"{1}\"", settings.BackupPath, game.Name));
            if (code == 0)
            {
                PlayniteApi.Notifications.Add(
                    String.Format("ludusavi-restore-success", game.Name),
                    String.Format("Restored saves for {0}", game.Name),
                    NotificationType.Info
                );
            }
            else
            {
                PlayniteApi.Notifications.Add(
                    String.Format("ludusavi-restore-failure", game.Name),
                    String.Format("Unable to back up saves for {0}", game.Name),
                    NotificationType.Error
                );
            }
            PendingOperation = false;
        }

        private void RestoreAllGames()
        {
            PendingOperation = true;
            var (code, stdout) = InvokeLudusavi(String.Format("restore --force --path \"{0}\"", settings.BackupPath));
            if (code == 0)
            {
                PlayniteApi.Notifications.Add(
                    "ludusavi-restore-success-all",
                    "Restored saves for all games",
                    NotificationType.Info
                );
            }
            else
            {
                PlayniteApi.Notifications.Add(
                    "ludusavi-restore-failure-all",
                    "Unable to restore saves for some games",
                    NotificationType.Error
                );
            }
            PendingOperation = false;
        }
    }
}
