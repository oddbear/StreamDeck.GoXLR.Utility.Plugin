using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using StreamDeck.GoXLR.Utility.Plugin.Enums;

namespace StreamDeck.GoXLR.Utility.Plugin.Settings
{
    public class VolumeChangeSettings
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "channelValue")]
        public ChannelName Channel { get; set; } = ChannelName.System;

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "actionValue")]
        public IntegerValue Action { get; set; } = IntegerValue.Set;

        [JsonProperty(PropertyName = "valueValue")]
        public int Value { get; set; } = 0;
    }
}
