namespace Sulakore.Communication.Bridge
{
    public class HTriggers : HTriggerBase
    {
        public override bool CaptureEvents { get; set; }

        public new void ProcessIncoming(byte[] data)
        {
            base.ProcessIncoming(data);
        }
        public new void ProcessOutgoing(byte[] data)
        {
            base.ProcessOutgoing(data);
        }
    }
}