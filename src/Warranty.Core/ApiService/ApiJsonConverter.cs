﻿namespace Warranty.Core.ApiService
{
    using JobService.Client.Api;
    using Newtonsoft.Json;

    public class ApiJsonConverter : IApiJsonConverter
    {
        private readonly JsonSerializerSettings _settings;

        public ApiJsonConverter()
        {
            _settings = new JsonSerializerSettings();
            _settings.Converters.Add(new OptionAttributeDictionaryJsonConverter());
            _settings.Converters.Add(new EnumerationJsonConverter());
        }

        public T Decode<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }

        public string Encode(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}