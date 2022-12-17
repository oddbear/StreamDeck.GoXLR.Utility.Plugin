using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using StreamDeck.GoXLR.Utility.Plugin.Enums;

namespace StreamDeck.GoXLR.Utility.Plugin.Settings
{
    public class RouteChangeSettings
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "inputValue")]
        public InputDevice Input { get; set; } = InputDevice.Microphone;

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "outputValue")]
        public OutputDevice Output { get; set; } = OutputDevice.Headphones;

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "actionValue")]
        public Value Action { get; set; } = Value.Toggle;
    }
}
