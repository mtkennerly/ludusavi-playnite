using Playnite.SDK.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace LudusaviPlaynite
{
    /// <summary>
    /// Miscellaneous utilities.
    /// </summary>
    public static class Etc
    {
        public static Version RECOMMENDED_APP_VERSION = new Version(0, 24, 0);
        private static Regex HOME_DIR = new Regex("^~");

        public static V GetDictValue<K, V>(Dictionary<K, V> dict, K key, V fallback)
        {
            if (dict == null || key == null)
            {
                return fallback;
            }

            V result;
            var found = dict.TryGetValue(key, out result);
            if (found)
            {
                return result;
            }
            else
            {
                return fallback;
            }
        }

        public static (int, string) RunCommand(string command, string args)
        {
            var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = command;
            p.StartInfo.Arguments = args;
            p.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
            p.Start();

            var stdout = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return (p.ExitCode, stdout);
        }

        public static (int, string) RunCommand(string command, string args, string stdin)
        {
            var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = command;
            p.StartInfo.Arguments = args;
            p.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
            p.Start();

            p.StandardInput.WriteLine(stdin);
            p.StandardInput.Close();

            var stdout = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return (p.ExitCode, stdout);
        }

        public static bool IsOnSteam(Game game)
        {
            return game.Source?.Name == "Steam"
                || game.PluginId == Guid.Parse("cb91dfc9-b977-43bf-8e70-55f46e410fab");
        }

        public static int? SteamId(Game game)
        {
            if (IsOnSteam(game) && int.TryParse(game.GameId, out var id))
            {
                return id;
            }

            return null;
        }

        public static bool TrySteamId(Game game, out int result)
        {
            var id = SteamId(game);
            if (id != null)
            {
                result = (int)id;
                return true;
            }

            result = 0;
            return false;
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

        public static string GetTitleId(Game game)
        {
            return string.Format("{0}:{1}", game.PluginId, game.GameId);
        }

        public static Platform GetGamePlatform(Game game)
        {
            return game?.Platforms?.ElementAtOrDefault(0);
        }

        public static bool ShouldSkipGame(Game game)
        {
            return Etc.HasTag(game, Tags.SKIP);
        }

        public static bool HasTag(Game game, string tagName)
        {
            return game.Tags?.Any(tag => tag.Name == tagName) ?? false;
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

        public static string GetBackupDisplayLine(Cli.Output.Backup backup)
        {
            var ret = backup.When.ToLocalTime().ToString();

            if (!string.IsNullOrEmpty(backup.Os) && backup.Os != "windows")
            {
                ret += string.Format(" [{0}]", backup.Os);
            }
            if (!string.IsNullOrEmpty(backup.Comment))
            {
                var line = "";
                var parts = backup.Comment.Split();

                foreach (var part in backup.Comment.Split())
                {
                    if (line != "")
                    {
                        line += " ";
                    }
                    line += part;
                    if (line.Length > 60)
                    {
                        ret += string.Format("\n    {0}", line);
                        line = "";
                    }
                }
                if (line != "")
                {
                    ret += string.Format("\n    {0}", line);
                }
            }

            return ret;
        }

        public static void OpenLudusaviMainPage()
        {
            try
            {
                RunCommand("cmd.exe", "/c \"start https://github.com/mtkennerly/ludusavi\"");
            }
            catch
            { }
        }

        public static void OpenLudusaviReleasePage()
        {
            try
            {
                RunCommand("cmd.exe", "/c \"start https://github.com/mtkennerly/ludusavi/releases\"");
            }
            catch
            { }
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
