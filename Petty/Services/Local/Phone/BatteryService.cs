using Petty.Resources.Localization;

namespace Petty.Services.Local.Phone
{
    public class BatteryService : Service, ILifeCycle
    {
        public BatteryService(
            IBattery battery,
            VoiceService voiceService,
            LoggerService loggerService)
            : base(loggerService)
        {
            _battery = battery;
            _voiceService = voiceService;
        }

        private long _isSleeping;
        private readonly IBattery _battery;
        private readonly VoiceService _voiceService;
        public bool IsStarting { get; private set; }
        public int BatteryChargeLevel => (int)_battery.ChargeLevel * 100;

        public void Start()
        {
            if (!IsStarting)
            {
                IsStarting = true;
                _battery.BatteryInfoChanged += Battery_BatteryInfoChanged;
            }
        }

        public void Stop()
        {
            if (IsStarting)
            {
                _battery.BatteryInfoChanged -= Battery_BatteryInfoChanged;
                IsStarting = false;
            }
        }

        private async void Battery_BatteryInfoChanged(object sender, BatteryInfoChangedEventArgs e)
        {
            if (Interlocked.Read(ref _isSleeping) > 0)
                return;

            Interlocked.Increment(ref _isSleeping);
            _ = Task.Run(() =>
            {
                Task.Delay(5000);
                Interlocked.Decrement(ref _isSleeping);
            });

            var speech = e.State switch
            {
                BatteryState.Charging => AppResources.BatteryCharging,
                BatteryState.Discharging => null,
                BatteryState.Full => AppResources.BatteryFull,
                BatteryState.NotCharging => null,
                BatteryState.NotPresent => null,
                BatteryState.Unknown => null,
                _ => null
            };

            if (speech != null)
                await _voiceService.SpeakAsync(speech);
        }
    }
}
