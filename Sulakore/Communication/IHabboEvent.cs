using Sulakore.Protocol;
using System.Collections.Generic;

namespace Sulakore.Communication
{
    public interface IHabboEvent
    {
        Dictionary<string, object> Data { get; }
        HMessage Packet { get; }
        ushort Header { get; }
    }
}