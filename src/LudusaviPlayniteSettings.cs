using Newtonsoft.Json;
using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace LudusaviPlaynite
{
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

        #region MultipleBackupRadioButtonsProperties

        //copies
        private bool threeCopies;
        private bool fiveCopies;
        private bool tenCopies;
        private bool fifteenCopies;

        //interval
        private bool fiveMinutes;
        private bool fifteenMinutes;
        private bool thirtyMinutes;
        private bool sixtyMinutes;

        [JsonIgnore]
        public bool ThreeCopies
        {
            // workaround so the correct radio button is clicked in the UI during initial render
            get => AskNumberOfBackupCopies == 3;
            set
            {
                // set the integer value only when the radio button is clicked
                threeCopies = value;

                if (threeCopies) AskNumberOfBackupCopies = 3;
                NotifyPropertyChanged(nameof(ThreeCopies));
            }
        }

        [JsonIgnore]
        public bool FiveCopies
        {
            get => AskNumberOfBackupCopies == 5;
            set
            {
                fiveCopies = value;

                if (fiveCopies) AskNumberOfBackupCopies = 5;
                NotifyPropertyChanged(nameof(FiveCopies));
            }
        }

        [JsonIgnore]
        public bool TenCopies
        {
            get => AskNumberOfBackupCopies == 10;
            set
            {
                tenCopies = value;

                if (tenCopies) AskNumberOfBackupCopies = 10;
                NotifyPropertyChanged(nameof(TenCopies));
            }
        }

        [JsonIgnore]
        public bool FifteenCopies
        {
            get => AskNumberOfBackupCopies == 15;
            set
            {
                fifteenCopies = value;

                if (fifteenCopies) AskNumberOfBackupCopies = 15;
                NotifyPropertyChanged(nameof(FifteenCopies));
            }
        }

        [JsonIgnore]
        public bool FiveMinutes
        {
            get => AskBackupMinuteInterval == 5;
            set
            {
                fiveMinutes = value;

                if (fiveMinutes) AskBackupMinuteInterval = 5;
                NotifyPropertyChanged(nameof(FiveMinutes));
            }
        }

        [JsonIgnore]
        public bool FifteenMinutes
        {
            get => AskBackupMinuteInterval == 15;
            set
            {
                fifteenMinutes = value;

                if (fifteenMinutes) AskBackupMinuteInterval = 15;
                NotifyPropertyChanged(nameof(FifteenMinutes));
            }
        }

        [JsonIgnore]
        public bool ThirtyMinutes
        {
            get => AskBackupMinuteInterval == 30;
            set
            {
                thirtyMinutes = value;

                if (thirtyMinutes) AskBackupMinuteInterval = 30;
                NotifyPropertyChanged(nameof(ThirtyMinutes));
            }
        }

        [JsonIgnore]
        public bool SixtyMinutes
        {
            get => AskBackupMinuteInterval == 60;
            set
            {
                sixtyMinutes = value;

                if (sixtyMinutes) AskBackupMinuteInterval = 60;
                NotifyPropertyChanged(nameof(SixtyMinutes));
            }
        }

        #endregion

        [JsonIgnore]
        public string BrowseButton_Label { get; set; }
        [JsonIgnore]
        public string OpenButton_Label { get; set; }

        private string executablePath = "ludusavi";
        public string ExecutablePath { get { return executablePath; } set { executablePath = value; NotifyPropertyChanged("ExecutablePath"); } }
        [JsonIgnore]
        public string ExecutablePath_Label { get; set; }

        private string backupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "ludusavi-playnite");
        public string BackupPath { get { return backupPath; } set { backupPath = value; NotifyPropertyChanged("BackupPath"); } }
        [JsonIgnore]
        public string BackupPath_Label { get; set; }

        public bool DoBackupOnGameStopped { get; set; } = true;
        [JsonIgnore]
        public string DoBackupOnGameStopped_Label { get; set; }

        public bool AskBackupOnGameStopped { get; set; } = true;
        [JsonIgnore]
        public string AskBackupOnGameStopped_Label { get; set; }

        public bool AskCreateMultipleBackups { get; set; } = true;
        [JsonIgnore]
        public string AskCreateMultipleBackups_Label { get; set; }

        public int AskNumberOfBackupCopies { get; set; } = 3;
        [JsonIgnore]
        public string AskNumberOfBackupCopies_Label { get; set; }

        public int AskBackupMinuteInterval { get; set; } = 30;
        [JsonIgnore]
        public string AskBackupMinuteInterval_Label { get; set; }

        public bool OnlyBackupOnGameStoppedIfPc { get; set; } = true;
        [JsonIgnore]
        public string OnlyBackupOnGameStoppedIfPc_Label { get; set; }

        public bool AddSuffixForNonPcGameNames { get; set; } = false;
        [JsonIgnore]
        public string AddSuffixForNonPcGameNames_Label { get; set; }

        public string SuffixForNonPcGameNames { get; set; } = " (<platform>)";

        public bool RetryNonPcGamesWithoutSuffix { get; set; } = false;
        [JsonIgnore]
        public string RetryNonPcGamesWithoutSuffix_Label { get; set; }

        public bool DoPlatformBackupOnNonPcGameStopped { get; set; } = false;
        [JsonIgnore]
        public string DoPlatformBackupOnNonPcGameStopped_Label { get; set; }

        public bool AskPlatformBackupOnNonPcGameStopped { get; set; } = true;
        [JsonIgnore]
        public string AskPlatformBackupOnNonPcGameStopped_Label { get; set; }

        public bool IgnoreBenignNotifications { get; set; } = false;
        [JsonIgnore]
        public string IgnoreBenignNotifications_Label { get; set; }

        public LudusaviPlayniteSettings()
        {
        }

        public LudusaviPlayniteSettings(LudusaviPlaynite plugin, Translator translator)
        {
            BrowseButton_Label = translator.BrowseButton();
            OpenButton_Label = translator.OpenButton();
            ExecutablePath_Label = translator.ExecutablePath_Label();
            BackupPath_Label = translator.BackupPath_Label();
            DoBackupOnGameStopped_Label = translator.DoBackupOnGameStopped_Label();
            AskBackupOnGameStopped_Label = translator.AskBackupOnGameStopped_Label();
            AskCreateMultipleBackups_Label = translator.AskCreateMultipleBackups_Label();
            AskNumberOfBackupCopies_Label = translator.AskNumberOfBackupCopies_Label();
            AskBackupMinuteInterval_Label = translator.AskBackupMinuteInterval_Label();
            OnlyBackupOnGameStoppedIfPc_Label = translator.OnlyBackupOnGameStoppedIfPc_Label();
            AddSuffixForNonPcGameNames_Label = translator.AddSuffixForNonPcGameNames_Label();
            RetryNonPcGamesWithoutSuffix_Label = translator.RetryNonPcGamesWithoutSuffix_Label();
            DoPlatformBackupOnNonPcGameStopped_Label = translator.DoPlatformBackupOnNonPcGameStopped_Label();
            AskPlatformBackupOnNonPcGameStopped_Label = translator.AskPlatformBackupOnNonPcGameStopped_Label();
            IgnoreBenignNotifications_Label = translator.IgnoreBenignNotifications_Label();

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
                ExecutablePath = savedSettings.ExecutablePath;
                BackupPath = savedSettings.BackupPath;
                DoBackupOnGameStopped = savedSettings.DoBackupOnGameStopped;
                AskBackupOnGameStopped = savedSettings.AskBackupOnGameStopped;
                AskCreateMultipleBackups = savedSettings.AskCreateMultipleBackups;
                AskNumberOfBackupCopies = savedSettings.AskNumberOfBackupCopies;
                AskBackupMinuteInterval = savedSettings.AskBackupMinuteInterval;
                OnlyBackupOnGameStoppedIfPc = savedSettings.OnlyBackupOnGameStoppedIfPc;
                AddSuffixForNonPcGameNames = savedSettings.AddSuffixForNonPcGameNames;
                SuffixForNonPcGameNames = savedSettings.SuffixForNonPcGameNames;
                RetryNonPcGamesWithoutSuffix = savedSettings.RetryNonPcGamesWithoutSuffix;
                DoPlatformBackupOnNonPcGameStopped = savedSettings.DoPlatformBackupOnNonPcGameStopped;
                AskPlatformBackupOnNonPcGameStopped = savedSettings.AskPlatformBackupOnNonPcGameStopped;
                IgnoreBenignNotifications = savedSettings.IgnoreBenignNotifications;
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
        }

        public bool VerifySettings(out List<string> errors)
        {
            // Code execute when user decides to confirm changes made since BeginEdit was called.
            // Executed before EndEdit is called and EndEdit is not called if false is returned.
            // List of errors is presented to user if verification fails.
            errors = new List<string>();
            return true;
        }
    }
}
