using System.Text.Json;

namespace PragueParking.Data
{
    public static class JsonFileService
    {
        public static T? LoadFromJsonFile<T>(string filePath) where T : class
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Varning: Filen '{filePath}' hittades inte.");
                return null;
            }
            try
            {
                string jsonString = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<T>(jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid inläsning av JSOn: {ex.Message}");
                    return null;
                }
        }
        public static void WriteToJsonFile<T>(T data, string filePath) where T : class
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(data, options);
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid skrivning till JSON: {ex.Message}");
            }

        }
    }
}
