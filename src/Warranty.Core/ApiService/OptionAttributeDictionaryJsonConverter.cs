namespace Warranty.Core.ApiService
{
    using System;
    using System.Collections.Generic;
    using JobService.Enumerations;
    using JobService.Enumerations.Extensions;
    using Newtonsoft.Json;

    public class OptionAttributeDictionaryJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return
                objectType.IsA(typeof(IDictionary<,>)) &&
                objectType.GetGenericArguments()[0].IsA(typeof(OptionAttributeType)) &&
                objectType.GetGenericArguments()[1].IsA(typeof(string));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var intermediateDictionary = new Dictionary<string, string>();
            serializer.Populate(reader, intermediateDictionary);

            var finalDictionary = new Dictionary<OptionAttributeType, string>();
            foreach (var pair in intermediateDictionary)
            {
                int id;
                if (Int32.TryParse(pair.Key, out id))
                    finalDictionary.Add(OptionAttributeType.FromValueOrDefault(id), pair.Value);
            }

            return finalDictionary;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                serializer.Serialize(writer, null);

            writer.WriteStartObject();
            writer.WritePropertyName("$type");
            writer.WriteValue(typeof(IDictionary<OptionAttributeType, string>).FullName);
            var attributeDictionary = value as IDictionary<OptionAttributeType, string>;
            foreach (var pair in attributeDictionary)
            {
                writer.WritePropertyName(pair.Key.Value.ToString());
                writer.WriteValue(pair.Value);
            }
            writer.WriteEndObject();
        }
    }
}