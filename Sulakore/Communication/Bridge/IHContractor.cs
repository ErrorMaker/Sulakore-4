using Sulakore.Protocol;
using System;
namespace Sulakore.Communication.Bridge
{
    public interface IHContractor
    {
        void SendToClient(byte[] data);
        void SendToServer(byte[] data);
    }
}