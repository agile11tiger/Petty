using SpeechEngine.Audio;

namespace SpeechEngine;

public interface ISpeechEngineParametersService
{
    Task<bool> CheckBeforeDownloadingSpeechModelAsync();
    void UpdateDownloadingBar(SpeechModelDownloadingProgressBar downloadingProgressBar);
    Task SendTryLaterAsync();
}
