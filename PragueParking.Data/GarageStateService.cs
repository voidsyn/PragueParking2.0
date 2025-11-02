using PragueParking.Core;

namespace PragueParking.Data
{
    public static class GarageStateService
    {
        // Filnamnet för parkeringsdatafilen
        private const string StateFilePath = "parking_data.json";

        //Denna metod anropas när programmet startar.
        public static void LoadGarageState(ParkingGarage garage)
        {
            var state = JsonFileService.LoadFromJsonFile<GarageState>(StateFilePath);
            if (state == null)
            {
                Console.WriteLine($"Parkeringsdatafilen ({StateFilePath}) hittades inte. Startar med tomt garage. ");
                return;
            }
            Console.WriteLine($"Läser in parkeringsdata från {StateFilePath}...");
            int spotsToProcess = Math.Min(state.Spots.Count, garage.Spots.Count);

            for (int i = 0; i < spotsToProcess; i++)
            {
                var spotState = state.Spots[i];
                var spotInGarage = garage.Spots[i];
                if (spotInGarage.SpotNumber == spotState.SpotNumber)
                {
                    foreach (var vehicleState in spotState.ParkedVehicles)
                    {
                        Vehicle vehicleToPark;
                        switch (vehicleState.Type.ToUpper())
                        {
                            case "CAR":
                                vehicleToPark = new Car(vehicleState.RegistrationNumber, vehicleState.CheckInTime);
                                break;
                            case "MC":
                                vehicleToPark = new MC(vehicleState.RegistrationNumber, vehicleState.CheckInTime);
                                break;
                            default:
                                Console.WriteLine($"Okänd fordonstyp i datafil: {vehicleState.Type}. Hoppar över detta fordon.");
                                continue;
                        }
                        spotInGarage.ParkedVehicles.Add(vehicleToPark);
                    }
                }
                else
                {
                    Console.WriteLine($"Varning: Spotnumber matcher inte vid index {i}. fil: {spotState.SpotNumber}, Garage: {spotInGarage.SpotNumber}");
                }
            }
            Console.WriteLine("Parkeringsdata inläst.");
        }
        public static void SaveGarageState(ParkingGarage garage)
        {
            Console.WriteLine($"Sparar parkeringsdata...");
            var state = new GarageState();

            foreach (var spotInGarage in garage.Spots)
            {
                var spotState = new ParkingSpotState
                {
                    SpotNumber = spotInGarage.SpotNumber,
                    MaxSize = spotInGarage.MaxSize
                };

                foreach (var vehicleInGarage in spotInGarage.ParkedVehicles)
                {
                    var vehicleState = new VehicleState
                    {
                        Type = vehicleInGarage.Type,
                        RegistrationNumber = vehicleInGarage.RegistrationNumber,
                        CheckInTime = vehicleInGarage.CheckInTime
                    };
                    spotState.ParkedVehicles.Add(vehicleState);
                }
                state.Spots.Add(spotState);
            }
            JsonFileService.WriteToJsonFile(state, StateFilePath);
            Console.WriteLine($"Parkeringsdata sparad till {StateFilePath}.");
        }
    }
}