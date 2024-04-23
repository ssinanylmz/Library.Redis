using System.Configuration;

namespace Library.Redis
{
    public static class ConfigurationHelper
    {
        internal static T Get<T>(string appSettingsKey, T defaultValue)
        {
            var valueAppSetting = ConfigurationManager.AppSettings[appSettingsKey];

            if (string.IsNullOrWhiteSpace(valueAppSetting))
                return defaultValue;

            try
            {
                var value = Convert.ChangeType(valueAppSetting, typeof(T));
                return (T)value;
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}