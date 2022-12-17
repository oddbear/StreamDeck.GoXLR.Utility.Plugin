using Newtonsoft.Json;

namespace StreamDeck.GoXLR.Utility.Plugin.Settings
{
    public class ProfileChangeSettings
    {
        [JsonProperty(PropertyName = "profileValue")]
        public int Profile { get; set; } = 0;
        
        [JsonProperty(PropertyName = "profileValues")]
        public SelectData[] Profiles { get; set; } = Array.Empty<SelectData>();
    }
}
