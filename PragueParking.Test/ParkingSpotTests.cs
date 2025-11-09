using Microsoft.VisualStudio.TestTools.UnitTesting;
using PragueParking.Core;

namespace PragueParking.Test
{
    [TestClass]
    public class ParkingSpotTests
    {
        [TestMethod]
        public void NewSpot_ShouldBeFree()
        {
            // Arrange
            var spot = new ParkingSpot(1);

            // Act
            bool isFree = spot.IsFree;

            // Assert
            Assert.IsTrue(isFree);
        }
        [TestMethod]
        public void ParkCar_ShouldMarkSpotAsOccupied()
        {
            // Arrange
            var spot = new ParkingSpot(1);
            var car = new Car("ABC123");

            // Act
            spot.ParkVehicle(car);

            // Assert
            Assert.IsFalse(spot.IsFree);
            Assert.AreEqual("ABC123", spot.ParkedVehicles[0].RegistrationNumber);
        }
        [TestMethod]
        public void TwoMCs_ShouldFitInSameSpot()
        {
            // Arrange
            var spot = new ParkingSpot(1);
            var mc1 = new MC("MC111");
            var mc2 = new MC("MC222");
            // Act
            spot.ParkVehicle(mc1);
            spot.ParkVehicle(mc2);
            // Assert
            
            Assert.AreEqual(2, spot.ParkedVehicles.Count);
            Assert.AreEqual("MC111", spot.ParkedVehicles[0].RegistrationNumber);
            Assert.AreEqual("MC222", spot.ParkedVehicles[1].RegistrationNumber);
            Assert.IsFalse(spot.IsFree);
        }
    }
}
