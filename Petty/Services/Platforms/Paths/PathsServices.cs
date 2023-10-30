namespace Petty.Services.Platforms.Paths;

public partial class PathsService : Service
{
#if !ANDROID
    public static string PicturesPath => throw new NotImplementedException();
    public static string MONOTYPE_CORSIVA_PATH => throw new NotImplementedException();
    public static string ScreenshotsPath => throw new NotImplementedException();
    public static string VideoPath => throw new NotImplementedException();
#endif
}
