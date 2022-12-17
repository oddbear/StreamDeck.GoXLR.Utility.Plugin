using SharpDeck;
using System.Diagnostics;
using StreamDeck.GoXLR.Utility.Plugin.Settings;
using StreamDeck.GoXLR.Utility.Plugin.Enums;
using StreamDeck.GoXLR.Utility.Plugin.Services;

namespace StreamDeck.GoXLR.Utility.Plugin.Actions
{
    [StreamDeckAction("com.oddbear.goxlr.utility.routechange")]
    public class RouteChangeAction : ActionBase<RouteChangeSettings>
    {
        private readonly RoutingService _routingService;

        public RouteChangeAction(
            RoutingService routingService)
        {
            _routingService = routingService;
        }

        protected override void RegisterCallbacks()
        {
            _routingService.RoutingUpdated += RouteUpdated;
        }

        protected override void UnregisterCallbacks()
        {
            _routingService.RoutingUpdated -= RouteUpdated;
        }

        protected override void OnButtonPress()
        {
            _routingService.SetRouting(_settings.Input, _settings.Output, _settings.Action);
        }

        protected override bool GetButtonState()
            => _routingService.GetState(_settings.Input, _settings.Output);

        protected override async Task SettingsChanged()
        {
            await UpdateInputImage();
            await UpdateOutputTitle();
        }

        private async void RouteUpdated(object? sender, (InputDevice input, OutputDevice output) route)
        {
            try
            {
                var currentRoute = (_settings.Input, _settings.Output);
                
                if (currentRoute != route)
                    return;
                
                await RefreshState();
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
            await SetTitleAsync($"{_settings.Input}\r\n{_settings.Output}");
        }
    }
}
