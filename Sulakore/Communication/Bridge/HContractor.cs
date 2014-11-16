namespace Sulakore.Communication.Bridge
{
    public class HContractor : IHContractor
    {
        public static HContractor Factory { get; private set; }

        public static HExtension LoadExtension(string path)
        {
            return null;
        }

        public int SendToClient(byte[] data)
        {
            return ((IHContractor)Factory).SendToClient(data);
        }
    }
}