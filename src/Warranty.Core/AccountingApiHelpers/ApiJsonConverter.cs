using System.Collections.Generic;
using System.Text;
using Common.Api.Http;
using Newtonsoft.Json;

namespace Warranty.Core.AccountingApiHelpers
{
    public class ApiJsonConverter : IApiConverter
    {
        private readonly JsonSerializerSettings _settings;

        public ApiJsonConverter()
        {
            _settings = new JsonSerializerSettings();
            _settings.Converters.Add(new EnumerationJsonConverter());
        }

        public T Decode<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }

        public string Encode(object obj, IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers = null)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public string GetMediaType()
        {
            return "application/json";
        }

        public Encoding GetEncoding()
        {
            return Encoding.UTF8;
        }
    }
}
