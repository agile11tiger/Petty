using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petty.Models.Interfaces
{
    public interface ISerializableDatabaseItem : IDatabaseItem
    {
        void Serialize();
        void Deserialize();

        static JsonSerializerSettings JsonSettings = new()
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
        };
    }
}
