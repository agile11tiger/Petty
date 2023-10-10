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

        public static string PicturesPath
        {
            get
            {
                var picturesPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).Path;

                //Unlikely since this is a base folder.
                if (!Directory.Exists(picturesPath))
                    Directory.CreateDirectory(picturesPath, UnixFileMode.GroupWrite | UnixFileMode.GroupRead);

                return picturesPath;
            }
        }

        public static string VideoPath
        {
            get
            {
                var videoPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMovies).Path;

                //Unlikely since this is a base folder.
                if (!Directory.Exists(videoPath))
                    Directory.CreateDirectory(videoPath, UnixFileMode.GroupWrite | UnixFileMode.GroupRead);

                return videoPath;
            }
        }
    }
}
