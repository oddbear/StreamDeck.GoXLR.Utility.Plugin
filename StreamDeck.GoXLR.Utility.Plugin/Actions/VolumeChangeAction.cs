using SharpDeck;
using System.Diagnostics;
using StreamDeck.GoXLR.Utility.Plugin.Settings;
using StreamDeck.GoXLR.Utility.Plugin.Enums;
using StreamDeck.GoXLR.Utility.Plugin.Services;

namespace StreamDeck.GoXLR.Utility.Plugin.Actions
{
    [StreamDeckAction("com.oddbear.goxlr.utility.volumechange")]
    public class VolumeChangeAction : ActionBase<VolumeChangeSettings>
    {
        private readonly VolumeChangeService _volumeChangeService;

        public VolumeChangeAction(VolumeChangeService volumeChangeService)
        {
            _volumeChangeService = volumeChangeService;
        }

        protected override void RegisterCallbacks()
        {
            _volumeChangeService.VolumeChangedChanged += VolumeChangedChanged;
        }

        protected override void UnregisterCallbacks()
        {
            _volumeChangeService.VolumeChangedChanged -= VolumeChangedChanged;
        }

        protected override void OnButtonPress()
        {
            _volumeChangeService.SetVolume(_settings.Channel, _settings.Action, _settings.Value);
        }

        protected override bool GetButtonState()
            => true;

        protected override async Task SettingsChanged()
        {
            await UpdateInputImage();
            await UpdateOutputTitle();
        }


        private async void VolumeChangedChanged(object? sender, ChannelName channelName)
        {
            try
            {
                if (channelName != _settings.Channel)
                    return;
                
                await UpdateOutputTitle();
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
            }
        }

        private async Task UpdateInputImage()
        {
            await SetImageAsync(null);
        }

        private async Task UpdateOutputTitle()
        {
            var volume = _volumeChangeService.GetVolume(_settings.Channel);
            await SetTitleAsync($"{volume}");
        }
    }
}
