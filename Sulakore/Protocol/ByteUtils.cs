using System.Linq;
using System.Collections.Generic;

namespace Sulakore.Protocol
{
    public static class ByteUtils
    {
        private static readonly object SplitLock;

        static ByteUtils()
        {
            SplitLock = new object();
        }

        public static byte[] Merge(byte[] source, params byte[][] chunks)
        {
            var data = new List<byte>();
            data.AddRange(source);
            foreach (byte[] chunk in chunks)
                data.AddRange(chunk);
            return data.ToArray();
        }
        public static byte[][] Split(ref byte[] cache, byte[] data, HDestination destination, HProtocol protocol)
        {
            lock (SplitLock)
            {
                if (cache != null)
                {
                    data = Merge(cache, data);
                    cache = null;
                }

                var chunks = new List<byte[]>();
                if (protocol == HProtocol.Ancient && destination == HDestination.Client)
                {
                    if (!data.Contains((byte)1)) cache = data;
                    else
                    {
                        var buffer = new List<byte>();
                        foreach (byte value in data)
                        {
                            buffer.Add(value);
                            if (value == 1)
                            {
                                chunks.Add(buffer.ToArray());
                                buffer.Clear();
                            }
                        }
                        if (buffer.Count > 0) cache = buffer.ToArray();
                    }
                }
                else
                {
                    bool isAncient = (protocol == HProtocol.Ancient);
                    int offset = isAncient ? 3 : 4;
                    int length = isAncient ? Ancient.DecypherShort(data, 1) : Modern.DecypherInt(data);

                    if (length == data.Length - offset) chunks.Add(data);
                    else
                    {
                        do
                        {
                            if (length > data.Length - offset) { cache = data; break; }
                            chunks.Add(CutBlock(ref data, 0, length + offset));
                            if (data.Length >= offset)
                                length = isAncient ? Ancient.DecypherShort(data, 1) : Modern.DecypherInt(data);
                        }
                        while (data.Length != 0);
                    }
                }
                return chunks.ToArray();
            }
        }

        public static byte[] CopyBlock(byte[] data, int offset, int length)
        {
            length = (length > data.Length) ? data.Length : length < 0 ? 0 : length;
            offset = offset + length >= data.Length ? data.Length - length : offset < 0 ? 0 : offset;

            var chunk = new byte[length];
            for (int i = 0; i < length; i++) chunk[i] = data[offset++];
            return chunk;
        }
        public static byte[] CutBlock(ref byte[] data, int offset, int length)
        {
            length = (length > data.Length) ? data.Length : length < 0 ? 0 : length;
            offset = offset + length >= data.Length ? data.Length - length : offset < 0 ? 0 : offset;

            var chunk = new byte[length];
            var trimmed = new byte[data.Length - length];
            for (int i = 0, j = offset; i < length; i++) chunk[i] = data[j++];
            for (int i = 0, j = 0; i < data.Length; i++)
            {
                if (i < offset || i >= offset + length)
                    trimmed[j++] = data[i];
            }
            data = trimmed;
            return chunk;
        }
    }
}