using System;
using System.Collections.ObjectModel;
using CoolParking.BL.Models;

namespace CoolParking.BL.Interfaces
{
    public interface IParkingService : IDisposable
    {
        decimal GetBalance();
        int GetCapacity();
        int GetFreePlaces();
        ReadOnlyCollection<Vehicle> GetVehicles();
        void AddVehicle(Vehicle vehicle);
        void RemoveVehicle(string vehicleId);
        void TopUpVehicle(string vehicleId, decimal sum);
        TransactionInfo[] GetLastParkingTransactions();
        string ReadFromLog();
    }
}
