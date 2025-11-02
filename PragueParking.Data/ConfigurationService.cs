using PragueParking.Data;

namespace PragueParking.Data
{
    public static class ConfigurationService
    {
        private const string ConfigFilePath = "config.json";
        public static GarageConfiguration ReadConfiguration()
        {
            var config = JsonFileService.LoadFromJsonFile<GarageConfiguration>(ConfigFilePath);
            if (config == null)
            {
                Console.WriteLine($"Konfigurationsfilen ({ConfigFilePath}) hittades inte eller kunde inte läsas. Använder standardvärden.");
                config = new GarageConfiguration();
            }
            return config;
        }
    }
}
