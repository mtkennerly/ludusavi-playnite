using ByteSizeLib;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LudusaviPlaynite
{
    public class Translator
    {
        private Language language;

        public Translator(Language language)
        {
            this.language = language;
        }

        public string Ludusavi()
        {
            switch (language)
            {
                default:
                    return "Ludusavi";
            }
        }

        public string AdjustedSize(ulong bytes)
        {
            switch (language)
            {
                default:
                    return ByteSize.FromBytes(bytes).ToBinaryString();
            }
        }

        public string SelectFileExecutableFilter()
        {
            switch (language)
            {
                default:
                    return "Executable|*.exe";
            }
        }

        public string BrowseButton()
        {
            switch (language)
            {
                default:
                    return "Browse";
            }
        }

        public string OpenButton()
        {
            switch (language)
            {
                default:
                    return "Open";
            }
        }

        public string Launch_Label()
        {
            switch (language)
            {
                default:
                    return "Launch";
            }
        }

        public string BackUpLastGame_Label()
        {
            switch (language)
            {
                default:
                    return "Back up save data for last game played";
            }
        }

        string GetCustomNoteForSingleGame(bool needsCustomEntry)
        {
            if (needsCustomEntry)
            {
                switch (language)
                {
                    default:
                        return " This requires a matching custom entry in Ludusavi.";
                }
            }
            return "";
        }

        public string BackUpOneGame_Confirm(string gameName, bool needsCustomEntry)
        {
            var customNote = GetCustomNoteForSingleGame(needsCustomEntry);
            switch (language)
            {
                default:
                    return string.Format("Back up save data for {0}?{1}", gameName, customNote);
            }
        }

        public string BackUpAllGames_Label()
        {
            switch (language)
            {
                default:
                    return "Back up save data for all games";
            }
        }

        public string BackUpAllGames_Confirm()
        {
            switch (language)
            {
                default:
                    return "Back up save data for all games that Ludusavi can find?";
            }
        }

        public string BackUpSelectedGames_Label()
        {
            switch (language)
            {
                default:
                    return "Back up save data for selected games";
            }
        }

        public string UseBackupCopy_Label(string gameName)
        {
            switch (language)
            {
                default:
                    return $"Restore using";
            }
        }

        string GetSelectionFormattedNames(List<(string, bool)> games)
        {
            if (games.Count() < 50)
            {
                return "\n\n" + String.Join(" | ", games.Select(x => x.Item1));
            }
            return "";
        }

        string GetSelectionCustomNote(List<(string, bool)> games)
        {
            if (games.Any(x => x.Item2))
            {
                switch (language)
                {
                    default:
                        return " Some games require a matching custom entry in Ludusavi.";
                }
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

            var formattedNames = GetSelectionFormattedNames(games);
            var customNote = GetSelectionCustomNote(games);

            switch (language)
            {
                default:
                    return string.Format("Back up save data for {0} selected games?{1}{2}", count, customNote, formattedNames);
            }
        }

        public string RestoreLastGame_Label()
        {
            switch (language)
            {
                default:
                    return "Restore save data for last game played";
            }
        }

        public string RestoreOneGame_Confirm(string gameName, bool needsCustomEntry)
        {
            var customNote = GetCustomNoteForSingleGame(needsCustomEntry);
            switch (language)
            {
                default:
                    return string.Format("Restore save data for {0}?{1}", gameName, customNote);
            }
        }

        public string RestoreAllGames_Label()
        {
            switch (language)
            {
                default:
                    return "Restore save data for all games";
            }
        }

        public string RestoreAllGames_Confirm()
        {
            switch (language)
            {
                default:
                    return "Restore save data for all games that Ludusavi can find?";
            }
        }

        public string RestoreSelectedGames_Label()
        {
            switch (language)
            {
                default:
                    return "Restore save data for selected games";
            }
        }

        // games: (name, requiresCustomEntry)
        public string RestoreSelectedGames_Confirm(List<(string, bool)> games)
        {
            var count = games.Count();
            if (count == 1)
            {
                return RestoreOneGame_Confirm(games[0].Item1, games[0].Item2);
            }

            var formattedNames = GetSelectionFormattedNames(games);
            var customNote = GetSelectionCustomNote(games);

            switch (language)
            {
                default:
                    return string.Format("Restore save data for {0} selected games?{1}{2}", count, customNote, formattedNames);
            }
        }

        public string OperationStillPending()
        {
            switch (language)
            {
                default:
                    return "Ludusavi is still working on a previous request. Please try again when you see the notification that it's done.";
            }
        }

        public string NoGamePlayedYet()
        {
            switch (language)
            {
                default:
                    return "You haven't played anything yet in this session.";
            }
        }

        public string UnableToRunLudusavi()
        {
            switch (language)
            {
                default:
                    return "Unable to run Ludusavi.";
            }
        }

        public string BackUpOneGame_Success(OperationResult result)
        {
            switch (language)
            {
                default:
                    return string.Format(
                        "Backed up saves for {0} ({1})",
                        result.Name,
                        AdjustedSize(result.Response.Overall.ProcessedBytes)
                    );
            }
        }

        public string BackUpOneGame_Empty(OperationResult result)
        {
            switch (language)
            {
                default:
                    return string.Format(
                        "No save data found to back up for {0}",
                        result.Name
                    );
            }
        }

        public string BackUpOneGame_Failure(OperationResult result)
        {
            switch (language)
            {
                default:
                    return string.Format(
                        "Backed up saves for {0} ({1} of {2}), but some saves failed",
                        result.Name,
                        AdjustedSize(result.Response.Overall.ProcessedBytes),
                        AdjustedSize(result.Response.Overall.TotalBytes)
                    );
            }
        }

        public string BackUpAllGames_Success(OperationResult result)
        {
            switch (language)
            {
                default:
                    return string.Format(
                        "Backed up saves for {0} games ({1}); click for full list",
                        result.Response.Overall.ProcessedGames,
                        AdjustedSize(result.Response.Overall.ProcessedBytes)
                    );
            }
        }

        public string BackUpAllGames_Failure(OperationResult result)
        {
            switch (language)
            {
                default:
                    return string.Format(
                        "Backed up saves for {0} of {1} games ({2} of {3}), but some failed; click for full list",
                        result.Response.Overall.ProcessedGames,
                        result.Response.Overall.TotalGames,
                        AdjustedSize(result.Response.Overall.ProcessedBytes),
                        AdjustedSize(result.Response.Overall.TotalBytes)
                    );
            }
        }

        public string RestoreOneGame_Success(OperationResult result)
        {
            switch (language)
            {
                default:
                    return string.Format(
                        "Restored saves for {0} ({1})",
                        result.Name,
                        AdjustedSize(result.Response.Overall.ProcessedBytes)
                    );
            }
        }

        public string RestoreOneGame_Empty(OperationResult result)
        {
            switch (language)
            {
                default:
                    return string.Format(
                        "No save data found to restore for {0}",
                        result.Name
                    );
            }
        }

        public string RestoreOneGame_Failure(OperationResult result)
        {
            switch (language)
            {
                default:
                    return string.Format(
                        "Restored saves for {0} ({1} of {2}), but some saves failed",
                        result.Name,
                        AdjustedSize(result.Response.Overall.ProcessedBytes),
                        AdjustedSize(result.Response.Overall.TotalBytes)
                    );
            }
        }

        public string RestoreAllGames_Success(OperationResult result)
        {
            switch (language)
            {
                default:
                    return string.Format(
                        "Restored saves for {0} games ({1}); click for full list",
                        result.Response.Overall.ProcessedGames,
                        AdjustedSize(result.Response.Overall.ProcessedBytes)
                    );
            }
        }

        public string RestoreAllGames_Failure(OperationResult result)
        {
            switch (language)
            {
                default:
                    return string.Format(
                        "Restored saves for {0} of {1} games ({2} of {3}), but some failed; click for full list",
                        result.Response.Overall.ProcessedGames,
                        result.Response.Overall.TotalGames,
                        AdjustedSize(result.Response.Overall.ProcessedBytes),
                        AdjustedSize(result.Response.Overall.TotalBytes)
                    );
            }
        }

        public string FullListGameLineItem_Failed()
        {
            switch (language)
            {
                default:
                    return "FAILED";
            }
        }

        public string FullListGameLineItem_Ignored()
        {
            switch (language)
            {
                default:
                    return "IGNORED";
            }
        }

        public string FullListGameLineItem(string name, ApiGame game)
        {
            var size = AdjustedSize(Convert.ToUInt64(game.Files.Sum(x => Convert.ToDecimal(x.Value.Bytes))));
            var failed = game.Files.Any(x => x.Value.Failed) || game.Registry.Any(x => x.Value.Failed);

            switch (language)
            {
                default:
                    if (failed)
                    {
                        return string.Format("[{0}] {1} ({2})", FullListGameLineItem_Failed(), name, size);
                    }
                    else if (game.Decision == "Ignored")
                    {
                        return string.Format("[{0}] {1} ({2})", FullListGameLineItem_Ignored(), name, size);
                    }
                    else
                    {
                        return string.Format("{0} ({1})", name, size);
                    }
            }
        }

        public string ExecutablePath_Label()
        {
            switch (language)
            {
                default:
                    return "Name or full path of the Ludusavi executable:";
            }
        }

        public string BackupPath_Label()
        {
            switch (language)
            {
                default:
                    return "Full path to directory for storing backups:";
            }
        }

        public string DoBackupOnGameStopped_Label()
        {
            switch (language)
            {
                default:
                    return "Back up save data for a game when you finish playing it";
            }
        }

        public string AskBackupOnGameStopped_Label()
        {
            switch (language)
            {
                default:
                    return "Ask first instead of doing it automatically";
            }
        }

        public string AskCreateMultipleBackups_Label()
        {
            switch (language)
            {
                default:
                    return "Create multiple backups while the game is running";
            }
        }

        public string AskNumberOfBackupCopies_Label()
        {
            switch (language)
            {
                default:
                    return "Number of copies to keep:";
            }
        }

        public string AskBackupMinuteInterval_Label()
        {
            switch (language)
            {
                default:
                    return "Backup interval in minutes:";
            }
        }

        public string OnlyBackupOnGameStoppedIfPc_Label()
        {
            switch (language)
            {
                default:
                    return "Only do this for PC games";
            }
        }

        public string AddSuffixForNonPcGameNames_Label()
        {
            switch (language)
            {
                default:
                    return "Look up non-PC games by adding this suffix to their names (requires custom entry):";
            }
        }

        public string RetryNonPcGamesWithoutSuffix_Label()
        {
            switch (language)
            {
                default:
                    return "If not found with the suffix, then try again without it";
            }
        }

        public string DoPlatformBackupOnNonPcGameStopped_Label()
        {
            switch (language)
            {
                default:
                    return "Back up save data by platform name after playing non-PC games (requires custom entry)";
            }
        }

        public string AskPlatformBackupOnNonPcGameStopped_Label()
        {
            switch (language)
            {
                default:
                    return "Ask first instead of doing it automatically";
            }
        }

        public string IgnoreBenignNotifications_Label()
        {
            switch (language)
            {
                default:
                    return "Only show notifications on failure";
            }
        }
    }
}
