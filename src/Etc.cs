using Playnite.SDK.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace LudusaviPlaynite
{
    public static class Etc
    {
        public static Version RECOMMENDED_APP_VERSION = new Version(0, 24, 0);
        private static Regex HOME_DIR = new Regex("^~");

        public static bool IsOnSteam(Game game)
        {
            return game.Source?.Name == "Steam"
                || game.PluginId == Guid.Parse("cb91dfc9-b977-43bf-8e70-55f46e410fab");
        }

        public static bool IsOnPc(Game game)
        {
            var pcSpecs = new List<string> { "macintosh", "pc_dos", "pc_linux", "pc_windows" };
            var pcNames = new List<string> { "Macintosh", "PC", "PC (DOS)", "PC (Linux)", "PC (Windows)" };
            return game.Platforms == null
                || game.Platforms.Count == 0
                || game.Platforms.Any(x => pcSpecs.Contains(x.SpecificationId))
                || game.Platforms.Any(x => pcNames.Contains(x.Name));
        }

        public static string NormalizePath(string path)
        {
            return HOME_DIR.Replace(path, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)).Replace("/", "\\");
        }

        public static bool OpenDir(string path)
        {
            try
            {
                Process.Start(NormalizePath(path));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public static class Tags
    {
        private const string PREFIX = "[Ludusavi] ";

        public const string LEGACY_SKIP = "ludusavi-skip";
        public const string SKIP = PREFIX + "Skip";

        public const string GAME_BACKUP = PREFIX + "Game: backup";
        public const string GAME_NO_BACKUP = PREFIX + "Game: no backup";

        public const string GAME_BACKUP_AND_RESTORE = PREFIX + "Game: backup and restore";
        public const string GAME_NO_RESTORE = PREFIX + "Game: no restore";

        public const string PLATFORM_BACKUP = PREFIX + "Platform: backup";
        public const string PLATFORM_NO_BACKUP = PREFIX + "Platform: no backup";

        public const string PLATFORM_BACKUP_AND_RESTORE = PREFIX + "Platform: backup and restore";
        public const string PLATFORM_NO_RESTORE = PREFIX + "Platform: no restore";

        public const string BACKED_UP = PREFIX + "Backed up";
        public const string UNKNOWN_SAVE_DATA = PREFIX + "Unknown save data";

        // Format: {new tag, {conflicting tags}}
        public static Dictionary<string, string[]> CONFLICTS = new Dictionary<string, string[]> {
            {SKIP, new string[] {}},
            {GAME_BACKUP, new string[] {SKIP, GAME_NO_BACKUP}},
            {GAME_NO_BACKUP, new string[] {GAME_BACKUP, GAME_BACKUP_AND_RESTORE}},
            {GAME_BACKUP_AND_RESTORE, new string[] {SKIP, GAME_BACKUP, GAME_NO_BACKUP, GAME_NO_RESTORE}},
            {GAME_NO_RESTORE, new string[] {GAME_BACKUP_AND_RESTORE}},
            {PLATFORM_BACKUP, new string[] {SKIP, PLATFORM_NO_BACKUP}},
            {PLATFORM_NO_BACKUP, new string[] {PLATFORM_BACKUP, PLATFORM_BACKUP_AND_RESTORE}},
            {PLATFORM_BACKUP_AND_RESTORE, new string[] {SKIP, PLATFORM_BACKUP, PLATFORM_NO_BACKUP, PLATFORM_NO_RESTORE}},
            {PLATFORM_NO_RESTORE, new string[] {PLATFORM_BACKUP_AND_RESTORE}},
        };

        // Format: {(new tag, conflicting tag), conflict replacement}
        public static Dictionary<(string, string), string> REPLACEMENTS = new Dictionary<(string, string), string> {
            {(GAME_NO_RESTORE, GAME_BACKUP_AND_RESTORE), GAME_BACKUP},
            {(PLATFORM_NO_RESTORE, PLATFORM_BACKUP_AND_RESTORE), PLATFORM_BACKUP},
        };
    }
}
