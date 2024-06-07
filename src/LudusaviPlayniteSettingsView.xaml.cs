using System.Windows;
using System.Windows.Controls;

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
