using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore.Storage;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace PettySQLite.Models
{
    public class Settings: IDatabaseItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public virtual BaseSettings? BaseSettings { get; set; }
        public virtual VoiceSettings? VoiceSettings { get; set; }
    }
}
