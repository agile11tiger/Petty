using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petty.Services.Local.Localization
{
    public class Language
    {
        public string Name { get; set; }
        public CultureInfo CultureInfo { get; set; }
    }
}
