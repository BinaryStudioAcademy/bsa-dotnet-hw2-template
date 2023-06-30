using System;
using System.Timers;

namespace CoolParking.BL.Interfaces
{
    public interface ITimerService : IDisposable
    {
        event ElapsedEventHandler Elapsed;
        double Interval { get; set; }
        void Start();
        void Stop();
    }
}
