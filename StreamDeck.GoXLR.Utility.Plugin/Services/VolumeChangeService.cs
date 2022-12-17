using System.Text.RegularExpressions;
using StreamDeck.GoXLR.Utility.Plugin.Enums;

namespace StreamDeck.GoXLR.Utility.Plugin.Services
{
    public class VolumeChangeService
    {
        private readonly GoXlrUtilityClient _client;

        private Dictionary<ChannelName, byte> _volumes = new ();

        public EventHandler<ChannelName> VolumeChangedChanged;

        public VolumeChangeService(GoXlrUtilityClient client)
        {
            _client = client;
            _client.PatchEvent += IsVolumeChangePatchEvent;

            foreach (var channelName in Enum.GetValues<ChannelName>())
                _volumes[channelName] = 0;
        }

        private void IsVolumeChangePatchEvent(object sender, Patch patch)
        {
            var match = Regex.Match(patch.Path, $@"/mixers/(?<serial>\w+)/levels/volumes/(?<channelName>\w+)");
            if (!match.Success)
                return;

            var stringChannelName = match.Groups["channelName"].Value;

            if (!Enum.TryParse<ChannelName>(stringChannelName, out var channelName))
                return;

            _volumes[channelName] = patch.Value?.ToObject<byte>() ?? 0;

            VolumeChangedChanged?.Invoke(this, channelName);
        }

        public byte GetVolume(ChannelName channelName)
        {
            return _volumes[channelName];
        }

        public void SetVolume(ChannelName channelName, IntegerValue action, int diff)
        {
            var volume = GetNewState(channelName, action, diff);

            if (volume > byte.MaxValue) volume = byte.MaxValue;
            if (volume < 0) volume = 0;

            var command = new
            {
                SetVolume = new object[] {
                    channelName.ToString(),
                    volume
                }
            };

            _client.SendCommand(command);
        }

        private int GetNewState(ChannelName channelName, IntegerValue action, int diff)
            => action switch
            {
                IntegerValue.Increment => _volumes[channelName] + diff,
                IntegerValue.Decrement => _volumes[channelName] - diff,
                _ => diff
            };
    }
}
