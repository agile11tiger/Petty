using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PettySQLite.Models
{
    public partial class BaseSettings : IDatabaseItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string LanguageType = CultureInfo.CurrentCulture.Name;
        public bool? UseFrontCamera
        {
            get => false;
            set => value = false;
        }
    }
}
