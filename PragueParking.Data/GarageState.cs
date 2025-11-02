using System.Collections.Generic;


namespace PragueParking.Data
{
    public class GarageState
    {
        public List<ParkingSpotState> Spots { get; set; } = new List<ParkingSpotState>();
    }
    public class ParkingSpotState
    {
        public int SpotNumber { get; set; }
        public int MaxSize { get; set; }
        public List<VehicleState> ParkedVehicles { get; set; } = new List<VehicleState>();
    }
    public class VehicleState
    {
        public string Type { get; set; } = "";
        public string RegistrationNumber { get; set; } = "";
        public DateTime CheckInTime { get; set; }
    }
}
