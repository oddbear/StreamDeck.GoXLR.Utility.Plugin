using System.Diagnostics;
using SharpDeck;
using StreamDeck.GoXLR.Utility.Plugin.Services;
using StreamDeck.GoXLR.Utility.Plugin.Settings;

namespace StreamDeck.GoXLR.Utility.Plugin.Actions
{
    [StreamDeckAction("com.oddbear.goxlr.utility.micprofilechange")]
    public class MicPresetChangeAction : ActionBase<MicProfileChangeSettings>
    {
        private readonly MicProfileService _micProfileService;

        public MicPresetChangeAction(
            MicProfileService micProfileService)
        {
            _micProfileService = micProfileService;
        }

        protected override void RegisterCallbacks()
        {
            _micProfileService.SelectedMicProfileChanged += SelectedMicProfileChanged;
            _micProfileService.MicProfilesChanged += MicProfilesChanged;
            MicProfilesChanged(this, EventArgs.Empty);
        }

        protected override void UnregisterCallbacks()
        {
#pragma warning disable CS8601
            _micProfileService.SelectedMicProfileChanged -= SelectedMicProfileChanged;
            _micProfileService.MicProfilesChanged -= MicProfilesChanged;
#pragma warning restore CS8601
        }

        private async void MicProfilesChanged(object? sender, EventArgs args)
        {
            try
            {
                var data = _micProfileService.GetMicProfiles()
                    .Select((profile, index) => new SelectData { Index = index, Name = profile })
                    .ToArray();

                var index = data.FirstOrDefault(selectData => selectData.Name == _micProfileService.GetMicProfile())?.Index ?? 0;

                var settings = new MicProfileChangeSettings
                {
                    MicProfile = index,
                    MicProfiles = data
                };

                await SetSettingsAsync(settings);
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
            }
        }

        protected override void OnButtonPress()
        {
            var profileName = GetButtonAssignedProfile();
            _micProfileService.SetMicProfile(profileName);
        }

        protected override bool GetButtonState()
        {
            var profileName = GetButtonAssignedProfile();
            return _micProfileService.GetMicProfile() == profileName;
        }

        protected override async Task SettingsChanged()
        {
            await UpdatePresetNameTitle();
        }
        
        private async void SelectedMicProfileChanged(object? sender, string profileName)
        {
            try
            {
                await RefreshState();
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
            }
        }
        
        private async Task UpdatePresetNameTitle()
        {
            var profileName = GetButtonAssignedProfile();
            await SetTitleAsync(profileName);
        }

        private string GetButtonAssignedProfile()
        {
            var index = _settings.MicProfile;
            var micProfileName = _micProfileService.GetMicProfiles()[index];
            return micProfileName;
        }
    }
}
