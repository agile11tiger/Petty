namespace Petty.Services.Platforms.Paths
{
    public partial class PathsService
    {
        public const string MONOTYPE_CORSIVA_PATH = "Petty.Resources.Fonts.MonotypeCorsiva.ttf";
        public static string ScreenshotsPath
        {
            get
            {
                var screenshotsPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).Path;

                //Unlikely since this is a base folder.
                if (!Directory.Exists(screenshotsPath))
                    Directory.CreateDirectory(screenshotsPath, UnixFileMode.GroupWrite | UnixFileMode.GroupRead);

                return screenshotsPath;
            }
        }
    }
}
