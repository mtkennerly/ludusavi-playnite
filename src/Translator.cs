using Playnite.SDK.Models;

namespace LudusaviPlaynite
{
    public enum Language
    {
        English,
    }

    public struct OperationResult
    {
        public Game Game;
    }

    public class Translator
    {
        private Language language;

        public Translator(Language language = Language.English)
        {
            // TODO: Get active language from Playnite API?
            this.language = language;
        }

        public string Launch_Label()
        {
            switch (language)
            {
                default:
                    return "Ludusavi: Launch";
            }
        }

        public string BackUpOneGame_Label()
        {
            switch (language)
            {
                default:
                    return "Ludusavi: Back up save data for last game played";
            }
        }

        public string BackUpOneGame_Confirm(Game game)
        {
            switch (language)
            {
                default:
                    return string.Format("Back up save data for {0}?", game.Name);
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

        public string RestoreOneGame_Label()
        {
            switch (language)
            {
                default:
                    return "Ludusavi: Restore save data for last game played";
            }
        }

        public string RestoreOneGame_Confirm(Game game)
        {
            switch (language)
            {
                default:
                    return string.Format("Restore save data for {0}?", game.Name);
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

        public string BackUpOneGame_Success(OperationResult result)
        {
            switch (language)
            {
                default:
                    return string.Format("Backed up saves for {0}", result.Game.Name);
            }
        }

        public string BackUpOneGame_Failure(OperationResult result)
        {
            switch (language)
            {
                default:
                    return string.Format("Unable to back up saves for {0}", result.Game.Name);
            }
        }

        public string BackUpAllGames_Success(OperationResult result)
        {
            switch (language)
            {
                default:
                    return "Backed up saves for all games";
            }
        }

        public string BackUpAllGames_Failure(OperationResult result)
        {
            switch (language)
            {
                default:
                    return "Unable to back up saves for some games";
            }
        }

        public string RestoreOneGame_Success(OperationResult result)
        {
            switch (language)
            {
                default:
                    return string.Format("Restored saves for {0}", result.Game.Name);
            }
        }

        public string RestoreOneGame_Failure(OperationResult result)
        {
            switch (language)
            {
                default:
                    return string.Format("Unable to back up saves for {0}", result.Game.Name);
            }
        }

        public string RestoreAllGames_Success(OperationResult result)
        {
            switch (language)
            {
                default:
                    return "Restored saves for all games";
            }
        }

        public string RestoreAllGames_Failure(OperationResult result)
        {
            switch (language)
            {
                default:
                    return "Unable to restore saves for some games";
            }
        }

        public string ExecutablePath_Label()
        {
            switch (language)
            {
                default:
                    return "Name or full path of the Ludusavi executable";
            }
        }

        public string BackupPath_Label()
        {
            switch (language)
            {
                default:
                    return "Full path to directory for storing backups";
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
                    return "...but ask first before doing the backup";
            }
        }
    }
}
