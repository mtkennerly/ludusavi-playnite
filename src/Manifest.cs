using Newtonsoft.Json;
using System.Collections.Generic;

namespace LudusaviPlaynite.Manifest
{
    public class Data : Dictionary<string, Game>
    { }

    public struct Game
    {
        [JsonProperty("files")]
        public Dictionary<string, object> Files;
        [JsonProperty("registry")]
        public Dictionary<string, object> Registry;
        [JsonProperty("steam")]
        public Steam Steam;
    }

    public class Steam
    {
        [JsonProperty("id")]
        public int? Id;
    }
}
