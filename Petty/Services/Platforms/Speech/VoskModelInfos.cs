using System.Globalization;
namespace Petty.Services.Platforms.Speech;

/// <summary>
/// https://alphacephei.com/vosk/models
/// </summary>
public static class VoskModelInfos
{
    private static readonly Dictionary<string, VoskModelInfo> _models = new()
    {
        //{ "en", new VoskModelInfo
        //{
        //    ByteSize = 214028616,
        //    ArchiveByteSize = 130557655,
        //    TwoLetterISOLanguageName = "en",
        //    Notes = "Big US English model with dynamic graph (about 128 MB)",
        //    Uri = "https://alphacephei.com/vosk/models/vosk-model-en-us-0.22-lgraph.zip",
        //}},
        { "en", new VoskModelInfo
        {
            ByteSize = 70898967,
            ArchiveByteSize = 41205931,
            TwoLetterISOLanguageName = "en",
            Notes = "Lightweight wideband model for Android and RPi (about 40 MB)",
            Uri = "https://alphacephei.com/vosk/models/vosk-model-small-en-us-0.15.zip",
        }},
        { "ru", new VoskModelInfo
        {
            ByteSize = 91289240,
            ArchiveByteSize = 46236750,
            TwoLetterISOLanguageName = "ru",
            Notes = "Lightweight wideband model for Android and RPi (45 MB)",
            Uri = "https://alphacephei.com/vosk/models/vosk-model-small-ru-0.22.zip",
        }},
    };

    public static VoskModelInfo GetModelInfo()
    {
        if (_models.TryGetValue(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, out VoskModelInfo model))
            return model;

        return _models["en"];
    }

    public class VoskModelInfo
    {
        public string Uri { get; set; }
        public string Notes { get; set; }
        public long ByteSize { get; set; }
        public long ArchiveByteSize { get; set; }
        public string TwoLetterISOLanguageName { get; set; }
        public string Name => Uri[(Uri.LastIndexOf('/') + 1)..^4];
        public string Path => System.IO.Path.Combine(DataPath, Name);
        public string ArchivePath => System.IO.Path.Combine(DataPath, Uri[(Uri.LastIndexOf('/') + 1)..]);
        public string DataPath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    }
}
