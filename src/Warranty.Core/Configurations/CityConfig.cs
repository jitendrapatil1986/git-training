using System.Configuration;

namespace Warranty.Core.Configurations
{
    public class CityConfig : ConfigurationElement
    {
        [ConfigurationProperty("City", IsKey = true, IsRequired = true)]
        public string City
        {
            get { return this["City"] as string; }
            set { this["City"] = value; }
        }

        [ConfigurationProperty("WarrantyAmount", IsRequired = true)]
        public decimal WarrantyAmount
        {
            get { return (decimal)this["WarrantyAmount"]; }
            set { this["WarrantyAmount"] = value; }
        }

        [ConfigurationProperty("ClosedOutCommunity", IsRequired = true)]
        public string ClosedOutCommunity
        {
            get { return this["ClosedOutCommunity"] as string; }
            set { this["ClosedOutCommunity"] = value; }
        }
    }
}