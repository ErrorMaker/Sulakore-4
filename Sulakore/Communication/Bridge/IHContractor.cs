using Sulakore.Protocol;

namespace Sulakore.Communication.Bridge
{
    public interface IHContractor
    {
        HProtocols Protocol { get; }

        void SendToClient(byte[] data);
        void SendToServer(byte[] data);

        void Unload(IHExtension extension);
    }
}