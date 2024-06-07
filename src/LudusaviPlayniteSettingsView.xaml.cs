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

        public LudusaviPlayniteSettingsView(LudusaviPlaynite plugin, Translator translator)
        {
            InitializeComponent();
            this.plugin = plugin;
            this.translator = translator;
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
            if (!Etc.OpenDir(plugin.settings.BackupPath))
            {
                this.plugin.ShowError(this.translator.CannotOpenFolder());
            }
        }
    }
}
