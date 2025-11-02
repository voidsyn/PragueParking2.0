using System;

namespace PragueParking.Core
{
    public class MC : Vehicle
    {
        public MC(string registrationNumber) : base(registrationNumber)
        {
        }

        public MC(string registrationNumber, DateTime checkInTime) : base(registrationNumber, checkInTime)
        {
        }

        public override int Size => 2;
        public override string Type => "MC";
    }
}