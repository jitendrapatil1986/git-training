namespace Warranty.Core.JobServiceApiHelpers
{
    using System;
    using System.Reflection;
    using JobService.Enumerations.Extensions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Warranty.Core.Enumerations;

    public class EnumerationJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null || objectType == null)
                return null;
            var enumeration = JObject.Load(reader);
            var parameterArray = new[] { enumeration["Value"].Value<int>() };
            var boxedParameterArray = new object[1];
            Array.Copy(parameterArray, boxedParameterArray, 1);
            var basedOn = objectType;
            while (!(basedOn.IsGenericType && basedOn.GetGenericTypeDefinition() == typeof(Common.Enumerations.Enumeration<>)))
            {
                basedOn = basedOn.BaseType;
            }
            return basedOn == null ? null : basedOn.GetMethod("FromValue", BindingFlags.Public | BindingFlags.Static).Invoke(null, boxedParameterArray);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsA(typeof(Enumeration<>));
        }
    }
}