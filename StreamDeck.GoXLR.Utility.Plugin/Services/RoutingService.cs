using StreamDeck.GoXLR.Utility.Plugin.Enums;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace StreamDeck.GoXLR.Utility.Plugin.Services
{
    public class RoutingService
    {
        private readonly GoXlrUtilityClient _client;
        private readonly Dictionary<(InputDevice, OutputDevice), bool> _routingTable = new();

        public event EventHandler<(InputDevice, OutputDevice)> RoutingUpdated;

        public RoutingService(GoXlrUtilityClient client)
        {
            _client = client;
            _client.PatchEvent += RouteUpdated;

            foreach (var input in Enum.GetValues<InputDevice>())
                foreach (var output in Enum.GetValues<OutputDevice>())
                    _routingTable[(input, output)] = false;
        }

        private void RouteUpdated(object? sender, Patch patch)
        {
            try
            {
                var match = Regex.Match(patch.Path, @"/mixers/(?<serial>\w+)/router/(?<input>\w+)/(?<output>\w+)");
                if (!match.Success)
                    return;

                var stringInput = match.Groups["input"].Value;
                var stringOutput = match.Groups["output"].Value;

                if (!Enum.TryParse<InputDevice>(stringInput, out var input))
                    return;

                if (!Enum.TryParse<OutputDevice>(stringOutput, out var output))
                    return;

                _routingTable[(input, output)] = patch.Value.ToObject<bool>();

                RoutingUpdated?.Invoke(this, (input, output));
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
            }
        }

        public void SetRouting(InputDevice input, OutputDevice output, BoolenValue action)
        {
            var state = GetNewState(input, output, action);

            var command = new
            {
                SetRouter = new object[]
                {
                    input.ToString(),
                    output.ToString(),
                    state
                }
            };

            _client.SendCommand(command);
        }

        public bool GetState(InputDevice input, OutputDevice output)
            => _routingTable[(input, output)];

        private bool GetNewState(InputDevice input, OutputDevice output, BoolenValue action)
            => action switch
            {
                BoolenValue.On => true,
                BoolenValue.Off => false,
                _ => !_routingTable[(input, output)]
            };
    }
}
