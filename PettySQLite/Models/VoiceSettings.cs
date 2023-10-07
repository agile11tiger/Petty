using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PettySQLite.Models
{
    public class VoiceSettings: IDatabaseItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public float Pitch { get; set; } = 1;
        public float Volume { get; set; } = 0.5f;

    }
}
