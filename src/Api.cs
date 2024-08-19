using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Playnite.SDK;
using Playnite.SDK.Models;
using System.Linq;

namespace LudusaviPlaynite.Api
{
    /// <summary>
    /// Used for running Ludusavi's `api` command.
    /// </summary>
    public class Runner
    {
        ILogger logger;
        LudusaviPlayniteSettings settings;
        Input input;

        public Runner(ILogger logger, LudusaviPlayniteSettings settings)
        {
            this.logger = logger;
            this.settings = settings;

            this.input = new Input
            {
                config = new Config
                {
                    BackupPath = settings.OverrideBackupPath ? settings.BackupPath : null,
                },
                requests = new List<Request> { },
            };
        }

        public (int, Output?) Invoke()
        {
            var (code, stdout) = this.InvokeDirect();

            Output? response;
            try
            {
                response = JsonConvert.DeserializeObject<Output>(stdout);
                logger.Debug(string.Format("Ludusavi exited with {0} and valid JSON content", code));
            }
            catch (Exception e)
            {
                response = null;
                logger.Debug(e, string.Format("Ludusavi exited with {0} and invalid JSON content", code));
            }

            return (code, response);
        }

        public (int, string) InvokeDirect(bool standalone = false)
        {
            var json = JsonConvert.SerializeObject(this.input);
            var args = "api";
            logger.Debug(string.Format(
                "Running Ludusavi API with {0} requests (findTitle: {1}, checkAppUpdate: {2})",
                this.input.requests.Count,
                this.input.requests.Count(r => r.findTitle != null),
                this.input.requests.Count(r => r.checkAppUpdate != null)
            ));

            try
            {
                var (code, stdout) = Etc.RunCommand(settings.ExecutablePath.Trim(), args, json);
                if (standalone)
                {
                    logger.Debug(string.Format("Ludusavi exited with {0}", code));
                }
                return (code, stdout);
            }
            catch (Exception e)
            {
                logger.Debug(e, "Ludusavi could not be executed");
                return (-1, null);
            }
        }

        public void FindTitle(Game game)
        {
            var names = new List<string> { settings.GetGameNameWithAlt(game) };
            int? steamId = null;
            var normalized = false;

            if (settings.AlternativeTitle(game) == null)
            {
                // The Steam ID would take priority over an alt title.

                steamId = Etc.SteamId(game);
                normalized = settings.RetryUnrecognizedGameWithNormalization;

                if (!Etc.IsOnPc(game) && settings.RetryNonPcGamesWithoutSuffix)
                {
                    names.Add(game.Name);
                }
            }

            var inner = new Requests.FindTitle
            {
                names = names,
                steamId = steamId,
                normalized = normalized,
            };

            var request = new Request
            {
                findTitle = inner,
            };
            this.input.requests.Add(request);
        }

        public void CheckAppUpdate()
        {
            var inner = new Requests.CheckAppUpdate { };

            var request = new Request
            {
                checkAppUpdate = inner,
            };
            this.input.requests.Add(request);
        }
    }

    public struct Input
    {
        public Config? config;
        public List<Request> requests;
    }

    public struct Config
    {
        [JsonProperty("backupPath")]
        public string BackupPath;
    }

    public struct Request
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Requests.FindTitle? findTitle;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Requests.CheckAppUpdate? checkAppUpdate;
    }

    public struct Output
    {
        public List<Response> responses;
    }

    public struct Response
    {
        public Responses.Error? error;
        public Responses.FindTitle? findTitle;
        public Responses.CheckAppUpdate? checkAppUpdate;
    }

    namespace Requests
    {
        public struct FindTitle
        {
            public List<string> names;
            public int? steamId;
            public bool normalized;
        }

        public struct CheckAppUpdate
        { }
    }

    namespace Responses
    {
        public struct Error
        {
            public string message;
        }

        public struct AppUpdate
        {
            public string url;
            public string version;
        }

        public struct FindTitle
        {
            public List<string> titles;
        }

        public struct CheckAppUpdate
        {
            public AppUpdate? update;
        }
    }
}
