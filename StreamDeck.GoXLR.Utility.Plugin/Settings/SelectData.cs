using Newtonsoft.Json;

namespace StreamDeck.GoXLR.Utility.Plugin.Settings;

public class SelectData
{
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "index")]
    public int Index { get; set; }
}