using System.ComponentModel.DataAnnotations.Schema;
namespace PettySQLite.Models;

public class Settings : IDatabaseItem
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    public virtual BaseSettings? BaseSettings { get; set; }
    public virtual VoiceSettings? VoiceSettings { get; set; }
}
