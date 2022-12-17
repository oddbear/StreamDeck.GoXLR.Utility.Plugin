using Newtonsoft.Json;

namespace StreamDeck.GoXLR.Utility.Plugin.Settings;

public class MicProfileChangeSettings
{
    [JsonProperty(PropertyName = "micProfileValue")]
    public int MicProfile { get; set; } = 0;

    [JsonProperty(PropertyName = "micProfileValues")]
    public SelectData[] MicProfiles { get; set; } = Array.Empty<SelectData>();
}