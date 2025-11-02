using System.Collections.Generic;
using System.Linq;

namespace PragueParking.Core
{
    public class ParkingSpot
    {
        public int SpotNumber { get; set; }
        public int MaxSize { get; set; } // Storlek på platsen (Standard är 4).
        public List<Vehicle> ParkedVehicles { get; set; } // Lista över parkerade fordon.
        public ParkingSpot(int spotNumber, int maxSize = 4)
        {
            SpotNumber = spotNumber;
            MaxSize = maxSize;
            ParkedVehicles = new List<Vehicle>();
        }
        public bool IsFree => ParkedVehicles.Count == 0; // Kollar om platsen är ledig.
        public bool IsFreeFor(Vehicle vehicle) // Kontrolerar om platsen har tillräckligt med utrymme för fordonet.
        {
            int currentSize = ParkedVehicles.Sum(v => v.Size);
            return (currentSize + vehicle.Size) <= MaxSize;
        }
        public bool ParkVehicle(Vehicle vehicle) // Försöker parkera fordonet på platsen.
        {
            if (IsFreeFor(vehicle))
            {
                ParkedVehicles.Add(vehicle);
                return true;
            }
            return false; // Platsen är inte tillräckligt stor för fordonet.
        }

        public Vehicle RemoveVehicle(string registrationNumber) // Tar bort fordonet från platsen baserat på registreringsnumret.
        {
            var vehicle = ParkedVehicles.FirstOrDefault(v => v.RegistrationNumber == registrationNumber);
            if (vehicle != null)
            {
                ParkedVehicles.Remove(vehicle);
            }
            return vehicle; // Returnerar det borttagna fordonet eller null om det inte hittades.
        }
        public string GetDisplayString() // Hämtar information om alla parkerade fordon på platsen.
        {
            if (IsFree)
            {
                return $"Plats {SpotNumber}: Ledig"; // Platsen är ledig.
            }
            var descriptions = ParkedVehicles.Select(v => $"{v.Type} {v.RegistrationNumber}");
            return $"Plats {SpotNumber}: {string.Join(", ", descriptions)}";

        }
    }
}