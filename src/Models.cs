using System;

namespace LudusaviPlaynite
{
    public class Backup
    {
        public string name;
        public DateTime when;
    }

    public enum Operation
    {
        Backup,
        Restore,
    }

    public enum Mode
    {
        Backup,
        Backups,
        Find,
        Restore,
        ManifestShow,
        Gui,
    }

    public enum Choice
    {
        Yes,
        Always,
        No,
        Never,
    }

    public static class ChoiceExt
    {
        public static bool Accepted(this Choice choice)
        {
            return choice == Choice.Yes || choice == Choice.Always;
        }
    }

    public struct OperationPreference
    {
        public bool Do;
        public bool Ask;
    }

    public struct OperationPreferences
    {
        public OperationPreference Backup;
        public OperationPreference Restore;
    }

    public struct PlayPreferences
    {
        public OperationPreferences Game;
        public OperationPreferences Platform;
    }

    public static class PlayPreferencesExt
    {
        public static bool ShouldAsk(this PlayPreferences prefs, OperationTiming timing, BackupCriteria criteria, Operation operation)
        {
            var byPref = false;
            switch (criteria)
            {
                case BackupCriteria.Game:
                    switch (operation)
                    {
                        case Operation.Backup:
                            byPref = prefs.Game.Backup.Ask;
                            break;
                        case Operation.Restore:
                            byPref = prefs.Game.Restore.Ask;
                            break;
                    }
                    break;
                case BackupCriteria.Platform:
                    switch (operation)
                    {
                        case Operation.Backup:
                            byPref = prefs.Platform.Backup.Ask;
                            break;
                        case Operation.Restore:
                            byPref = prefs.Platform.Restore.Ask;
                            break;
                    }
                    break;
            }

            switch (timing)
            {
                case OperationTiming.Free:
                    return true;
                case OperationTiming.BeforePlay:
                    return byPref;
                case OperationTiming.DuringPlay:
                    return false;
                case OperationTiming.AfterPlay:
                    return byPref;
                default:
                    return false;
            }
        }
    }

    public struct OperationResult
    {
        public string Name;
        public Cli.Output.Response Response;
    }

    public enum BackupCriteria
    {
        Game,
        Platform,
    }

    public static class BackupCriteriaExt
    {
        public static bool ByGame(this BackupCriteria criteria)
        {
            return criteria == BackupCriteria.Game;
        }

        public static bool ByPlatform(this BackupCriteria criteria)
        {
            return criteria == BackupCriteria.Platform;
        }
    }

    public enum RefreshContext
    {
        Startup,
        EditedConfig,
        ConfiguredTitle,
        CreatedBackup,
    }

    public enum OperationTiming
    {
        Free,
        BeforePlay,
        DuringPlay,
        AfterPlay,
    }

    public struct RestorationError
    {
        public string Message;
        public bool Empty;
    }

    public enum Icon
    {
        Download = '\uEF08',
        UiAdd = '\uEC3E',
        UiEdit = '\uEC55',
        UiFolder = '\uEC5B',
        UiPlay = '\uEC74',
        UiRemove = '\uEC7E',
        UploadAlt = '\uF01C',
    }
}
