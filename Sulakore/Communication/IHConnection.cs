using System;
using Sulakore.Protocol.Encryption;
using Sulakore.Protocol;

namespace Sulakore.Communication
{
    public interface IHConnection
    {
        event EventHandler<EventArgs> Connected;
        event EventHandler<DataToEventArgs> DataToClient;
        event EventHandler<DataToEventArgs> DataToServer;
        event EventHandler<DisconnectedEventArgs> Disconnected;

        int Port { get; }
        string Host { get; }
        string[] Addresses { get; }
        HProtocols Protocol { get; }

        Rc4 ServerDecrypt { get; set; }
        Rc4 ServerEncrypt { get; set; }

        Rc4 ClientEncrypt { get; set; }
        Rc4 ClientDecrypt { get; set; }

        bool IsConnected { get; }
        bool RequestEncrypted { get; }
        bool ResponseEncrypted { get; }

        int SendToServer(byte[] data);
        int SendToClient(byte[] data);
    }
}