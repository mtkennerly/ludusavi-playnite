using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LudusaviPlaynite
{
    /// <summary>
    /// Interact with the user (e.g., ask confirmation) and/or the Playnite API (e.g., edit tags).
    /// </summary>
    public class Interactor
    {
        IPlayniteAPI PlayniteApi;
        LudusaviPlayniteSettings settings;
        Translator translator;

        private static HashSet<string> addedIconResources = new HashSet<string>();

        public Interactor(IPlayniteAPI api, LudusaviPlayniteSettings settings, Translator translator)
        {
            this.PlayniteApi = api;
            this.settings = settings;
            this.translator = translator;
        }

        public bool UserConsents(string message)
        {
            var choice = PlayniteApi.Dialogs.ShowMessage(message, "", System.Windows.MessageBoxButton.YesNo);
            return choice == MessageBoxResult.Yes;
        }

        public Choice AskUser(string message)
        {
            var yes = new MessageBoxOption(translator.YesButton(), true, false);
            var always = new MessageBoxOption(translator.YesRememberedButton(), false, false);
            var no = new MessageBoxOption(translator.NoButton(), false, false);
            var never = new MessageBoxOption(translator.NoRememberedButton(), false, false);

            var choice = PlayniteApi.Dialogs.ShowMessage(
                message,
                "",
                MessageBoxImage.None,
                new List<MessageBoxOption> { always, never, yes, no }
            );

            if (choice == yes)
            {
                return Choice.Yes;
            }
            else if (choice == always)
            {
                return Choice.Always;
            }
            else if (choice == no)
            {
                return Choice.No;
            }
            else if (choice == never)
            {
                return Choice.Never;
            }
            else
            {
                throw new InvalidOperationException(String.Format("AskUser got unexpected answer: {0}", choice.Title));
            }
        }

        public void NotifyInfo(string message)
        {
            NotifyInfo(message, () => { });
        }

        public void NotifyInfo(string message, Action action)
        {
            if (settings.IgnoreBenignNotifications)
            {
                return;
            }
            PlayniteApi.Notifications.Add(new NotificationMessage(Guid.NewGuid().ToString(), message, NotificationType.Info, action));
        }

        public void NotifyInfo(string message, OperationTiming timing)
        {
            if (timing == OperationTiming.DuringPlay)
            {
                return;
            }
            else
            {
                NotifyInfo(message);
            }
        }

        public void NotifyError(string message)
        {
            NotifyError(message, () => { });
        }

        public void NotifyError(string message, Action action)
        {
            PlayniteApi.Notifications.Add(new NotificationMessage(Guid.NewGuid().ToString(), message, NotificationType.Error, action));
        }

        public void NotifyError(string message, OperationTiming timing)
        {
            if (timing == OperationTiming.DuringPlay)
            {
                return;
            }
            else
            {
                NotifyError(message);
            }
        }

        public void ShowError(string message)
        {
            PlayniteApi.Dialogs.ShowErrorMessage(message, translator.Ludusavi());
        }

        public bool AddTag(Game game, string tagName)
        {
            var dbTag = PlayniteApi.Database.Tags.FirstOrDefault(tag => tag.Name == tagName);
            if (dbTag == null)
            {
                dbTag = PlayniteApi.Database.Tags.Add(tagName);
            }

            var dbGame = PlayniteApi.Database.Games[game.Id];
            if (dbGame.TagIds == null)
            {
                dbGame.TagIds = new List<Guid>();
            }
            var added = dbGame.TagIds.AddMissing(dbTag.Id);
            if (added)
                PlayniteApi.Database.Games.Update(dbGame);
            return added;
        }

        public bool RemoveTag(Game game, string tagName)
        {
            if (game.Tags == null || game.Tags.All(tag => tag.Name != tagName))
            {
                return false;
            }

            var dbTag = PlayniteApi.Database.Tags.FirstOrDefault(tag => tag.Name == tagName);
            if (dbTag == null)
            {
                return false;
            }

            var dbGame = PlayniteApi.Database.Games[game.Id];
            if (dbGame.TagIds == null)
            {
                return false;
            }
            var removed = dbGame.TagIds.RemoveAll(id => id == dbTag.Id) > 0;
            if (removed)
                PlayniteApi.Database.Games.Update(dbGame);
            return removed;
        }

        public void UpdateTagsForChoice(Game game, Choice choice, string alwaysTag, string neverTag, string fallbackTag = null)
        {
            if (choice == Choice.Always)
            {
                if (fallbackTag != null)
                {
                    RemoveTag(game, fallbackTag);
                }
                AddTag(game, alwaysTag);
            }
            else if (choice == Choice.Never)
            {
                if (fallbackTag != null && Etc.HasTag(game, alwaysTag))
                {
                    AddTag(game, fallbackTag);
                }
                RemoveTag(game, alwaysTag);
                AddTag(game, neverTag);
            }
        }

        private void AddIconResource(string key, char character)
        {
            AddIconResource(key, character.ToString());
        }

        private void AddIconResource(string key, string text)
        {
            if (addedIconResources.Contains(key))
            {
                return;
            }

            if (Application.Current.Resources.Contains(key))
            {
                return;
            }

            Application.Current.Resources.Add(key, new TextBlock
            {
                Text = text,
                FontSize = 16,
                FontFamily = ResourceProvider.GetResource("FontIcoFont") as FontFamily
            });

            addedIconResources.Add(key);
        }

        private string GetIconResource(char character)
        {
            var key = $"IcoFontResource - {character}";
            AddIconResource(key, character);
            return key;
        }

        public string GetIcon(Icon icon)
        {
            return GetIconResource((char)icon);
        }
    }
}
