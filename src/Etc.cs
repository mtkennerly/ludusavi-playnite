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
        private static Regex homeDir = new Regex("^~");

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
            return homeDir.Replace(path, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)).Replace("/", "\\");
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
}
