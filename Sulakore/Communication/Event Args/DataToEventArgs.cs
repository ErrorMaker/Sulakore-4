using System;
using Sulakore.Protocol;
using System.ComponentModel;

namespace Sulakore.Communication
{
    public class DataToEventArgs : CancelEventArgs
    {
        public HMessage Packet { get; set; }
        public int Step { get; private set; }

        public DataToEventArgs(byte[] data, HDestination destination, int step)
        {
            Step = step;
            Packet = new HMessage(data, destination);
        }
    }
}