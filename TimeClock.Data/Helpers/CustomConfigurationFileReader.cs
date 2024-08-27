using System.Configuration;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.JdeSync")]
namespace TimeClock.Data.Helpers
{
    /// <summary>
    /// Gets the named connection string from App.dev.config
    /// </summary>
    internal static class DevConfigurationFileReader
    {
        private static readonly string FileName = "App.dev.config";
        /// <summary>
        /// Gets the named connection string from App.dev.config
        /// </summary>
        /// <param name="name">the name of the connection string</param>
        /// <returns></returns>
        public static string GetConnectionString(string name)
        {
            var configMap = new ExeConfigurationFileMap()
            {
                ExeConfigFilename = FileName,
            };

            var config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

            return config.ConnectionStrings.ConnectionStrings[name].ConnectionString;
        }
    }
}
