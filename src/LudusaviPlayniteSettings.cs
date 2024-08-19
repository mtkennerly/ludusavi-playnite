using Newtonsoft.Json;
using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace LudusaviPlaynite
{
    public enum BackupFormatType
    {
        Simple,
        Zip,
    }

    public enum BackupCompressionType
    {
        None,
        Deflate,
        Bzip2,
        Zstd,
    }

    public class LudusaviPlayniteSettings : ISettings, INotifyPropertyChanged
    {
        private readonly LudusaviPlaynite plugin;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [JsonIgnore]
        public Dictionary<BackupFormatType, string> BackupFormatOptions { get; }

        [JsonIgnore]
        public Dictionary<BackupCompressionType, string> BackupCompressionOptions { get; }

        public bool MigratedTags { get; set; } = false;
        public string SuggestedUpgradeTo { get; set; } = "0.0.0";

        public DateTime CheckedAppUpdate { get; set; } = DateTime.UtcNow;
        public string PresentedAppUpdate { get; set; } = "0.0.0";

        public Dictionary<string, string> AlternativeTitles { get; set; } = new Dictionary<string, string>();

        [JsonIgnore]
        public string BrowseButton_Label { get; set; }
        [JsonIgnore]
        public string OpenButton_Label { get; set; }

        private string executablePath = "ludusavi";
        public string ExecutablePath { get { return executablePath; } set { executablePath = value; NotifyPropertyChanged("ExecutablePath"); } }
        [JsonIgnore]
        public string ExecutablePath_Label { get; set; }

        public bool OverrideBackupPath { get; set; } = true;

        private string backupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "ludusavi-playnite");
        public string BackupPath { get { return backupPath; } set { backupPath = value; NotifyPropertyChanged("BackupPath"); } }
        [JsonIgnore]
        public string BackupPath_Label { get; set; }

        public bool OverrideBackupFormat { get; set; } = false;
        [JsonIgnore]
        public string OverrideBackupFormat_Label { get; set; }

        public BackupFormatType BackupFormat { get; set; } = BackupFormatType.Simple;

        public bool OverrideBackupCompression { get; set; } = false;
        [JsonIgnore]
        public string OverrideBackupCompression_Label { get; set; }

        public BackupCompressionType BackupCompression { get; set; } = BackupCompressionType.Deflate;

        public bool OverrideBackupRetention { get; set; } = false;
        [JsonIgnore]
        public string OverrideBackupRetention_Label { get; set; }

        public byte FullBackupLimit { get; set; } = 2;
        [JsonIgnore]
        public string FullBackupLimit_Label { get; set; }

        public byte DifferentialBackupLimit { get; set; } = 10;
        [JsonIgnore]
        public string DifferentialBackupLimit_Label { get; set; }

        public bool DoRestoreOnGameStarting { get; set; } = false;
        [JsonIgnore]
        public string DoRestoreOnGameStarting_Label { get; set; }

        public bool DoBackupOnGameStopped { get; set; } = true;
        [JsonIgnore]
        public string DoBackupOnGameStopped_Label { get; set; }

        public bool AskBackupOnGameStopped { get; set; } = true;
        [JsonIgnore]
        public string AskBackupOnGameStopped_Label { get; set; }

        public bool OnlyBackupOnGameStoppedIfPc { get; set; } = true;
        [JsonIgnore]
        public string OnlyBackupOnGameStoppedIfPc_Label { get; set; }

        public bool RetryUnrecognizedGameWithNormalization { get; set; } = false;
        [JsonIgnore]
        public string RetryUnrecognizedGameWithNormalization_Label { get; set; }

        public bool AddSuffixForNonPcGameNames { get; set; } = false;
        [JsonIgnore]
        public string AddSuffixForNonPcGameNames_Label { get; set; }

        public string SuffixForNonPcGameNames { get; set; } = " (<platform>)";

        public bool RetryNonPcGamesWithoutSuffix { get; set; } = false;
        [JsonIgnore]
        public string RetryNonPcGamesWithoutSuffix_Label { get; set; }

        public bool DoPlatformRestoreOnNonPcGameStarting { get; set; } = false;
        [JsonIgnore]
        public string DoPlatformRestoreOnNonPcGameStarting_Label { get; set; }

        public bool DoPlatformBackupOnNonPcGameStopped { get; set; } = false;
        [JsonIgnore]
        public string DoPlatformBackupOnNonPcGameStopped_Label { get; set; }

        public bool AskPlatformBackupOnNonPcGameStopped { get; set; } = true;
        [JsonIgnore]
        public string AskPlatformBackupOnNonPcGameStopped_Label { get; set; }

        public bool DoBackupDuringPlay { get; set; }
        [JsonIgnore]
        public string DoBackupDuringPlay_Label { get; set; }

        public bool TagGamesWithBackups { get; set; } = false;
        [JsonIgnore]
        public string TagGamesWithBackups_Label { get; set; }

        public bool TagGamesWithUnknownSaveData { get; set; } = false;
        [JsonIgnore]
        public string TagGamesWithUnknownSaveData_Label { get; set; }

        public double BackupDuringPlayInterval { get; set; } = 30;
        [JsonIgnore]
        public string BackupDuringPlayInterval_Label { get; set; }

        public bool IgnoreBenignNotifications { get; set; } = false;
        [JsonIgnore]
        public string IgnoreBenignNotifications_Label { get; set; }

        public bool CheckAppUpdate { get; set; } = true;
        [JsonIgnore]
        public string CheckAppUpdate_Label { get; set; }

        // Parameterless constructor must exist if you want to use LoadPluginSettings method.
        public LudusaviPlayniteSettings()
        {
        }

        public LudusaviPlayniteSettings(LudusaviPlaynite plugin, Translator translator)
        {
            BackupFormatOptions = new Dictionary<BackupFormatType, string>()
            {
                {BackupFormatType.Simple, translator.OptionSimple()},
                {BackupFormatType.Zip, "Zip"},
            };

            BackupCompressionOptions = new Dictionary<BackupCompressionType, string>()
            {
                {BackupCompressionType.None, translator.OptionNone()},
                {BackupCompressionType.Deflate, "Deflate"},
                {BackupCompressionType.Bzip2, "Bzip2"},
                {BackupCompressionType.Zstd, "Zstd"},
            };

            BrowseButton_Label = translator.BrowseButton();
            OpenButton_Label = translator.OpenButton();
            ExecutablePath_Label = translator.ExecutablePath_Label();
            BackupPath_Label = translator.BackupPath_Label();
            OverrideBackupFormat_Label = translator.OverrideBackupFormat_Label();
            OverrideBackupCompression_Label = translator.OverrideBackupCompression_Label();
            OverrideBackupRetention_Label = translator.OverrideBackupRetention_Label();
            FullBackupLimit_Label = translator.FullBackupLimit_Label();
            DifferentialBackupLimit_Label = translator.DifferentialBackupLimit_Label();
            DoRestoreOnGameStarting_Label = translator.DoRestoreOnGameStarting_Label();
            DoBackupOnGameStopped_Label = translator.DoBackupOnGameStopped_Label();
            AskBackupOnGameStopped_Label = translator.AskBackupOnGameStopped_Label();
            OnlyBackupOnGameStoppedIfPc_Label = translator.OnlyBackupOnGameStoppedIfPc_Label();
            RetryUnrecognizedGameWithNormalization_Label = translator.RetryUnrecognizedGameWithNormalization_Label();
            AddSuffixForNonPcGameNames_Label = translator.AddSuffixForNonPcGameNames_Label();
            RetryNonPcGamesWithoutSuffix_Label = translator.RetryNonPcGamesWithoutSuffix_Label();
            DoPlatformRestoreOnNonPcGameStarting_Label = translator.DoPlatformRestoreOnNonPcGameStarting_Label();
            DoPlatformBackupOnNonPcGameStopped_Label = translator.DoPlatformBackupOnNonPcGameStopped_Label();
            AskPlatformBackupOnNonPcGameStopped_Label = translator.AskPlatformBackupOnNonPcGameStopped_Label();
            DoBackupDuringPlay_Label = translator.DoBackupDuringPlay_Label();
            TagGamesWithBackups_Label = translator.TagGamesWithBackups_Label();
            TagGamesWithUnknownSaveData_Label = translator.TagGamesWithUnknownSaveData_Label();
            BackupDuringPlayInterval_Label = translator.BackupDuringPlayInterval_Label();
            IgnoreBenignNotifications_Label = translator.IgnoreBenignNotifications_Label();
            CheckAppUpdate_Label = translator.CheckAppUpdate_Label();

            // Injecting your plugin instance is required for Save/Load method because Playnite saves data to a location based on what plugin requested the operation.
            this.plugin = plugin;
            Load();
        }

        private void Load()
        {
            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<LudusaviPlayniteSettings>();

            // LoadPluginSettings returns null if not saved data is available.
            if (savedSettings != null)
            {
                MigratedTags = savedSettings.MigratedTags;
                if (savedSettings.SuggestedUpgradeTo != null)
                {
                    SuggestedUpgradeTo = savedSettings.SuggestedUpgradeTo;
                }
                if (savedSettings.CheckedAppUpdate != null)
                {
                    CheckedAppUpdate = savedSettings.CheckedAppUpdate;
                }
                if (savedSettings.PresentedAppUpdate != null)
                {
                    PresentedAppUpdate = savedSettings.PresentedAppUpdate;
                }
                if (savedSettings.AlternativeTitles != null)
                {
                    AlternativeTitles = savedSettings.AlternativeTitles;
                }

                if (savedSettings.ExecutablePath != null)
                {
                    ExecutablePath = savedSettings.ExecutablePath;
                }
                OverrideBackupPath = savedSettings.OverrideBackupPath;
                if (savedSettings.BackupPath != null)
                {
                    BackupPath = savedSettings.BackupPath;
                }

                OverrideBackupFormat = savedSettings.OverrideBackupFormat;
                BackupFormat = savedSettings.BackupFormat;
                OverrideBackupCompression = savedSettings.OverrideBackupCompression;
                BackupCompression = savedSettings.BackupCompression;
                OverrideBackupRetention = savedSettings.OverrideBackupRetention;
                FullBackupLimit = savedSettings.FullBackupLimit;
                DifferentialBackupLimit = savedSettings.DifferentialBackupLimit;

                DoBackupOnGameStopped = savedSettings.DoBackupOnGameStopped;
                DoRestoreOnGameStarting = savedSettings.DoRestoreOnGameStarting;
                AskBackupOnGameStopped = savedSettings.AskBackupOnGameStopped;
                OnlyBackupOnGameStoppedIfPc = savedSettings.OnlyBackupOnGameStoppedIfPc;
                RetryUnrecognizedGameWithNormalization = savedSettings.RetryUnrecognizedGameWithNormalization;
                AddSuffixForNonPcGameNames = savedSettings.AddSuffixForNonPcGameNames;
                if (savedSettings.SuffixForNonPcGameNames != null)
                {
                    SuffixForNonPcGameNames = savedSettings.SuffixForNonPcGameNames;
                }
                RetryNonPcGamesWithoutSuffix = savedSettings.RetryNonPcGamesWithoutSuffix;
                DoPlatformRestoreOnNonPcGameStarting = savedSettings.DoPlatformRestoreOnNonPcGameStarting;
                DoPlatformBackupOnNonPcGameStopped = savedSettings.DoPlatformBackupOnNonPcGameStopped;
                AskPlatformBackupOnNonPcGameStopped = savedSettings.AskPlatformBackupOnNonPcGameStopped;
                DoBackupDuringPlay = savedSettings.DoBackupDuringPlay;
                TagGamesWithBackups = savedSettings.TagGamesWithBackups;
                TagGamesWithUnknownSaveData = savedSettings.TagGamesWithUnknownSaveData;
                BackupDuringPlayInterval = savedSettings.BackupDuringPlayInterval;
                IgnoreBenignNotifications = savedSettings.IgnoreBenignNotifications;
                CheckAppUpdate = savedSettings.CheckAppUpdate;
            }
            else
            {
                OverrideBackupPath = false;
            }
        }

        public void BeginEdit()
        {
            // Code executed when settings view is opened and user starts editing values.
        }

        public void CancelEdit()
        {
            // Code executed when user decides to cancel any changes made since BeginEdit was called.
            // This method should revert any changes made to Option1 and Option2.
            Load();
        }

        public void EndEdit()
        {
            // Code executed when user decides to confirm changes made since BeginEdit was called.
            // This method should save settings made to Option1 and Option2.
            plugin.SavePluginSettings(this);
            this.plugin.Refresh(RefreshContext.EditedConfig);
        }

        public bool VerifySettings(out List<string> errors)
        {
            // Code execute when user decides to confirm changes made since BeginEdit was called.
            // Executed before EndEdit is called and EndEdit is not called if false is returned.
            // List of errors is presented to user if verification fails.
            errors = new List<string>();
            return true;
        }

        public string GetGameName(Game game)
        {
            if (!Etc.IsOnPc(game) && this.AddSuffixForNonPcGameNames)
            {
                return string.Format("{0}{1}", game.Name, this.SuffixForNonPcGameNames.Replace("<platform>", game.Platforms[0].Name));
            }
            else
            {
                return game.Name;
            }
        }

        public string GetGameNameWithAlt(Game game)
        {
            var alt = AlternativeTitle(game);
            if (alt != null)
            {
                return alt;
            }
            else
            {
                return GetGameName(game);
            }
        }

        public string AlternativeTitle(Game game)
        {
            return Etc.GetDictValue(this.AlternativeTitles, game.Name, null);
        }

        public string GetDisplayName(Game game, BackupCriteria criteria)
        {
            switch (criteria)
            {
                case BackupCriteria.Game:
                    return GetGameName(game);
                case BackupCriteria.Platform:
                    return Etc.GetGamePlatform(game)?.Name ?? "unknown platform";
                default:
                    throw new InvalidOperationException(String.Format("GetDisplayName got unexpected criteria: {0}", criteria));
            }
        }

        public PlayPreferences GetPlayPreferences(Game game)
        {
            if (Etc.ShouldSkipGame(game))
            {
                return new PlayPreferences();
            }

            var gameBackupDo = (this.DoBackupOnGameStopped || Etc.HasTag(game, Tags.GAME_BACKUP) || Etc.HasTag(game, Tags.GAME_BACKUP_AND_RESTORE))
                && !Etc.HasTag(game, Tags.GAME_NO_BACKUP)
                && (Etc.IsOnPc(game) || !this.OnlyBackupOnGameStoppedIfPc || Etc.HasTag(game, Tags.GAME_BACKUP) || Etc.HasTag(game, Tags.GAME_BACKUP_AND_RESTORE));
            var platformBackupDo = (this.DoPlatformBackupOnNonPcGameStopped || Etc.HasTag(game, Tags.PLATFORM_BACKUP) || Etc.HasTag(game, Tags.PLATFORM_BACKUP_AND_RESTORE))
                && !Etc.HasTag(game, Tags.PLATFORM_NO_BACKUP)
                && !Etc.IsOnPc(game)
                && Etc.GetGamePlatform(game) != null;

            var prefs = new PlayPreferences
            {
                Game = new OperationPreferences
                {
                    Backup = new OperationPreference
                    {
                        Do = gameBackupDo,
                        Ask = this.AskBackupOnGameStopped && !Etc.HasTag(game, Tags.GAME_BACKUP) && !Etc.HasTag(game, Tags.GAME_BACKUP_AND_RESTORE),
                    },
                    Restore = new OperationPreference
                    {
                        Do = gameBackupDo
                            && (this.DoRestoreOnGameStarting || Etc.HasTag(game, Tags.GAME_BACKUP_AND_RESTORE))
                            && !Etc.HasTag(game, Tags.GAME_NO_RESTORE),
                        Ask = this.AskBackupOnGameStopped && !Etc.HasTag(game, Tags.GAME_BACKUP_AND_RESTORE),
                    },
                },
                Platform = new OperationPreferences
                {
                    Backup = new OperationPreference
                    {
                        Do = platformBackupDo,
                        Ask = this.AskPlatformBackupOnNonPcGameStopped && !Etc.HasTag(game, Tags.PLATFORM_BACKUP) && !Etc.HasTag(game, Tags.PLATFORM_BACKUP_AND_RESTORE),
                    },
                    Restore = new OperationPreference
                    {
                        Do = platformBackupDo
                            && (this.DoPlatformRestoreOnNonPcGameStarting || Etc.HasTag(game, Tags.PLATFORM_BACKUP_AND_RESTORE))
                            && !Etc.HasTag(game, Tags.PLATFORM_NO_RESTORE),
                        Ask = this.AskPlatformBackupOnNonPcGameStopped && !Etc.HasTag(game, Tags.PLATFORM_BACKUP_AND_RESTORE),
                    },
                },
            };

            return prefs;
        }
    }
}
