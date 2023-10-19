using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
namespace PettySQLite.Models;

public partial class BaseSettings : IDatabaseItem
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    public string LanguageType = CultureInfo.CurrentCulture.Name;
    public InformationPerceptionModes InformationPerceptionMode { get; set; }
}
