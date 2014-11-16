using System;

namespace Sulakore.Protocol
{
    public static class HEncoding
    {
        private const string UnsupportedProtocol = "Specified Sulakore.Protocol.HProtocols value is not supported.";
        private const string DecIntDataTooSmall = "The length of the specified data is not valid for the given protocol.";

        public static byte[] CypherShort(this HProtocols protocol, ushort value)
        {
            if (protocol == HProtocols.Unknown) throw new Exception(UnsupportedProtocol);
            return (protocol == HProtocols.Modern ? Modern.CypherShort(value) : Ancient.CypherShort(value));
        }
        public static byte[] CypherShort(this HProtocols protocol, byte[] source, int offset, ushort value)
        {
            if (protocol == HProtocols.Unknown) throw new Exception(UnsupportedProtocol);
            return (protocol == HProtocols.Modern ? Modern.CypherShort(source, offset, value) : Ancient.CypherShort(source, offset, value));
        }
        public static ushort DecypherShort(this HProtocols protocol, byte[] data)
        {
            if (protocol == HProtocols.Unknown) throw new Exception(UnsupportedProtocol);
            return (protocol == HProtocols.Modern ? Modern.DecypherShort(data) : Ancient.DecypherShort(data));
        }
        public static ushort DecypherShort(this HProtocols protocol, string encoded)
        {
            if (protocol == HProtocols.Unknown) throw new Exception(UnsupportedProtocol);
            return (protocol == HProtocols.Modern ? Modern.DecypherShort(encoded) : Ancient.DecypherShort(encoded));
        }
        public static ushort DecypherShort(this HProtocols protocol, byte[] data, int offset)
        {
            if (protocol == HProtocols.Unknown) throw new Exception(UnsupportedProtocol);
            return (protocol == HProtocols.Modern ? Modern.DecypherShort(data, offset) : Ancient.DecypherShort(data, offset));
        }
        public static ushort DecypherShort(this HProtocols protocol, byte first, byte second)
        {
            if (protocol == HProtocols.Unknown) throw new Exception(UnsupportedProtocol);
            return (protocol == HProtocols.Modern ? Modern.DecypherShort(first, second) : Ancient.DecypherShort(first, second));
        }

        public static byte[] CypherInt(this HProtocols protocol, int value)
        {
            if (protocol == HProtocols.Unknown) throw new Exception(UnsupportedProtocol);
            return (protocol == HProtocols.Modern ? Modern.CypherInt(value) : Ancient.CypherInt(value));
        }
        public static byte[] CypherInt(this HProtocols protocol, byte[] source, int offset, int value)
        {
            if (protocol == HProtocols.Unknown) throw new Exception(UnsupportedProtocol);
            return (protocol == HProtocols.Modern ? Modern.CypherInt(source, offset, value) : Ancient.CypherInt(source, offset, value));
        }
        public static int DecypherInt(this HProtocols protocol, byte[] data)
        {
            if (protocol == HProtocols.Unknown) throw new Exception(UnsupportedProtocol);
            return (protocol == HProtocols.Modern ? Modern.DecypherInt(data) : Ancient.DecypherInt(data));
        }
        public static int DecypherInt(this HProtocols protocol, string encoded)
        {
            if (protocol == HProtocols.Unknown) throw new Exception(UnsupportedProtocol);
            return (protocol == HProtocols.Modern ? Modern.DecypherInt(encoded) : Ancient.DecypherInt(encoded));
        }
        public static int DecypherInt(this HProtocols protocol, byte[] data, int offset)
        {
            if (protocol == HProtocols.Unknown) throw new Exception(UnsupportedProtocol);
            return (protocol == HProtocols.Modern ? Modern.DecypherInt(data, offset) : Ancient.DecypherInt(data, offset));
        }
        public static int DecypherInt(this HProtocols protocol, byte first, params byte[] data)
        {
            if (protocol == HProtocols.Unknown) throw new Exception(UnsupportedProtocol);
            if (protocol == HProtocols.Modern && data.Length < 3) throw new Exception(DecIntDataTooSmall);
            return (protocol == HProtocols.Modern ? Modern.DecypherInt(first, data[0], data[1], data[2]) : Ancient.DecypherInt(first, data));
        }
    }
}