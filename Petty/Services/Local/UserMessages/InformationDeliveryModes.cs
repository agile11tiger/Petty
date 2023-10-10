namespace Petty.Services.Local.UserMessages
{
    [Flags]
    public enum InformationDeliveryModes
    {
        VoiceAlert = 1,
        DisplayAlertInApp = 2,
        DisplayAlertOutsideApp = 4,
        DisplayAlertOutsideAppOnLockedScreen = 8,
    }
}
