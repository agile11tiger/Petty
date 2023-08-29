using Petty.Resources.Localization;
using System.Globalization;

namespace Petty.Services.Local
{
    public class LocalizationService
    {
        public LocalizationService() 
        { 

        }

        private Dictionary<string, string> _cacheStringWithParameters = new Dictionary<string, string>();

        public string Get(string key, params object[] parameters)
        {
            var str = AppResources.ResourceManager.GetString(key);
            return string.Format(CultureInfo.InvariantCulture, str, parameters);
        }
    }
}
