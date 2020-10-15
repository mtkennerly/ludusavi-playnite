using Newtonsoft.Json;
using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public bool IgnoreBenignNotifications { get; set; } = false;
        [JsonIgnore]
        public string IgnoreBenignNotifications_Label { get; set; }

        // Parameterless constructor must exist if you want to use LoadPluginSettings method.
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
            OnlyBackupOnGameStoppedIfPc_Label = translator.OnlyBackupOnGameStoppedIfPc_Label();
            AddSuffixForNonPcGameNames_Label = translator.AddSuffixForNonPcGameNames_Label();
            RetryNonPcGamesWithoutSuffix_Label = translator.RetryNonPcGamesWithoutSuffix_Label();
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
                OnlyBackupOnGameStoppedIfPc = savedSettings.OnlyBackupOnGameStoppedIfPc;
                AddSuffixForNonPcGameNames = savedSettings.AddSuffixForNonPcGameNames;
                SuffixForNonPcGameNames = savedSettings.SuffixForNonPcGameNames;
                RetryNonPcGamesWithoutSuffix = savedSettings.RetryNonPcGamesWithoutSuffix;
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
