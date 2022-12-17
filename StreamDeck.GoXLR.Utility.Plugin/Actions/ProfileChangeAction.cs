using System.Diagnostics;
using SharpDeck;
using StreamDeck.GoXLR.Utility.Plugin.Services;
using StreamDeck.GoXLR.Utility.Plugin.Settings;

namespace StreamDeck.GoXLR.Utility.Plugin.Actions
{
    [StreamDeckAction("com.oddbear.goxlr.utility.profilechange")]
    public class PresetChangeAction : ActionBase<ProfileChangeSettings>
    {
        private readonly ProfileService _profileService;

        public PresetChangeAction(
            ProfileService profileService)
        {
            _profileService = profileService;
        }

        protected override void RegisterCallbacks()
        {
            _profileService.SelectedProfileChanged += SelectedProfileChanged;
            _profileService.ProfilesChanged += ProfilesChanged;
            ProfilesChanged(this, EventArgs.Empty);
        }

        protected override void UnregisterCallbacks()
        {
#pragma warning disable CS8601
            _profileService.SelectedProfileChanged -= SelectedProfileChanged;
            _profileService.ProfilesChanged -= ProfilesChanged;
#pragma warning restore CS8601
        }

        private async void ProfilesChanged(object? sender, EventArgs args)
        {
            try
            {
                var data = _profileService.GetProfiles()
                    .Select((profile, index) => new SelectData { Index = index, Name = profile })
                    .ToArray();

                var index = data.FirstOrDefault(selectData => selectData.Name == _profileService.GetProfile())?.Index ?? 0;

                var settings = new ProfileChangeSettings
                {
                    Profile = index,
                    Profiles = data
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
            _profileService.SetProfile(profileName);
        }

        protected override bool GetButtonState()
        {
            var profileName = GetButtonAssignedProfile();
            return _profileService.GetProfile() == profileName;
        }

        protected override async Task SettingsChanged()
        {
            await UpdatePresetNameTitle();
        }
        
        private async void SelectedProfileChanged(object? sender, string profileName)
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
            var index = _settings.Profile;
            var profileName = _profileService.GetProfiles()[index];
            return profileName;
        }
    }
}
