using Sulakore.Protocol;

namespace Sulakore.Communication.Bridge
{
    public interface IHContractor
    {
        HProtocol Protocol { get; }

        void SendToClient(byte[] data);
        void SendToServer(byte[] data);

        void InitiateUnload(IHExtension extension);
    }
}