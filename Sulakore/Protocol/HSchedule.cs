using System;

namespace Sulakore.Protocol
{
    public class HSchedule
    {
        private readonly Action<HSchedule> _callback;

        public int Burst { get; set; }
        public int Interval { get; set; }
        public HMessage Packet { get; set; }

        public HSchedule(HMessage packet, int interval, int burst, Action<HSchedule> callback)
        {
            Packet = packet;
            Interval = interval;
            Burst = burst;

            _callback = callback;
        }

        public void OnTick()
        {
            _callback(this);
        }
    }
}