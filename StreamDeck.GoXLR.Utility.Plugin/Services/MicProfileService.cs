using System.Text.RegularExpressions;

namespace StreamDeck.GoXLR.Utility.Plugin.Services
{
    public class MicProfileService
    {
        private readonly GoXlrUtilityClient _client;

        private List<string> _micProfiles = new ();
        private string? _selectedMicProfile;

        public EventHandler<string> SelectedMicProfileChanged;
        public EventHandler MicProfilesChanged;

        public MicProfileService(GoXlrUtilityClient client)
        {
            _client = client;
            _client.PatchEvent += IsProfileNamePatchEvent;
            _client.PatchEvent += IsProfileNameIndexPatchEvent;
            _client.PatchEvent += IsProfileListPatchEvent;
        }

        private void IsProfileNamePatchEvent(object? sender, Patch patch)
        {
            if (!Regex.IsMatch(patch.Path, @"/mixers/(?<serial>\w+)/mic_profile_name"))
                return;

            var profile = patch.Value?.ToObject<string>();
            if (profile is null)
                return;

            _selectedMicProfile = profile;
            SelectedMicProfileChanged?.Invoke(this, profile);
        }

        private void IsProfileNameIndexPatchEvent(object? sender, Patch patch)
        {
            var match = Regex.Match(patch.Path, @"/files/mic_profiles/(?<index>\d+)");
            if (!match.Success)
                return;

            var stringIndex = match.Groups["index"].Value;
            if (!int.TryParse(stringIndex, out var index))
                return;

            var value = patch.Value?.ToObject<string>();
            switch (patch.Op)
            {
                case OpPatchEnum.Add:
                    if (value is not null)
                        _micProfiles.Add(value);
                    break;
                case OpPatchEnum.Remove:
                    _micProfiles.RemoveAt(index);
                    break;
                case OpPatchEnum.Replace:
                    if (value is not null)
                        _micProfiles[index] = value;
                    break;
            }
            MicProfilesChanged?.Invoke(this, EventArgs.Empty);
        }

        private void IsProfileListPatchEvent(object? sender, Patch patch)
        {
            if (!Regex.IsMatch(patch.Path, @"/files/mic_profiles"))
                return;

            _micProfiles = patch.Value?.ToObject<List<string>>() ?? new List<string>();
        }

        public string? GetMicProfile()
        {
            return _selectedMicProfile;
        }

        public void SetMicProfile(string micProfileName)
        {
            _selectedMicProfile = micProfileName;
            var command = new
            {
                LoadMicProfile = micProfileName
            };

            _client.SendCommand(command);
        }

        public string[] GetMicProfiles()
        {
            return _micProfiles.ToArray();
        }
    }
}
