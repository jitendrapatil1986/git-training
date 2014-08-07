namespace Warranty.UI.Core.Helpers
{
    using System.Configuration;

    public static class EnvironmentHelper
    {
        private const string EnvironmentKey = "Environment";
        private const string ProductionValue = "prod";

        private static bool? _isProduction;

        public static bool IsProduction
        {
            get
            {
                if (!_isProduction.HasValue)
                {
                    var value = ConfigurationManager.AppSettings[EnvironmentKey];

                    _isProduction = (value == ProductionValue);
                }

                return _isProduction.Value;
            }
        }

        public static bool IsDevelopment
        {
            get { return !IsProduction; }
        }
    }
}