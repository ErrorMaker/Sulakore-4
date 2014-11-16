namespace Sulakore.Communication.Bridge
{
    public interface IHContractor
    {
        int SendToClient(byte[] data);
    }
}