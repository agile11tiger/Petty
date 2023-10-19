using Newtonsoft.Json;
namespace Petty.Models.Interfaces;

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
