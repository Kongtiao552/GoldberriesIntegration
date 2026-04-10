using System;
using Newtonsoft.Json;

namespace Celeste.Mod.GoldberriesIntegration.Models.Goldberries;

public class TimeSpanConverter : JsonConverter<TimeSpan?>
{
    public override TimeSpan? ReadJson(JsonReader reader, Type objectType, TimeSpan? existingValue, bool hasExistingValue, JsonSerializer serializer) {
        if (reader.Value == null) {
            return null;
        }
        
        return TimeSpan.FromSeconds(Convert.ToInt32(reader.Value));
    }

    public override void WriteJson(JsonWriter writer, TimeSpan? value, JsonSerializer serializer) {
        if (!value.HasValue) {
            writer.WriteNull();
            return;
        }

        writer.WriteValue((int) value.Value.TotalSeconds);
    }
}