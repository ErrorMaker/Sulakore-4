using System;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public class DataToEventArgs : EventArgs
    {
        public bool Skip { get; set; }
        public HMessage Packet { get; set; }
        public int Step { get; private set; }

        public DataToEventArgs(byte[] data, HDestinations destination, int step)
        {
            Step = step;
            Packet = new HMessage(data, destination);
        }
    }
}