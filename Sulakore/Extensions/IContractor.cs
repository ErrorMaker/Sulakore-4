namespace Sulakore.Extensions
{
    public interface IContractor
    {
        int SendToClient(byte[] data);
        int SendToServer(byte[] data);
    }
}