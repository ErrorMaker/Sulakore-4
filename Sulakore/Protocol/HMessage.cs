using System;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using Sulakore.Communication;

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
                if (!IsCorrupted && _header != value)
                {
                    _header = value;
                    _buffer.RemoveRange(0, 2);
                    _buffer.InsertRange(0, Protocol.CypherShort(value));
                    Reconstruct();
                }
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

        #region Packet Identifiers
        public bool IsHostSay
        {
            get
            {
                try
                {
                    int packet = 0;
                    if (ReadString(packet) == string.Empty || ReadString(ref packet).Length > 100) return false;
                    if ((ReadInt(packet) == 1 || ReadInt(packet) == 2 || ReadInt(packet) == 9 || ReadInt(packet) == 10) && ReadInt(packet) != 29) return false;
                    ReadInt(ref packet);
                    ReadInt(ref packet);
                    if (Length > ReadString(0).Length + 12) return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsHostSign
        {
            get
            {
                try
                {
                    int position = 0;
                    if (ReadString(ref position) != "OwnAvatarMenu") return false;
                    if (ReadString(ref position) != "click") return false;
                    if (ReadString(ref position) != "sign") return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsHostExit
        {
            get
            {
                try
                {
                    if (ReadInt(0) != -1) return false;
                    if (Length != 6) return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsHostShout
        {
            get
            {
                try
                {
                    int position = 0;
                    if (ReadString(position) == string.Empty || ReadString(position).Length > 100) return false;
                    ReadString(ref position);
                    if ((ReadInt(position) == 1 || ReadInt(position) == 2 || ReadInt(position) == 9 || ReadInt(position) == 10) && ReadInt(position) != 29) return false;
                    ReadInt(ref position);
                    if (Length > ReadString(0).Length + 8) return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsHostDance
        {
            get
            {
                try
                {
                    int position = 0;
                    if (ReadString(ref position) != "OwnAvatarMenu") return false;
                    if (ReadString(ref position) != "click") return false;
                    if (!ReadString(ref position).Contains("dance_")) return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsHostTrade
        {
            get
            {
                try
                {
                    int position = 0;
                    if (ReadString(ref position) != "InfoStand") return false;
                    if (ReadString(ref position) != "click") return false;
                    if (ReadString(ref position) != "RWUAM_START_TRADING") return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsCoordinate
        {
            get
            {
                try
                {
                    if (ReadInt(0) < 0 || ReadInt(0) >= 2000) return false;
                    if (ReadInt(4) < 0 || ReadInt(4) >= 2000) return false;
                    if (Length != 10) return false;
                    return true;
                }
                catch
                { return false; }
            }
        }
        public bool IsHostKicked
        {
            get
            {
                try
                {
                    if (ReadInt(0) != 4008) return false;
                    if (Length != 6) return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsHostGesture
        {
            get
            {
                try
                {
                    int position = 0;
                    if (ReadString(ref position) != "OwnAvatarMenu") return false;
                    if (ReadString(ref position) != "click") return false;
                    if (ReadString(position) != "wave" && ReadString(position) != "idle" && ReadString(position) != "blow" && ReadString(position) != "laugh") return false;
                    ReadString(ref position);
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsHostNavigate
        {
            get
            {
                try
                {
                    int position = 0;
                    if (ReadString(ref position) != "Navigation") return false;
                    ReadString(ref position);
                    if (!ReadString(ref position).Contains("go.")) return false;
                    if (ReadString(ref position).Length < 1) return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsHostBanPlayer
        {
            get
            {
                try
                {
                    int position = 0;
                    if (ReadString(ref position) != "InfoStand") return false;
                    if (ReadString(ref position) != "click") return false;
                    if (!ReadString(ref position).Contains("RWUAM_BAN_USER")) return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsHostMutePlayer
        {
            get
            {
                try
                {
                    int position = 0;
                    if (ReadString(ref position) != "InfoStand") return false;
                    if (ReadString(ref position) != "click") return false;
                    if (!ReadString(ref position).Contains("RWUAM_MUTE_USER_")) return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsHostKickPlayer
        {
            get
            {
                try
                {
                    int position = 0;
                    if (ReadString(ref position) != "InfoStand") return false;
                    if (ReadString(ref position) != "click") return false;
                    if (ReadString(ref position) != "RWUAM_KICK_USER") return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsHostChangeData
        {
            get
            {
                try
                {
                    int position = 0;
                    if (ReadInt(ref position) != -1) return false;
                    if (!ReadString(position).Contains("hd-") || !ReadString(position).Contains("ch-") || !ReadString(position).Contains("lg-")) return false;
                    ReadString(ref position);
                    if (ReadString(position) != "m" && ReadString(position) != "f") return false;
                    ReadString(ref position);
                    if (ReadString(ref position).Length > 76) return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsHostChangeMotto
        {
            get
            {
                try
                {
                    if (ReadString(0).Length > 38) return false;
                    if (Length != ReadString(0).Length + 4) return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsPossiblePlayerId
        {
            get
            {
                try
                {
                    if (ReadInt(0) < 0) return false;
                    if (Length != 6) return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsHostChangeStance
        {
            get
            {
                try
                {
                    int position = 0;
                    if (ReadString(ref position) != "OwnAvatarMenu") return false;
                    if (ReadString(ref position) != "click") return false;
                    if (ReadString(position) != "sit" && ReadString(position) != "stand") return false;
                    ReadString(ref position);
                    if (ReadString(ref position) != string.Empty) return false;
                    if (ReadInt(ref position) != 0) return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsHostMoveFurniture
        {
            get
            {
                try
                {
                    if (ReadInt(0) <= 0) return false;
                    if (ReadInt(4) < 0) return false;
                    if (ReadInt(8) < 0) return false;
                    if (ReadInt(12) < 0 || ReadInt(12) > 7) return false;
                    if (Length != 18) return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsHostChangeClothes
        {
            get
            {
                try
                {
                    if (ReadString(0) != "M" && ReadString(0) != "F") return false;
                    if (!ReadString(3).Contains("hd-") || !ReadString(3).Contains("ch-") || !ReadString(3).Contains("lg-")) return false;
                    if (Length > ReadString(0).Length + ReadString(3).Length + 6) return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }

        public bool IsPlayerSign
        {
            get
            {
                try
                {
                    int position = 0;
                    if (ReadInt(ref position) != 1) return false;
                    ReadInt(ref position);
                    ReadInt(ref position);
                    ReadInt(ref position);
                    if (ReadString(ref position)[1] != '.') return false;
                    if (ReadInt(position) > 7) return false;
                    if (ReadInt(ref position) != ReadInt(ref position)) return false;
                    if (!ReadString(position).Contains("/sign")) return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsPlayerWalking
        {
            get
            {
                try
                {
                    int position = 0;
                    if (ReadInt(ref position) <= 0) return false;
                    ReadInt(ref position);
                    ReadInt(ref position);
                    ReadInt(ref position);
                    if (ReadString(ref position)[1] != '.') return false;
                    if (ReadInt(position) > 7) return false;
                    if (ReadInt(ref position) != ReadInt(ref position)) return false;
                    if (!ReadString(position).Contains("/mv")) return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsPlayerTalking
        {
            get
            {
                try
                {
                    int position = 0;
                    ReadInt(ref position);
                    if (ReadString(ref position) == string.Empty) return false;
                    if (ReadInt(ref position) != 0) return false;
                    if ((ReadInt(position) == 1 || ReadInt(position) == 2 || ReadInt(position) == 9 || ReadInt(position) == 10) && ReadInt(position) != 29) return false;
                    ReadInt(ref position);
                    if (ReadInt(ref position) != 0) return false;
                    if (ReadInt(ref position) == 65535 && ReadInt(ref position) != 65535) return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsPlayerEntering
        {
            get
            {
                try
                {
                    int position = 0;
                    if (ReadInt(ref position) != 1) return false;
                    ReadInt(ref position);
                    ReadString(ref position);
                    ReadString(ref position);
                    if (!ReadString(ref position).Contains("hd-")) return false;
                    ReadInt(ref position);
                    ReadInt(ref position);
                    ReadInt(ref position);
                    if (!ReadString(ref position).Contains(".")) return false;
                    ReadInt(ref position);
                    ReadInt(ref position);
                    if (ReadString(position) != "m" && ReadString(ref position) != "f") return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsPlayerChangeData
        {
            get
            {
                try
                {
                    int position = 0;
                    if (ReadInt(ref position) == -1) return false;
                    if (!ReadString(position).Contains("ch-") || !ReadString(position).Contains("hd-") || !ReadString(position).Contains("lg-")) return false;
                    ReadString(ref position);
                    if (ReadString(position) != "m" && ReadString(position) != "f") return false;
                    ReadString(ref position);
                    if (ReadString(ref position).Length > 76) return false;
                    ReadInt(ref position);
                    return true;
                }
                catch
                { return false; }
            }
        }
        public bool IsPlayerChangeStance
        {
            get
            {
                try
                {
                    int position = 0;
                    if (ReadInt(ref position) <= 0) return false;
                    ReadInt(ref position);
                    ReadInt(ref position);
                    ReadInt(ref position);
                    if (ReadString(ref position)[1] != '.') return false;
                    if (ReadInt(position) > 7) return false;
                    if ((ReadInt(ref position) < 0 || ReadInt(position) > 7) || (ReadInt(position + 4) < 0 || ReadInt(ref position) > 7)) return false;
                    if (ReadString(position).Contains("/mv") || ReadString(position).Contains("/sign")) return false;
                    if (ReadString(position).Length != 13 && ReadString(position).Length != 2 && !ReadString(position).Contains("/sit")) return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsPlayerMoveFurniture
        {
            get
            {
                try
                {
                    int position = 20;
                    if (ReadInt(0) <= 0) return false;
                    if (ReadInt(4) < 0) return false;
                    if (ReadInt(8) < 0) return false;
                    if (ReadInt(12) < 0) return false;
                    if (ReadInt(16) < 0 || ReadInt(16) > 7) return false;
                    if (!ReadString(ref position).Contains(".")) return false;
                    if (!ReadString(ref position).Contains(".")) return false;
                    if (ReadInt(ref position) < 0) return false;
                    if (ReadInt(ref position) < 0) return false;
                    ReadString(ref position);
                    ReadInt(ref position);
                    ReadInt(ref position);
                    if (ReadInt(position) <= 0) return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsPlayerDropFurniture
        {
            get
            {
                try
                {
                    int position = 20;
                    if (ReadInt(0) <= 0) return false;
                    if (ReadInt(4) < 0) return false;
                    if (ReadInt(8) < 0) return false;
                    if (ReadInt(12) < 0) return false;
                    if (ReadInt(16) < 0 || ReadInt(16) > 7) return false;
                    if (!ReadString(ref position).Contains(".")) return false;
                    if (!ReadString(ref position).Contains(".")) return false;
                    if (ReadInt(ref position) < 0) return false;
                    if (ReadInt(ref position) < 0) return false;
                    ReadString(ref position);
                    ReadInt(ref position);
                    ReadInt(ref position);
                    if (ReadInt(ref position) <= 0) return false;
                    if (ReadString(position).Length <= 0) return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
        }
        public bool IsMultiplePlayerMovement
        {
            get
            {
                try
                {
                    string packet = ToString();
                    if (ReadInt(0) <= 1) return false;
                    if (!packet.Contains("/mv ") && !packet.Contains("/sit ") && !packet.Contains("/lay ")) return false;
                    return true;
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); return false; }
            }
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
            return Protocol.DecypherShort(Body[index++], Body[index++]);
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
        public static implicit operator byte[] (HMessage hmessage)
        {
            return hmessage.ToBytes();
        }

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
                            buffer.AddRange(protocol.CypherInt(value));
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
                                buffer.AddRange(protocol.CypherShort((ushort)value.Length));
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
            buffer.AddRange(protocol.CypherShort(header));

            buffer.AddRange(ConstructBody(destination, protocol, chunks));

            if (!isAncient || destination == HDestinations.Server)
                buffer.InsertRange(isAncient ? 1 : 0, protocol.CypherShort((ushort)(buffer.Count - (isAncient ? 1 : 0))));
            else if (buffer[buffer.Count - 1] != 1) buffer.Add(1);

            return buffer.ToArray();
        }
        #endregion
    }
}