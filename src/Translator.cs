using ByteSizeLib;
using System;

namespace LudusaviPlaynite
{
    public class Translator
    {
        private Language language;

        public Translator(Language language = Language.English)
        {
            // TODO: Get active language from Playnite API?
            // https://github.com/JosefNemec/Playnite/issues/1937
            this.language = language;
        }

        public string AdjustedSize(int bytes)
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
                    return "Ludusavi: Launch";
            }
        }

        public string BackUpLastGame_Label()
        {
            switch (language)
            {
                default:
                    return "Ludusavi: Back up save data for last game played";
            }
        }

        public string BackUpOneGame_Confirm(string gameName)
        {
            switch (language)
            {
                default:
                    return string.Format("Back up save data for {0}?", gameName);
            }
        }

        public string BackUpAllGames_Label()
        {
            switch (language)
            {
                default:
                    return "Ludusavi: Back up save data for all games";
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

        public string RestoreLastGame_Label()
        {
            switch (language)
            {
                default:
                    return "Ludusavi: Restore save data for last game played";
            }
        }

        public string RestoreOneGame_Confirm(string gameName)
        {
            switch (language)
            {
                default:
                    return string.Format("Restore save data for {0}?", gameName);
            }
        }

        public string RestoreAllGames_Label()
        {
            switch (language)
            {
                default:
                    return "Ludusavi: Restore save data for all games";
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
                        "Backed up saves for {0} games ({1})",
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
                        "Backed up saves for {0} of {1} games ({2} of {3}), but some failed",
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
                        "Restored saves for {0} games ({1})",
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
                        "Restored saves for {0} of {1} games ({2} of {3}), but some failed",
                        result.Response.Overall.ProcessedGames,
                        result.Response.Overall.TotalGames,
                        AdjustedSize(result.Response.Overall.ProcessedBytes),
                        AdjustedSize(result.Response.Overall.TotalBytes)
                    );
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
                    return "Look up non-PC games by adding this suffix to their names:";
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
    }
}
