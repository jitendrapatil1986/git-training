namespace Warranty.Core.Extensions
{
    using System.IO;
    using Configurations;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public static class ObjectExtensions
    {
        public static string ToJson(this object obj, bool useEnumerationDisplay = false)
        {
            var jsonSerializer =
                JsonSerializer.Create(new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                });

            if (useEnumerationDisplay)
            {
                jsonSerializer.Converters.Add(new EnumerationDisplayConverter());
            }

            var stringWriter = new StringWriter();
            jsonSerializer.Serialize(stringWriter, obj);

            return stringWriter.ToString();
        }
    }
}
