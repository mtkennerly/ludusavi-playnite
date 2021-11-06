using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace LudusaviPlaynite
{
    public partial class LudusaviPlayniteSettingsView : UserControl
    {
        private LudusaviPlaynite plugin;
        private Translator translator;
        private Regex homeDir = new Regex("^~");

        public LudusaviPlayniteSettingsView(LudusaviPlaynite plugin, Translator translator)
        {
            this.plugin = plugin;
            this.translator = translator;
            InitializeComponent();
            DataContext = plugin.settings;
        }

        private string NormalizePath(string path)
        {
            return homeDir.Replace(path, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)).Replace("/", "\\");
        }

        public void OnBrowseExecutablePath(object sender, RoutedEventArgs e)
        {
            var choice = this.plugin.PlayniteApi.Dialogs.SelectFile(translator.SelectFileExecutableFilter());
            if (choice.Length > 0)
            {
                this.plugin.settings.ExecutablePath = choice;
            }
        }

        public void OnBrowseBackupPath(object sender, RoutedEventArgs e)
        {
            var choice = this.plugin.PlayniteApi.Dialogs.SelectFolder();
            if (choice.Length > 0)
            {
                this.plugin.settings.BackupPath = choice;
            }
        }

        public void OnOpenBackupPath(object sender, RoutedEventArgs e)
        {
            Process.Start(NormalizePath(plugin.settings.BackupPath));
        }

        private void CreateMultipleBackups_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)CheckBox_CreateMultipleBackups.IsChecked)
            {
                Panel_BackupCopies.IsEnabled = true;
                Panel_BackupInterval.IsEnabled = true;
            }
            else
            {
                Panel_BackupCopies.IsEnabled = false;
                Panel_BackupInterval.IsEnabled = false;
            }
        }
    }
}
