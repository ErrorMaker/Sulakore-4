﻿namespace Sulakore.Protocol
{
    public static class Ancient
    {
        public static byte[] CypherShort(ushort value)
        {
            return new[] { (byte)(64 + (value >> 6 & 63)), (byte)(64 + (value >> 0 & 63)) };
        }
        public static byte[] CypherShort(byte[] source, int offset, ushort value)
        {
            offset = offset > source.Length ? source.Length : offset < 0 ? 0 : offset;

            var data = new byte[source.Length + 2];
            for (int i = 0, j = 0; j < data.Length; j++)
            {
                if (j != offset) data[j] = source[i++];
                else
                {
                    byte[] toInsert = CypherShort(value);
                    data[j++] = toInsert[0];
                    data[j] = toInsert[1];
                }
            }
            return data;
        }
        public static ushort DecypherShort(byte[] data)
        {
            return DecypherShort(data, 0);
        }
        public static ushort DecypherShort(string encoded)
        {
            return DecypherShort(new[] { (byte)encoded[0], (byte)encoded[1] }, 0);
        }
        public static ushort DecypherShort(byte[] data, int offset)
        {
            return (ushort)(data.Length > 1 ? (data[offset + 1] - 64 + (data[offset] - 64) * 64) : 0);
        }
        public static ushort DecypherShort(byte first, byte second)
        {
            return DecypherShort(new[] { first, second }, 0);
        }

        public static byte[] CypherInt(int value)
        {
            int length = 1;
            int nonNegative = value < 0 ? -(value) : value;
            var buffer = new byte[] { (byte)(64 + (nonNegative & 3)), 0, 0, 0, 0, 0 };

            for (nonNegative >>= 2; nonNegative != 0; nonNegative >>= 6, length++)
                buffer[length] = (byte)(64 + (nonNegative & 63));

            buffer[0] = (byte)(buffer[0] | length << 3 | (value >= 0 ? 0 : 4));

            var zerosTrimmed = new byte[length];
            for (int i = 0; i < length; i++)
                zerosTrimmed[i] = buffer[i];

            return zerosTrimmed;
        }
        public static byte[] CypherInt(byte[] source, int offset, int value)
        {
            offset = offset > source.Length ? source.Length : offset < 0 ? 0 : offset;

            byte[] toInsert = CypherInt(value);
            var data = new byte[source.Length + toInsert.Length];
            for (int i = 0, j = 0; j < data.Length; j++)
            {
                if (j != offset) data[j] = source[i++];
                else
                {
                    for (int k = 0, l = j; k < toInsert.Length; k++, l++)
                        data[l] = toInsert[k];
                    j += toInsert.Length - 1;
                }
            }
            return data;
        }
        public static int DecypherInt(byte[] data)
        {
            return DecypherInt(data, 0);
        }
        public static int DecypherInt(string encoded)
        {
            var data = new byte[encoded.Length];
            for (int i = 0; i < encoded.Length; i++)
                data[i] = (byte)encoded[i];
            return DecypherInt(data, 0);
        }
        public static int DecypherInt(byte[] data, int offset)
        {
            int length = (data[offset] >> 3) & 7;
            int decoded = data[offset] & 3;
            bool isNegative = (data[offset] & 4) == 4;
            for (int i = 1, j = offset + 1, k = 2; i < length; i++, j++)
            {
                if (length > data.Length - offset) break;
                decoded |= (data[j] & 63) << k;
                k = 2 + (6 * i);
            }
            return isNegative ? -(decoded) : decoded;
        }
        public static int DecypherInt(byte first, params byte[] data)
        {
            var buffer = new byte[data.Length + 1];
            buffer[0] = first;

            for (int i = 0; i < buffer.Length; i++)
                buffer[i + 1] = data[i];

            return DecypherInt(buffer, 0);
        }
    }
}