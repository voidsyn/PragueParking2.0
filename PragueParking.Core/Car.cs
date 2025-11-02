using System;

namespace PragueParking.Core
{
    public class Car : Vehicle
    { 
        public Car(string registrationNumber) : base(registrationNumber)
        {
        }
        public Car(string registrationNumber, DateTime checkInTime) : base(registrationNumber, checkInTime)
        {
        }

        public override int Size => 4;
        public override string Type => "CAR";
    }
}