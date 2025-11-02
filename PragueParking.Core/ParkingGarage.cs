using System.Collections.Generic;
using System.Linq;

namespace PragueParking.Core
{
    public class ParkingGarage
    {
        public List<ParkingSpot> Spots { get; set; }
        public int TotalSpots => Spots.Count;
        public int OccupiedSpots => Spots.Count(spot => !spot.IsFree);
        public int FreeSpots => TotalSpots - OccupiedSpots;
        public ParkingGarage(int numberOfSpots = 100)
        {
            Spots = new List<ParkingSpot>();
            for (int i = 0; i < numberOfSpots; i++)
            {
                Spots.Add(new ParkingSpot(i, 4)); // Skapar parkeringsplatser med standardstorlek 4.

            }
        }

        public bool ParkVehicle(Vehicle vehicle)
        {
            var spot = Spots.FirstOrDefault(s => s.IsFreeFor(vehicle));
            if (spot != null)
            {
                return spot.ParkVehicle(vehicle);
            }
            return false; // Fordonet hittades inte i garaget.
        }

        public bool UnparkVehicle(string registrationNumber)
        {
            var spot = Spots.FirstOrDefault(s => s.ParkedVehicles.Any(v => v.RegistrationNumber == registrationNumber));
            if (spot != null)
            {
                var vehicleToRemove = spot.RemoveVehicle(registrationNumber);
                return vehicleToRemove != null;
            }
            return false; // Fordonet hittades inte i garaget.
        }

        public bool MoveVehicle(string registrationNumber, int newSpotIndex)
        {
            if (newSpotIndex < 0 || newSpotIndex >= Spots.Count)
            {
                return false; // Ogiltigt platsindex.
            }

            // Hitta platsen där fordonet står nu.
            var currentSpot = Spots.FirstOrDefault(s => s.ParkedVehicles.Any(v => v.RegistrationNumber == registrationNumber));
            if (currentSpot == null)

            {
                return false; // Fordonet hittades inte i garaget.
            }

            var vehicleToMove = currentSpot.RemoveVehicle(registrationNumber);
            if (vehicleToMove == null)
            {
                return false; // Fordonet kunde inte tas bort från den nuvarande platsen.
            }
            // Försöker parkera på den nya platsen.
            bool success = Spots[newSpotIndex].ParkVehicle(vehicleToMove);
            if (!success)
            {
                // Om parkeringen misslyckades, återställ fordonet till den ursprungliga platsen.
                currentSpot.ParkVehicle(vehicleToMove);
                return false; // Flytten misslyckades.
            }
            return true; // Flytten lyckades.
        }

        public Vehicle? GetVehicle(string registrationNumber)
        {
            var spot = Spots.FirstOrDefault(s => s.ParkedVehicles.Any(v => v.RegistrationNumber == registrationNumber));
            return spot?.ParkedVehicles.FirstOrDefault(v => v.RegistrationNumber == registrationNumber);
        }

        public int? FindVehicleSpotIndex(string registrationNumber)
        {
            var spot = Spots.FirstOrDefault(s => s.ParkedVehicles.Any(v => v.RegistrationNumber == registrationNumber));
            if (spot != null)
            {
                return spot.SpotNumber;
            }
            return null; // Fordonet finns inte i garaget.
        }

        public IEnumerable<string> GetGarageStatus()
        {
            return Spots.Select(spot => spot.GetDisplayString());
        }

    }
}