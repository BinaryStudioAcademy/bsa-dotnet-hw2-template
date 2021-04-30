using System;
using System.Linq;
using CoolParking.BL.Interfaces;
using CoolParking.BL.Models;
using Xunit;
using FakeItEasy;
using CoolParking.BL.Services;

namespace CoolParking.BL.Tests
{
    public class ParkingServiceTests : IDisposable
    {
        readonly ParkingService _parkingService;
        readonly FakeTimerService _withdrawTimer;
        readonly FakeTimerService _logTimer;
        readonly ILogService _logService;

        public ParkingServiceTests()
        {
            _withdrawTimer = new FakeTimerService();
            _logTimer = new FakeTimerService();
            _logService = A.Fake<ILogService>();
            _parkingService = new ParkingService(_withdrawTimer, _logTimer, _logService);
        }

        public void Dispose()
        {
            _parkingService.Dispose();
        }

        [Fact]
        public void Parking_IsSingelton()
        {
            var newParkingService = new ParkingService(_withdrawTimer, _logTimer, _logService);
            var vehicle = new Vehicle("AA-0001-AA", VehicleType.Truck, 100);
            _parkingService.AddVehicle(vehicle);  

            Assert.Single(newParkingService.GetVehicles());
            Assert.Single(_parkingService.GetVehicles());
            Assert.Same(_parkingService.GetVehicles()[0], newParkingService.GetVehicles()[0]);
        }

        [Fact]
        public void GetCapacity_WhenEmpty_Then10()
        {
            Assert.Equal(10, _parkingService.GetCapacity());
        }

        [Fact]
        public void GetFreePlaces_WhenEmpty_Then10()
        {
            Assert.Equal(10, _parkingService.GetFreePlaces());
        }

        [Fact]
        public void AddVehicle_WhenNewVehicle_ThenVehiclesPlusOne()
        {
            var vehicle = new Vehicle("AA-0001-AA", VehicleType.Bus, 100);

            _parkingService.AddVehicle(vehicle);

            Assert.Single(_parkingService.GetVehicles());
        }

        [Fact]
        public void AddVehicle_WhenExistingVehicleId_ThenThrowArgumentException()
        {
            var vehicle1 = new Vehicle("AA-0001-AA", VehicleType.Bus, 100);
            var vehicle2 = new Vehicle(vehicle1.Id, VehicleType.Motorcycle, 200);
            _parkingService.AddVehicle(vehicle1);

            Assert.Throws<ArgumentException>(() => _parkingService.AddVehicle(vehicle2));
        }

        [Theory]
        [InlineData("AA 0001", VehicleType.Bus, 100)]
        [InlineData("AA-0001-AA", VehicleType.Bus, -100)]
        public void NewVehicle_WhenWrongArguments_ThenThrowArgumentException(string id, VehicleType vehicleType, decimal balance)
        {
            Assert.Throws<ArgumentException>(() => new Vehicle(id, vehicleType, balance));
        }

        [Fact]
        public void RemoveVehicle_WhenSingleExistingVehicle_ThenVehiclesEmpty()
        {
            var vehicle = new Vehicle("AA-0001-AA", VehicleType.Bus, 100);
            _parkingService.AddVehicle(vehicle);

            _parkingService.RemoveVehicle(vehicle.Id);

            Assert.Empty(_parkingService.GetVehicles());
        }

        [Fact]
        public void RemoveVehicle_WhenUnexistingVehicle_ThenThrowArgumentException()
        {
            var vehicle = new Vehicle("AA-0001-AA", VehicleType.Bus, 100);
            _parkingService.AddVehicle(vehicle);

            Assert.Throws<ArgumentException>(() => _parkingService.RemoveVehicle("AA-0002-AA"));
        }

        [Fact]
        public void TopUpVehicle_WhenExistingVehicleWith100ToppedUpOn100money_ThenVehiclesBalanceIs200money()
        {
            var vehicle = new Vehicle("AA-0001-AA", VehicleType.PassengerCar, 100);
            _parkingService.AddVehicle(vehicle);
            _parkingService.TopUpVehicle(vehicle.Id, 100);
            Assert.Equal(200, vehicle.Balance);
        }

        [Fact]
        public void TopUpVehicle_WhenExistingVehicleOnNegativeSum_ThenThrowArgumentException()
        {
            var vehicle = new Vehicle("AA-0001-AA", VehicleType.PassengerCar, 100);
            _parkingService.AddVehicle(vehicle);

            Assert.Throws<ArgumentException>(() => _parkingService.TopUpVehicle("AA-0001-AA", -100));
        }

        [Fact]
        public void TopUpVehicle_WhenUnexistingVehicle_ThenThrowArgumentException()
        {
            var vehicle = new Vehicle("AA-0001-AA", VehicleType.Bus, 100);
            _parkingService.AddVehicle(vehicle);

            Assert.Throws<ArgumentException>(() => _parkingService.TopUpVehicle("AA-0002-AA", 100));
        }

        [Fact]
        public void RegularWithdraw_WhenVehiclePositiveBalance_TaxIsWithdrowedFromVehicleToParkingBalance()
        {
            var vehicle = new Vehicle("AA-0001-AA", VehicleType.Truck, 100);
            _parkingService.AddVehicle(vehicle);
            _withdrawTimer.FireElapsedEvent();
            _withdrawTimer.FireElapsedEvent();

            Assert.Equal(90, vehicle.Balance);
            Assert.Equal(10, _parkingService.GetBalance());
        }

        [Fact]
        public void GetLastParkingTransactions_WhenTruckAndBusAfter2WithdrawTimeouts_ThenTransactionsSumIs17()
        {
            var vehicle1 = new Vehicle("AA-0001-AA", VehicleType.Truck, 100);
            var vehicle2 = new Vehicle("AA-0002-AA", VehicleType.Bus, 100);
            _parkingService.AddVehicle(vehicle1);
            _parkingService.AddVehicle(vehicle2);
            _withdrawTimer.FireElapsedEvent();
            _withdrawTimer.FireElapsedEvent();

            var lastParkingTransactions = _parkingService.GetLastParkingTransactions();

            Assert.Equal(17m, lastParkingTransactions.Sum(tr => tr.Sum));
        }

        [Fact]
        public void WhenLogTimerIsElapsed_ThenWriteLogIsHappened()
        {
            _logTimer.FireElapsedEvent();

            A.CallTo(() => _logService.Write(A<string>._)).MustHaveHappenedOnceExactly();
        }
    }
}