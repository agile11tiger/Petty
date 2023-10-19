using System.ComponentModel.DataAnnotations.Schema;
namespace PettySQLite.Models;

public class VoiceSettings : IDatabaseItem
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public float Pitch { get; set; } = 1;
    public float Volume { get; set; } = 0.5f;

}
