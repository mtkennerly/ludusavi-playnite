using ByteSizeLib;
using Linguini.Bundle;
using Linguini.Bundle.Builder;
using Linguini.Shared.Types.Bundle;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LudusaviPlaynite
{
    using FluentArgs = Dictionary<string, IFluentType>;

    public class Translator
    {
        readonly static string GAME = "game";
        readonly static string PROCESSED_GAMES = "processed-games";
        readonly static string PROCESSED_SIZE = "processed-size";
        readonly static string TOTAL_GAMES = "total-games";
        readonly static string TOTAL_SIZE = "total-size";
        readonly static string TAG = "tag";
        readonly static string TOTAL_BACKUPS = "total-backups";
        readonly static string FAILED_BACKUPS = "failed-backups";

        private FluentBundle bundle;

        public Translator(string language)
        {
            SetLanguage(language);
        }

        private FluentBundle MakeBundle(string language)
        {
            return LinguiniBuilder.Builder()
                .CultureInfo(new CultureInfo(language))
                .AddResource(ReadFtl(language))
                .SetUseIsolating(false)
                .UncheckedBuild();
        }

        private string ReadFtl(string language)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("LudusaviPlaynite." + language.Replace("_", "-") + ".ftl"))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private void SetLanguage(string language)
        {
            this.bundle = MakeBundle("en-US");

            string target;
            try
            {
                target = ReadFtl(language);
            }
            catch
            {
                // No translation for this language.
                return;
            }
            this.bundle.AddResourceOverriding(target);
        }

        private int TotalCustomGames(bool needsCustomEntry)
        {
            return needsCustomEntry ? 1 : 0;
        }

        private int TotalCustomGames(List<(string, bool)> games)
        {
            return games.Where(x => x.Item2).Count();
        }

        private string Translate(string id, FluentArgs args = null)
        {
            return this.bundle.GetAttrMessage(id, args);
        }

        public string Ludusavi()
        {
            return Translate("ludusavi");
        }

        public string AdjustedSize(ulong bytes)
        {
            return ByteSize.FromBytes(bytes).ToBinaryString();
        }

        public string SelectFileExecutableFilter()
        {
            return Translate("file-filter-executable") + "|*.exe";
        }

        public string BrowseButton()
        {
            return Translate("button-browse");
        }

        public string OpenButton()
        {
            return Translate("button-open");
        }

        public string YesButton()
        {
            return Translate("button-yes");
        }

        public string YesRememberedButton()
        {
            return Translate("button-yes-remembered");
        }

        public string NoButton()
        {
            return Translate("button-no");
        }

        public string NoRememberedButton()
        {
            return Translate("button-no-remembered");
        }

        public string Launch_Label()
        {
            return Translate("label-launch");
        }

        public string BackUpLastGame_Label()
        {
            return Translate("back-up-last-game");
        }

        private string NeedsCustomEntry(int totalGames)
        {
            return Translate(
                "needs-custom-entry",
                new FluentArgs() {
                    { TOTAL_GAMES, (FluentNumber)totalGames },
                }
            );
        }

        private string NeedsCustomEntry_Appended(bool needed)
        {
            return needed ? " " + NeedsCustomEntry(1) : "";
        }

        private string NeedsCustomEntry_Appended(List<(string, bool)> games)
        {
            return TotalCustomGames(games) > 0 ? " " + NeedsCustomEntry(games.Count) : "";
        }

        private string ChangeSummary(OperationResult result)
        {
            var totalNew = result.Response.Overall.ChangedGames?.New ?? 0;
            var totalDiff = result.Response.Overall.ChangedGames?.Different ?? 0;
            return $" [+{totalNew}, Δ{totalDiff}]";
        }

        public string BackUpOneGame_Confirm(string gameName, bool needsCustomEntry)
        {
            return Translate(
                "back-up-specific-game.confirm",
                new FluentArgs() {
                    {GAME, (FluentString)gameName},
                }
            ) + NeedsCustomEntry_Appended(needsCustomEntry);
        }

        public string BackUpAllGames_Label()
        {
            return Translate("back-up-all-games");
        }

        public string BackUpAllGames_Confirm()
        {
            return Translate("back-up-all-games.confirm");
        }

        public string BackUpSelectedGames_Label()
        {
            return Translate("back-up-selected-games");
        }

        string GetSelectionFormattedNames(IEnumerable<string> games)
        {
            if (games.Count() < 50)
            {
                return "\n\n" + String.Join(" | ", games);
            }
            return "";
        }

        // games: (name, requiresCustomEntry)
        public string BackUpSelectedGames_Confirm(List<(string, bool)> games)
        {
            var count = games.Count();
            if (count == 1)
            {
                return BackUpOneGame_Confirm(games[0].Item1, games[0].Item2);
            }

            var formattedNames = GetSelectionFormattedNames(games.Select(x => x.Item1));

            return Translate(
                "back-up-selected-games.confirm",
                new FluentArgs() {
                    {TOTAL_GAMES, (FluentNumber)count},
                }
            ) + NeedsCustomEntry_Appended(games) + formattedNames;
        }

        public string RestoreLastGame_Label()
        {
            return Translate("restore-last-game");
        }

        public string RestoreOneGame_Confirm(string gameName, bool needsCustomEntry)
        {
            return Translate(
                "restore-specific-game.confirm",
                new FluentArgs() {
                    {GAME, (FluentString)gameName},
                }
            ) + NeedsCustomEntry_Appended(needsCustomEntry);
        }

        public string RestoreAllGames_Label()
        {
            return Translate("restore-all-games");
        }

        public string RestoreAllGames_Confirm()
        {
            return Translate("restore-all-games.confirm");
        }

        public string RestoreSelectedGames_Label()
        {
            return Translate("restore-selected-games");
        }

        // games: (name, requiresCustomEntry)
        public string RestoreSelectedGames_Confirm(List<(string, bool)> games)
        {
            var count = games.Count();
            if (count == 1)
            {
                return RestoreOneGame_Confirm(games[0].Item1, games[0].Item2);
            }

            var formattedNames = GetSelectionFormattedNames(games.Select(x => x.Item1));
            return Translate(
                "restore-selected-games.confirm",
                new FluentArgs() {
                    {TOTAL_GAMES, (FluentNumber)count},
                }
            ) + NeedsCustomEntry_Appended(games) + formattedNames;
        }

        public string AddTagForSelectedGames_Label(string tag)
        {
            return Translate(
                "add-tag-for-selected-games",
                new FluentArgs() {
                    {TAG, (FluentString)tag},
                }
            );
        }

        public string AddTagForSelectedGames_Confirm(string tag, IEnumerable<string> games)
        {
            var formattedNames = GetSelectionFormattedNames(games);
            return Translate(
                "add-tag-for-selected-games.confirm",
                new FluentArgs() {
                    {TAG, (FluentString)tag},
                    {TOTAL_GAMES, (FluentNumber)games.Count()},
                }
            ) + formattedNames;
        }

        public string RemoveTagForSelectedGames_Label(string tag)
        {
            return Translate(
                "remove-tag-for-selected-games",
                new FluentArgs() {
                    {TAG, (FluentString)tag},
                }
            );
        }

        public string RemoveTagForSelectedGames_Confirm(string tag, IEnumerable<string> games)
        {
            var formattedNames = GetSelectionFormattedNames(games);
            return Translate(
                "remove-tag-for-selected-games.confirm",
                new FluentArgs() {
                    {TAG, (FluentString)tag},
                    {TOTAL_GAMES, (FluentNumber)games.Count()},
                }
            ) + formattedNames;
        }

        public string OperationStillPending()
        {
            return Translate("operation-still-pending");
        }

        public string NoGamePlayedYet()
        {
            return Translate("no-game-played-yet");
        }

        public string UnableToRunLudusavi()
        {
            return Translate("unable-to-run-ludusavi");
        }

        public string UnrecognizedGame(string name)
        {
            return Translate(
                "unrecognized-game",
                new FluentArgs() {
                    {GAME, (FluentString)name},
                }
            );
        }

        public string BackUpOneGame_Success(OperationResult result)
        {
            return Translate(
                "back-up-specific-game.on-success",
                new FluentArgs() {
                    {GAME, (FluentString)result.Name},
                    {PROCESSED_SIZE, (FluentString)AdjustedSize(result.Response.Overall.ProcessedBytes)},
                }
            );
        }

        public string BackUpOneGame_Unchanged(OperationResult result)
        {
            return Translate(
                "back-up-specific-game.on-unchanged",
                new FluentArgs() {
                    {GAME, (FluentString)result.Name},
                }
            );
        }

        public string BackUpOneGame_Empty(OperationResult result)
        {
            return Translate(
                "back-up-specific-game.on-empty",
                new FluentArgs() {
                    {GAME, (FluentString)result.Name},
                }
            );
        }

        public string BackUpOneGame_Failure(OperationResult result)
        {
            return Translate(
                "back-up-specific-game.on-failure",
                new FluentArgs() {
                    {GAME, (FluentString)result.Name},
                    {PROCESSED_SIZE, (FluentString)AdjustedSize(result.Response.Overall.ProcessedBytes)},
                    {TOTAL_SIZE, (FluentString)AdjustedSize(result.Response.Overall.TotalBytes)},
                }
            );
        }

        public string BackUpAllGames_Success(OperationResult result)
        {
            return Translate(
                "back-up-all-games.on-success",
                new FluentArgs() {
                    {PROCESSED_GAMES, (FluentNumber)result.Response.Overall.ProcessedGames},
                    {PROCESSED_SIZE, (FluentString)AdjustedSize(result.Response.Overall.ProcessedBytes)},
                }
            ) + ChangeSummary(result);
        }

        public string BackUpAllGames_Failure(OperationResult result)
        {
            return Translate(
                "back-up-all-games.on-failure",
                new FluentArgs() {
                    {PROCESSED_GAMES, (FluentNumber)result.Response.Overall.ProcessedGames},
                    {TOTAL_GAMES, (FluentNumber)result.Response.Overall.TotalGames},
                    {PROCESSED_SIZE, (FluentString)AdjustedSize(result.Response.Overall.ProcessedBytes)},
                    {TOTAL_SIZE, (FluentString)AdjustedSize(result.Response.Overall.TotalBytes)},
                }
            ) + ChangeSummary(result);
        }

        public string BackUpDuringPlay_Success(string game, int totalBackups)
        {
            return Translate(
                "back-up-during-play-on-success",
                new FluentArgs() {
                    {GAME, (FluentString)game},
                    {TOTAL_BACKUPS, (FluentNumber)totalBackups},
                }
            );
        }

        public string BackUpDuringPlay_Failure(string game, int totalBackups, int failedBackups)
        {
            return Translate(
                "back-up-during-play-on-failure",
                new FluentArgs() {
                    {GAME, (FluentString)game},
                    {TOTAL_BACKUPS, (FluentNumber)totalBackups},
                    {FAILED_BACKUPS, (FluentNumber)failedBackups},
                }
            );
        }

        public string RestoreOneGame_Success(OperationResult result)
        {
            return Translate(
                "restore-specific-game.on-success",
                new FluentArgs() {
                    {GAME, (FluentString)result.Name},
                    {PROCESSED_SIZE, (FluentString)AdjustedSize(result.Response.Overall.ProcessedBytes)}
                }
            );
        }

        public string RestoreOneGame_Unchanged(OperationResult result)
        {
            return Translate(
                "restore-specific-game.on-unchanged",
                new FluentArgs() {
                    {GAME, (FluentString)result.Name},
                }
            );
        }

        public string RestoreOneGame_Empty(OperationResult result)
        {
            return Translate(
                "restore-specific-game.on-empty",
                new FluentArgs() {
                    {GAME, (FluentString)result.Name},
                }
            );
        }

        public string RestoreOneGame_Failure(OperationResult result)
        {
            return Translate(
                "restore-specific-game.on-failure",
                new FluentArgs() {
                    {GAME, (FluentString)result.Name},
                    {PROCESSED_SIZE, (FluentString)AdjustedSize(result.Response.Overall.ProcessedBytes)},
                    {TOTAL_SIZE, (FluentString)AdjustedSize(result.Response.Overall.TotalBytes)},
                }
            );
        }

        public string RestoreAllGames_Success(OperationResult result)
        {
            return Translate(
                "restore-all-games.on-success",
                new FluentArgs() {
                    {PROCESSED_GAMES, (FluentNumber)result.Response.Overall.ProcessedGames},
                    {PROCESSED_SIZE, (FluentString)AdjustedSize(result.Response.Overall.ProcessedBytes)},
                }
            ) + ChangeSummary(result);
        }

        public string RestoreAllGames_Failure(OperationResult result)
        {
            return Translate(
                "restore-all-games.on-failure",
                new FluentArgs() {
                    {PROCESSED_GAMES, (FluentNumber)result.Response.Overall.ProcessedGames},
                    {TOTAL_GAMES, (FluentNumber)result.Response.Overall.TotalGames},
                    {PROCESSED_SIZE, (FluentString)AdjustedSize(result.Response.Overall.ProcessedBytes)},
                    {TOTAL_SIZE, (FluentString)AdjustedSize(result.Response.Overall.TotalBytes)},
                }
            ) + ChangeSummary(result);
        }

        public string FullListGameLineItem(string name, ApiGame game)
        {
            var size = AdjustedSize(Convert.ToUInt64(game.Files.Sum(x => Convert.ToDecimal(x.Value.Bytes))));
            var failed = game.Files.Any(x => x.Value.Failed) || game.Registry.Any(x => x.Value.Failed);
            var status = failed ? "failed" : (game.Decision == "Ignored" ? "ignored" : "success");

            var parts = new List<string>();

            if (game.Change == "New")
            {
                parts.Add("[+]");
            }
            else if (game.Change == "Different")
            {
                parts.Add("[Δ]");
            }

            if (failed)
            {
                parts.Add(string.Format("[{0}]", Translate("badge-failed")));
            }
            else if (game.Decision == "Ignored")
            {
                parts.Add(string.Format("[{0}]", Translate("badge-ignored")));
            }

            parts.Add(name);
            parts.Add(string.Format("({0})", size));

            return string.Join(" ", parts);
        }

        public string ExecutablePath_Label()
        {
            return Translate("config-executable-path");
        }

        public string BackupPath_Label()
        {
            return Translate("config-backup-path");
        }

        public string OverrideBackupFormat_Label()
        {
            return Translate("config-override-backup-format");
        }

        public string OverrideBackupCompression_Label()
        {
            return Translate("config-override-backup-compression");
        }

        public string OverrideBackupRetention_Label()
        {
            return Translate("config-override-backup-retention");
        }

        public string FullBackupLimit_Label()
        {
            return Translate("config-full-backup-limit");
        }

        public string DifferentialBackupLimit_Label()
        {
            return Translate("config-differential-backup-limit");
        }

        public string DoBackupOnGameStopped_Label()
        {
            return Translate("config-do-backup-on-game-stopped");
        }

        public string DoRestoreOnGameStarting_Label()
        {
            return Translate("config-do-restore-on-game-starting");
        }

        public string AskBackupOnGameStopped_Label()
        {
            return Translate("config-ask-backup-on-game-stopped");
        }

        public string OnlyBackupOnGameStoppedIfPc_Label()
        {
            return Translate("config-only-backup-on-game-stopped-if-pc");
        }

        public string RetryUnrecognizedGameWithNormalization_Label()
        {
            return Translate("config-retry-unrecognized-game-with-normalization");
        }

        public string AddSuffixForNonPcGameNames_Label()
        {
            return Translate("config-add-suffix-for-non-pc-game-names");
        }

        public string RetryNonPcGamesWithoutSuffix_Label()
        {
            return Translate("config-retry-non-pc-games-without-suffix");
        }

        public string DoPlatformBackupOnNonPcGameStopped_Label()
        {
            return Translate("config-do-platform-backup-on-non-pc-game-stopped");
        }

        public string DoPlatformRestoreOnNonPcGameStarting_Label()
        {
            return Translate("config-do-platform-restore-on-non-pc-game-starting");
        }

        public string AskPlatformBackupOnNonPcGameStopped_Label()
        {
            return Translate("config-ask-platform-backup-on-non-pc-game-stopped");
        }

        public string DoBackupDuringPlay_Label()
        {
            return Translate("config-do-backup-during-play");
        }

        public string BackupDuringPlayInterval_Label()
        {
            return Translate("label-minutes");
        }

        public string IgnoreBenignNotifications_Label()
        {
            return Translate("config-ignore-benign-notifications");
        }

        public string OptionSimple()
        {
            return Translate("option-simple");
        }

        public string OptionNone()
        {
            return Translate("option-none");
        }
    }
}
