namespace Warranty.Core.Configurations
{
    using System;
    using System.Linq;
    using Newtonsoft.Json;
    using Yay.Enumerations;

    public class EnumerationDisplayConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var enumeration = (EnumerationBase)value;
            writer.WriteValue(enumeration.DisplayName);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;

            var displayValue = reader.Value.ToString();

            return EnumerationHelper.GetAll(objectType).SingleOrDefault(x => x.DisplayName == displayValue);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.Closes(typeof(Enumeration<>));
        }
    }
}
