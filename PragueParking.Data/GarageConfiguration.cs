using System.Collections.Generic;


namespace PragueParking.Data
{
    public class GarageConfiguration
    {
        public int NumberOfSpots { get; set; } = 100;
        public Dictionary<string, int> VehicleTypes { get; set; } = new Dictionary<string, int> { { "CAR", 4 }, { "MC", 2 } };
        public Dictionary<string, double> PricesPerHour { get; set; } = new Dictionary<string, double> { { "CAR", 20.0 }, { "MC", 10.0 } };
        public int FreeMinutes { get; set; } = 10;
    }
}
