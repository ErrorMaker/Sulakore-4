using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Sulakore.Protocol
{
    public class HMessage
    {
        #region Private Fields
        private ushort _header;
        private byte[] _bufferCache;
        private string _stringCache, _rawBody;

        private readonly bool _logWriting;
        private readonly List<byte> _buffer;
        private readonly List<object> _appended, _prepended;
        #endregion

        #region Public Properties
        public ushort Header
        {
            get { return _header; }
            set
            {
                if (IsCorrupted || _header == value) return;

                _header = value;
                _buffer.RemoveRange(0, 2);
                _buffer.InsertRange(0, Protocol == HProtocols.Ancient ? Ancient.CypherShort(value) : Modern.CypherShort(value));
                Reconstruct();
            }
        }
        public int Position { get; set; }
        public int Length { get; private set; }
        public byte[] Body { get; private set; }
        public bool IsCorrupted { get; private set; }
        public HProtocols Protocol { get; private set; }
        public HDestinations Destination { get; private set; }

        public object[] Written
        {
            get
            {
                var mergedWritten = new List<object>();
                mergedWritten.AddRange(_prepended);
                mergedWritten.AddRange(_appended);
                return mergedWritten.ToArray();
            }
        }
        public object[] Appended
        {
            get { return _appended.ToArray(); }
        }
        public object[] Prepended
        {
            get { return _prepended.ToArray(); }
        }
        #endregion

        #region Constructor(s)
        private HMessage()
        {
            _buffer = new List<byte>();
            _appended = new List<object>();
            _prepended = new List<object>();
        }

        public HMessage(byte[] data)
            : this(data, HDestinations.Unknown)
        { }

        public HMessage(string packet)
            : this(ToBytes(packet), HDestinations.Unknown)
        { }

        public HMessage(string packet, HDestinations destination)
            : this(ToBytes(packet), destination)
        { }

        public HMessage(byte[] data, HDestinations destination)
            : this()
        {
            if (data == null) throw new NullReferenceException();
            if (data.Length < 1) throw new Exception("The minimum amount of bytes required to initialize an HMessage instance is 1(One). If the amount of bytes passed is < 3(Three), and >= 1(One), it will be immediately be identified as a corrupted packet. { IsCorrupted = true }");

            Destination = destination;
            bool hasByteZero = data.Contains(byte.MinValue);
            bool isAncientHeader = !hasByteZero && data.Length == 2 && data[1] != 1;

            if (!isAncientHeader && data.Length >= 6 && Modern.DecypherInt(data) == data.Length - 4)
            {
                Protocol = HProtocols.Modern;

                _header = Modern.DecypherShort(data, 4);
                Append(ByteUtils.CopyBlock(data, 4, data.Length - 4));

                if (data.Length == 6)
                    _logWriting = true;
            }
            else if ((destination == HDestinations.Server && isAncientHeader) || (!hasByteZero && data.Length >= 5 && Ancient.DecypherShort(data, 1) == data.Length - 3))
            {
                Destination = HDestinations.Server;
                Protocol = HProtocols.Ancient;

                _header = Ancient.DecypherShort(data, isAncientHeader ? 0 : 3);
                Append(isAncientHeader ? data : ByteUtils.CopyBlock(data, 3, data.Length - 3));

                if (data.Length == 5 || isAncientHeader)
                    _logWriting = true;
            }
            else if (isAncientHeader || (!hasByteZero && data.Length >= 3 && data[data.Length - 1] == 1 && Destination != HDestinations.Server))
            {
                Destination = HDestinations.Client;
                Protocol = HProtocols.Ancient;

                if (isAncientHeader) data = new byte[] { data[0], data[1], 1 };
                _header = Ancient.DecypherShort(data);
                Append(data);

                if (data.Length == 3 || isAncientHeader)
                    _logWriting = true;
            }
            else
            {
                Body = data;
                _bufferCache = data;
                IsCorrupted = true;
                Length = data.Length;
                _buffer.AddRange(data);
                _stringCache = ToString(data);
            }
        }

        public HMessage(ushort header, params object[] chunks)
            : this(header, HDestinations.Unknown, HProtocols.Modern, chunks)
        { }

        public HMessage(ushort header, HDestinations destination, HProtocols protocol, params object[] chunks)
            : this(Construct(header, destination, protocol, chunks), destination)
        {
            _logWriting = true;
            _appended.AddRange(chunks);
        }
        #endregion

        #region Reading Methods
        public int ReadInt()
        {
            int index = Position;
            int value = ReadInt(ref index);
            Position = index;
            return value;
        }
        public int ReadInt(int index)
        {
            return ReadInt(ref index);
        }
        public int ReadInt(ref int index)
        {
            if (IsCorrupted || index >= Body.Length) return 0;

            switch (Protocol)
            {
                case HProtocols.Modern:
                {
                    if (index + 4 > Body.Length) return 0;
                    return Modern.DecypherInt(Body[index++], Body[index++], Body[index++], Body[index++]);
                }
                case HProtocols.Ancient:
                {
                    int length = (Body[index] >> 3) & 7;
                    if (length < 1) length++;
                    if (index + length > Body.Length) return 0;

                    int value = Ancient.DecypherInt(Body, index);
                    index += length;

                    return value;
                }
                default: return 0;
            }
        }

        public int ReadShort()
        {
            int index = Position;
            int value = ReadShort(ref index);
            Position = index;
            return value;
        }
        public int ReadShort(int index)
        {
            return ReadShort(ref index);
        }
        public int ReadShort(ref int index)
        {
            if (IsCorrupted) return 0;

            byte[] chunk = new byte[] { Body[index++], Body[index++] };
            return Protocol == HProtocols.Ancient ? Ancient.DecypherShort(chunk) : Modern.DecypherShort(chunk);
        }

        public bool ReadBool()
        {
            int index = Position;
            bool value = ReadBool(ref index);
            Position = index;
            return value;
        }
        public bool ReadBool(int index)
        {
            return ReadBool(ref index);
        }
        public bool ReadBool(ref int index)
        {
            if (IsCorrupted) return false;

            switch (Protocol)
            {
                case HProtocols.Modern: return Body[index++] == 1;
                case HProtocols.Ancient: return Body[index++] == 'I';
                default: return false;
            }
        }

        public string ReadString()
        {
            int index = Position;
            string value = ReadString(ref index);
            Position = index;
            return value;
        }
        public string ReadString(int index)
        {
            return ReadString(ref index);
        }
        public string ReadString(ref int index)
        {
            if (IsCorrupted) return string.Empty;
            if (Protocol == HProtocols.Modern || (Protocol == HProtocols.Ancient && Destination == HDestinations.Server))
            {
                int sLength = ReadShort(ref index);
                byte[] sData = ByteUtils.CopyBlock(Body, (index += sLength) - sLength, sLength);
                return Encoding.Default.GetString(sData);
            }

            if (Protocol != HProtocols.Ancient || Destination != HDestinations.Client) return string.Empty;

            string chunk = _rawBody.Substring(index).Split((char)2)[0];
            index += chunk.Length + 1;
            return chunk;
        }
        #endregion

        #region Writing Methods
        public void Append(int value)
        {
            Append(new object[] { value });
        }
        public void Append(bool value)
        {
            Append(new object[] { value });
        }
        public void Append(string value)
        {
            Append(new object[] { value });
        }
        public void Append(params object[] chunks)
        {
            if (IsCorrupted) return;
            if (_logWriting) _appended.AddRange(chunks);
            byte[] constructed = ConstructBody(Destination, Protocol, chunks);
            if (Protocol == HProtocols.Ancient && Destination == HDestinations.Client)
            {
                _buffer.InsertRange(_buffer.Count - 1, constructed);
                Reconstruct();
            }
            else Append(constructed);
        }

        public void Prepend(int value)
        {
            Prepend(new object[] { value });
        }
        public void Prepend(bool value)
        {
            Prepend(new object[] { value });
        }
        public void Prepend(string value)
        {
            Prepend(new object[] { value });
        }
        public void Prepend(params object[] chunks)
        {
            if (IsCorrupted) return;
            if (_logWriting) _prepended.AddRange(chunks);
            byte[] constructed = ConstructBody(Destination, Protocol, chunks);
            Prepend(constructed);
        }
        #endregion

        #region Private Methods
        private void Reconstruct()
        {
            _bufferCache = null;
            _stringCache = null;

            Length = _buffer.Count;
            if (Body != null && Body.Length == Length - 2) return;

            Body = ByteUtils.CopyBlock(_buffer.ToArray(), 2, Length - 2);
            _rawBody = Encoding.Default.GetString(Body);
        }
        private void Append(params byte[] chunk)
        {
            _buffer.AddRange(chunk);
            Reconstruct();
        }
        private void Prepend(params byte[] chunk)
        {
            _buffer.InsertRange(2, chunk);
            Position += chunk.Length;
            Reconstruct();
        }
        #endregion

        #region Instance Formatters
        public byte[] ToBytes()
        {
            return _bufferCache ?? (_bufferCache = Construct(_header, Destination, Protocol, Body));
        }
        public override string ToString()
        {
            return _stringCache ?? (_stringCache = ToString(ToBytes()));
        }
        #endregion

        #region Static Methods
        public static byte[] ToBytes(string packet)
        {
            for (int i = 0; i <= 13; i++)
                packet = packet.Replace("[" + i + "]", ((char)i).ToString());
            return Encoding.Default.GetBytes(packet);
        }
        public static string ToString(byte[] packet)
        {
            string result = Encoding.Default.GetString(packet);
            for (int i = 0; i <= 13; i++)
                result = result.Replace(((char)i).ToString(), "[" + i + "]");
            return result;
        }

        public static byte[] Construct(ushort header, params object[] chunks)
        {
            return Construct(header, HDestinations.Unknown, HProtocols.Modern, chunks);
        }
        public static byte[] ConstructBody(HDestinations destination, HProtocols protocol, params object[] chunks)
        {
            if (protocol == HProtocols.Unknown) throw new Exception("You must specify a supported Sulakore.Protocol.HProtocols object for this method. (Ancient / Modern)");
            if (protocol == HProtocols.Ancient && destination == HDestinations.Unknown) throw new Exception("Cannot construct the body of a Sulakore.Protocol.HProtocols.Ancient type packet without a valid Sulakore.Protocol.HDestinations object. (Client / Server)");

            var buffer = new List<byte>();
            bool isAncient = (protocol == HProtocols.Ancient);

            for (int i = 0; i < chunks.Length; i++)
            {
                object chunk = chunks[i];
                if (chunk == null) throw new NullReferenceException(string.Format("Unable to encode a null object. {{ Index = {0} }}", i));

                var data = chunk as byte[];
                if (data != null) buffer.AddRange(data);
                else
                {
                    switch (Type.GetTypeCode(chunk.GetType()))
                    {
                        case TypeCode.Int32:
                        {
                            var value = (int)chunk;
                            buffer.AddRange(protocol == HProtocols.Ancient ? Ancient.CypherInt(value) : Modern.CypherInt(value));
                            break;
                        }
                        case TypeCode.Boolean:
                        {
                            var value = (bool)chunk;
                            buffer.Add(isAncient ? (byte)(value ? 73 : 72) : Convert.ToByte(value));
                            break;
                        }
                        case TypeCode.Byte:
                        {
                            var value = (byte)chunk;
                            buffer.Add(value);
                            break;
                        }
                        default:
                        {
                            string value = chunk.ToString();
                            if (!isAncient || destination == HDestinations.Server)
                            {
                                ushort valueLength = (ushort)value.Length;
                                buffer.AddRange(protocol == HProtocols.Ancient ? Ancient.CypherShort(valueLength) : Modern.CypherShort(valueLength));
                                buffer.AddRange(Encoding.Default.GetBytes(value));
                            }
                            else
                            {
                                buffer.AddRange(Encoding.Default.GetBytes(value));
                                buffer.Add(2);
                            }
                            break;
                        }
                    }
                }
            }
            return buffer.ToArray();
        }
        public static byte[] Construct(ushort header, HDestinations destination, HProtocols protocol, params object[] chunks)
        {
            if (protocol == HProtocols.Unknown) throw new Exception("You must specify a supported Sulakore.Protocol.HProtocols object for this method. (Ancient / Modern)");
            if (protocol == HProtocols.Ancient && destination == HDestinations.Unknown) throw new Exception("Cannot construct the body of a Sulakore.Protocol.HProtocols.Ancient type packet without a valid Sulakore.Protocol.HDestinations object. (Client / Server)");

            var buffer = new List<byte>();
            bool isAncient = (protocol == HProtocols.Ancient);

            if (isAncient && destination == HDestinations.Server) buffer.Add(64);
            buffer.AddRange(protocol == HProtocols.Ancient ? Ancient.CypherShort(header) : Modern.CypherShort(header));

            buffer.AddRange(ConstructBody(destination, protocol, chunks));

            if (!isAncient || destination == HDestinations.Server)
                buffer.InsertRange(isAncient ? 1 : 0, isAncient ? Ancient.CypherShort((ushort)(buffer.Count - 1)) : Modern.CypherInt(buffer.Count));
            else if (buffer[buffer.Count - 1] != 1) buffer.Add(1);

            return buffer.ToArray();
        }
        #endregion
    }
}