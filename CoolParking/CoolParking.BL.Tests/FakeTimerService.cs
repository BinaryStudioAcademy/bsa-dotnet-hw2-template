using System.Timers;
using CoolParking.BL.Interfaces;

namespace CoolParking.BL.Tests
{
    public class FakeTimerService :  ITimerService
    {
        public double Interval { get; set; }

        public event ElapsedEventHandler Elapsed;

        public void FireElapsedEvent()
        {
            Elapsed?.Invoke(this, null);
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void Dispose()
        {
        }

    }
}
