using System.ComponentModel;

using Sulakore.Protocol;

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