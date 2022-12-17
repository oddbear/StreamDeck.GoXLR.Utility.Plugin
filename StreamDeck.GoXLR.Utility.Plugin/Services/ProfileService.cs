using System.Text.RegularExpressions;

namespace StreamDeck.GoXLR.Utility.Plugin.Services
{
    public class ProfileService
    {
        private readonly GoXlrUtilityClient _client;

        private List<string> _profiles = new ();
        private string? _selectedProfile;

        public EventHandler<string> SelectedProfileChanged;
        public EventHandler ProfilesChanged;

        public ProfileService(GoXlrUtilityClient client)
        {
            _client = client;
            _client.PatchEvent += IsProfileNamePatchEvent;
            _client.PatchEvent += IsProfileNameIndexPatchEvent;
            _client.PatchEvent += IsProfileListPatchEvent;
        }

        private void IsProfileNamePatchEvent(object? sender, Patch patch)
        {
            if (!Regex.IsMatch(patch.Path, @"/mixers/(?<serial>\w+)/profile_name"))
                return;

            var profile = patch.Value?.ToObject<string>();
            if (profile is null)
                return;

            _selectedProfile = profile;
            SelectedProfileChanged?.Invoke(this, profile);
        }

        private void IsProfileNameIndexPatchEvent(object? sender, Patch patch)
        {
            var match = Regex.Match(patch.Path, @"/files/profiles/(?<index>\d+)");
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
                        _profiles.Add(value);
                    break;
                case OpPatchEnum.Remove:
                    _profiles.RemoveAt(index);
                    break;
                case OpPatchEnum.Replace:
                    if (value is not null)
                        _profiles[index] = value;
                    break;
            }
            ProfilesChanged?.Invoke(this, EventArgs.Empty);
        }

        private void IsProfileListPatchEvent(object? sender, Patch patch)
        {
            if (!Regex.IsMatch(patch.Path, @"/files/profiles"))
                return;

            _profiles = patch.Value?.ToObject<List<string>>() ?? new List<string>();
        }

        public string? GetProfile()
        {
            return _selectedProfile;
        }

        public void SetProfile(string profileName)
        {
            _selectedProfile = profileName;
            var command = new
            {
                LoadProfile = profileName
            };

            _client.SendCommand(command);
        }

        public string[] GetProfiles()
        {
            return _profiles.ToArray();
        }
    }
}
