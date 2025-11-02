using System;

namespace PragueParking.Core
{
    //Basklassen för alla fordonstyper.
    public abstract class Vehicle
    {
        public string RegistrationNumber { get; set; } // Registeringsnummer.
        public DateTime CheckInTime { get; private set; } // Tiden då fordonet checkade in.

        public abstract int Size { get; }
        public abstract string Type { get; }

        public Vehicle(string registrationNumber)
        {
            RegistrationNumber = registrationNumber;
            CheckInTime = DateTime.Now; // Sparar tiden då Fordonet checkar in.
        }

        public Vehicle(string registrationNumber, DateTime checkInTime)
        {
            RegistrationNumber = registrationNumber;
            CheckInTime = checkInTime; // Sparar tiden då Fordonet checkar in.
        }

        // En metod för att beräkna parkeringstiden för alla fordonstyper.
        public TimeSpan GetParkingDuration()
        {
            return DateTime.Now - CheckInTime;
        }

        //Metod för att generera kvitto vid utcheckning.
        public string GenerateReceipt(double pricePerHour, int freeMinutes)
        {

            DateTime departureTime = DateTime.Now;
            TimeSpan parkedTime = departureTime - CheckInTime;

            // Beräknar avgift
            double chargeHours = parkedTime.TotalHours - ((double) freeMinutes / 60.0);
            int hoursToCharge = 0;
            double totalFee = 0.0;

            if (chargeHours > 0)
            {
                hoursToCharge = (int)Math.Ceiling(chargeHours);
                totalFee = hoursToCharge * pricePerHour;
            }

            var receipt = $"--- PARKERINGSKVITTO ---\n" +
                          $"Fordonstyp: {Type}\n" +
                          $"Registreringsnummer: {RegistrationNumber}\n" +
                          $"Incheckningstid: {CheckInTime:yyyy-MM-dd HH:mm:ss}\n" +
                          $"Utcheckningstid: {departureTime:yyyy-MM-dd HH:mm:ss}\n" +
                          $"Parkeringstid: {parkedTime:hh\\:mm\\:ss}\n" +
                          $"Avgiftbelagd tid: {hoursToCharge} timmar (påbörjade)\n" +
                          $"Pris per timme: {pricePerHour} CZK\n" +
                          $"Total avgift: {totalFee} CZK\n"+
                          $"--- Välkommen åter! ---";

            return receipt;
        }
    }
}