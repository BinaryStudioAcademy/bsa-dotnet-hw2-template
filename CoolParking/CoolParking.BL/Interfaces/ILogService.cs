using System;

namespace CoolParking.BL.Interfaces
{
    public interface ILogService : IDisposable
    {
        string LogPath { get; }
        void Write(string logInfo);
        string Read();
    }
}