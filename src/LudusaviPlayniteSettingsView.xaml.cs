using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Playnite.SDK;
using Playnite.SDK.Plugins;
using Playnite.SDK.Models;

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

        private void ThreeCopies_Checked(object sender, RoutedEventArgs e)
        {
            plugin.settings.AskNumberOfBackupCopies = 3;
        }
        private void FiveCopies_Checked(object sender, RoutedEventArgs e)
        {
            this.
            plugin.settings.AskNumberOfBackupCopies = 5;
        }
        private void TenCopies_Checked(object sender, RoutedEventArgs e)
        {
            plugin.settings.AskNumberOfBackupCopies = 10;
        }
        private void FiveMinutes_Checked(object sender, RoutedEventArgs e)
        {
            plugin.settings.AskBackupMinuteInterval = 5;
        }
        private void FifteenMinutes_Checked(object sender, RoutedEventArgs e)
        {
            plugin.settings.AskBackupMinuteInterval = 15;
        }
        private void ThirtyMinutes_Checked(object sender, RoutedEventArgs e)
        {
            plugin.settings.AskBackupMinuteInterval = 30;
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
