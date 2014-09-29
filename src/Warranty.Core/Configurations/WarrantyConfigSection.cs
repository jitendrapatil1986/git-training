﻿using System.Configuration;
using System.Linq;

namespace Warranty.Core.Configurations
{
    public class WarrantyConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("Cities", IsRequired = false)]
        public CitiesCollection Cities
        {
            get { return base["Cities"] as CitiesCollection; }
        }

        public static WarrantyConfigSection GetSection()
        {
            return (WarrantyConfigSection)ConfigurationManager.GetSection("WarrantyConfigSection");
        }

        public static string GetCity(string cityCode)
        {
            var cityConfig = GetSection()
                                .Cities
                                .Cast<CityConfig>()
                                .SingleOrDefault(x => cityCode == x.City);

            var city = cityConfig != null ? cityConfig.City : "";
            return city;
        }
    }
}
