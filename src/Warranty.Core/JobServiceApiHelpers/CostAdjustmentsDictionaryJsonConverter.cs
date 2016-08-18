using System;
using System.Collections.Generic;
using System.Linq;
using JobService.Client.Models.BuildConfigurationDtos;
using JobService.Enumerations;
using JobService.Enumerations.Extensions;
using Newtonsoft.Json;

namespace Warranty.Core.JobServiceApiHelpers
{
    public class CostAdjustmentsDictionaryJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return
                objectType.IsA(typeof(IDictionary<,>)) &&
                objectType.GetGenericArguments()[0].IsA(typeof(CostAdjustmentType)) &&
                objectType.GetGenericArguments()[1].IsA(typeof(CostAdjustmentDto));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var intermediateDictionary = new Dictionary<string, CostAdjustmentDto>();
            serializer.Populate(reader, intermediateDictionary);

            return intermediateDictionary.Where(pair => CostAdjustmentType.FromDisplayName(pair.Key) != null).ToDictionary(pair => CostAdjustmentType.FromDisplayName(pair.Key), pair => pair.Value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                serializer.Serialize(writer, null);

            writer.WriteStartObject();
            writer.WritePropertyName("$type");
            writer.WriteValue(typeof(IDictionary<CostAdjustmentType, CostAdjustmentDto>).FullName);
            var attributeDictionary = value as IDictionary<CostAdjustmentType, CostAdjustmentDto>;
            foreach (var pair in attributeDictionary)
            {
                writer.WritePropertyName(pair.Key.Value.ToString());
                writer.WriteValue(pair.Value);
            }
            writer.WriteEndObject();
        }
    }
}