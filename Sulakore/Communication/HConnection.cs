using System;
using System.IO;
using System.Net;
using System.Linq;
using Sulakore.Protocol;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using Sulakore.Protocol.Encryption;

namespace Sulakore.Communication
{
    public sealed class HConnection : HTriggerBase, IHConnection, IDisposable
    {
        #region Game Connection Events
        public event EventHandler<EventArgs> Connected;
        public event EventHandler<DataToEventArgs> DataToClient;
        public event EventHandler<DataToEventArgs> DataToServer;
        public event EventHandler<DisconnectedEventArgs> Disconnected;
        #endregion

        #region Private Fields
        private TcpListenerEx _htcpExt;
        private Socket _clientS, _serverS;
        private int _toClientS, _toServerS, _socketCount;
        private bool _hasOfficialSocket, _disconnectAllowed;
        private byte[] _clientB, _serverB, _clientC, _serverC;
        private Dictionary<ushort, Action<HMessage>> _incomingEvents;
        private Dictionary<ushort, Action<HMessage>> _outgoingEvents;

        private const TaskCreationOptions _eventCallFlags = (TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness);

        private readonly object _disposeLock, _resetHostLock, _disconnectLock, _sendToClientLock, _sendToServerLock;

        private static readonly string HostsPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\drivers\\etc\\hosts";
        #endregion

        #region Public Properties
        public int Port { get; private set; }
        public string Host { get; private set; }
        public string[] Addresses { get; private set; }

        private Rc4 _serverDecrypt;
        public Rc4 ServerDecrypt
        {
            get { return _serverDecrypt; }
            set
            {
                if ((_serverDecrypt = value) != null)
                    ResponseEncrypted = false;
            }
        }
        public Rc4 ServerEncrypt { get; set; }

        private Rc4 _clientDecrypt;
        public Rc4 ClientDecrypt
        {
            get { return _clientDecrypt; }
            set
            {
                if ((_clientDecrypt = value) != null)
                    RequestEncrypted = false;
            }
        }
        public Rc4 ClientEncrypt { get; set; }

        public bool IsConnected
        {
            get { return _serverS != null && _serverS.Connected; }
        }
        public bool RequestEncrypted { get; private set; }
        public bool ResponseEncrypted { get; private set; }

        public int SocketSkip { get; set; }
        public bool CaptureEvents { get; set; }
        public HProtocols Protocol { get; private set; }
        #endregion

        #region Constructor(s)
        public HConnection(string host, int port)
            : base()
        {
            _disposeLock = new object();
            _resetHostLock = new object();
            _disconnectLock = new object();
            _sendToClientLock = new object();
            _sendToServerLock = new object();

            _incomingEvents = new Dictionary<ushort, Action<HMessage>>();
            _outgoingEvents = new Dictionary<ushort, Action<HMessage>>();

            Host = host;
            Port = port;
            ResetHost();

            Addresses = Dns.GetHostAddresses(host).Select(ip => ip.ToString()).ToArray();
        }
        #endregion

        #region Public Methods
        public void Connect(bool loopback = false)
        {
            if (loopback)
            {
                if (!File.Exists(HostsPath))
                    File.Create(HostsPath).Close();

                string[] hostsL = File.ReadAllLines(HostsPath);
                if (!Array.Exists(hostsL, ip => Addresses.Contains(ip)))
                {
                    List<string> gameIPs = Addresses.ToList(); if (!gameIPs.Contains(Host)) gameIPs.Add(Host);
                    string mapping = string.Format("127.0.0.1\t\t{{0}}\t\t#{0}[{{1}}/{1}]", Host, gameIPs.Count);
                    File.AppendAllLines(HostsPath, gameIPs.Select(ip => string.Format(mapping, ip, gameIPs.IndexOf(ip) + 1)));
                }
            }

            (_htcpExt = new TcpListenerEx(IPAddress.Any, Port)).Start();
            _htcpExt.BeginAcceptSocket(SocketAccepted, null);
            _disconnectAllowed = true;
        }

        public int SendToClient(byte[] data)
        {
            if (_clientS == null || !_clientS.Connected) return 0;
            lock (_sendToClientLock)
            {
                if (ServerEncrypt != null)
                    data = ServerEncrypt.SafeParse(data);

                return _clientS.Send(data);
            }
        }
        public int SendToClient(ushort header, params object[] chunks)
        {
            return SendToClient(HMessage.Construct(header, HDestinations.Client, Protocol, chunks));
        }

        public int SendToServer(byte[] data)
        {
            if (!IsConnected) return 0;

            lock (_sendToServerLock)
            {
                if (ClientEncrypt != null)
                    data = ClientEncrypt.SafeParse(data);

                return _serverS.Send(data);
            }
        }
        public int SendToServer(ushort header, params object[] chunks)
        {
            return SendToServer(HMessage.Construct(header, HDestinations.Server, Protocol, chunks));
        }

        public void AttachIncoming(ushort header, Action<HMessage> callback)
        {
            _incomingEvents[header] = callback;
        }
        public void DetachIncoming(ushort header)
        {
            if (_incomingEvents.ContainsKey(header))
                _incomingEvents.Remove(header);
        }

        public void AttachOutgoing(ushort header, Action<HMessage> callback)
        {
            _outgoingEvents[header] = callback;
        }
        public void DetachOutgoing(ushort header)
        {
            if (_outgoingEvents.ContainsKey(header))
                _outgoingEvents.Remove(header);
        }

        public void ResetHost()
        {
            lock (_resetHostLock)
            {
                if (Host == null || !File.Exists(HostsPath)) return;
                string[] hostsL = File.ReadAllLines(HostsPath).Where(line => !line.Contains(Host) && !line.StartsWith("127.0.0.1")).ToArray();
                File.WriteAllLines(HostsPath, hostsL);
            }
        }
        public void Disconnect()
        {
            lock (_disconnectLock)
            {
                if (!_disconnectAllowed) return;
                _disconnectAllowed = false;

                if (_clientS != null)
                {
                    _clientS.Shutdown(SocketShutdown.Both);
                    _clientS.Close();
                    _clientS = null;
                }
                if (_serverS != null)
                {
                    _serverS.Shutdown(SocketShutdown.Both);
                    _serverS.Close();
                    _serverS = null;
                }
                ResetHost();
                if (_htcpExt != null)
                {
                    _htcpExt.Stop();
                    _htcpExt = null;
                }
                Protocol = HProtocols.Unknown;
                _toClientS = _toServerS = _socketCount = 0;
                _clientB = _serverB = _clientC = _serverC = null;
                _hasOfficialSocket = RequestEncrypted = ResponseEncrypted = false;
                ClientEncrypt = ClientDecrypt = ServerEncrypt = ServerDecrypt = null;
                if (Disconnected != null)
                {
                    var disconnectedEventArgs = new DisconnectedEventArgs();
                    Disconnected(this, disconnectedEventArgs);
                    if (disconnectedEventArgs.UnsubscribeFromEvents)
                    {
                        SKore.Unsubscribe(ref Connected);
                        SKore.Unsubscribe(ref DataToClient);
                        SKore.Unsubscribe(ref DataToServer);
                        SKore.Unsubscribe(ref Disconnected);
                        base.Dispose();
                    }
                }
            }
        }
        public override sealed void Dispose()
        {
            lock (_disposeLock)
            {
                SKore.Unsubscribe(ref Connected);
                SKore.Unsubscribe(ref DataToClient);
                SKore.Unsubscribe(ref DataToServer);
                SKore.Unsubscribe(ref Disconnected);
                base.Dispose();
                Disconnect();

                Host = null;
                Addresses = null;
                Port = SocketSkip = 0;
                CaptureEvents = false;

                _outgoingEvents.Clear();
                _incomingEvents.Clear();
            }
        }
        #endregion

        #region Private Methods
        private void SocketAccepted(IAsyncResult iAr)
        {
            try
            {
                if (++_socketCount == SocketSkip)
                {
                    _htcpExt.EndAcceptSocket(iAr).Close();
                    _htcpExt.BeginAcceptSocket(SocketAccepted, null);
                }
                else
                {
                    _clientS = _htcpExt.EndAcceptSocket(iAr);
                    _serverS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    _serverS.BeginConnect(Addresses[0], Port, ConnectedToServer, null);
                }
            }
            catch
            {
                if (_htcpExt != null && _htcpExt.Active)
                {
                    if (_htcpExt.Pending()) _htcpExt.EndAcceptSocket(iAr).Close();
                    _htcpExt.BeginAcceptSocket(SocketAccepted, null);
                }
                else Disconnect();
            }
        }
        private void ConnectedToServer(IAsyncResult iAr)
        {
            _serverS.EndConnect(iAr);
            _serverB = new byte[1024];
            _clientB = new byte[512];
            ReadClientData();
            ReadServerData();
        }

        private void ReadClientData()
        {
            if (_clientS != null && _clientS.Connected)
                _clientS.BeginReceive(_clientB, 0, _clientB.Length, SocketFlags.None, DataFromClient, null);
        }
        private void DataFromClient(IAsyncResult iAr)
        {
            try
            {
                if (_clientS == null) return;
                int length = _clientS.EndReceive(iAr);
                if (length < 1) { Disconnect(); return; }

                byte[] data = ByteUtils.CopyBlock(_clientB, 0, length);
                #region Official Socket Check
                if (!_hasOfficialSocket)
                {
                    bool isModern = Modern.DecypherShort(data, 4) == 4000;
                    if (_hasOfficialSocket = (isModern || Ancient.DecypherShort(data, 3) == 206))
                    {
                        ResetHost();

                        _htcpExt.Stop();
                        _htcpExt = null;

                        Protocol = isModern ? HProtocols.Modern : HProtocols.Ancient;

                        if (Connected != null)
                            Connected(this, EventArgs.Empty);
                    }
                    else
                    {
                        SendToServer(data);
                        return;
                    }
                }
                #endregion
                #region Decrypt/Split
                if (ClientDecrypt != null)
                    ClientDecrypt.Parse(data);

                if (_toServerS == 3 && Protocol == HProtocols.Modern)
                {
                    int dLength = data.Length >= 6 ? Modern.DecypherInt(data) : 0;
                    RequestEncrypted = (dLength != data.Length - 4);
                }

                byte[][] chunks = RequestEncrypted ? new[] { data } : ByteUtils.Split(ref _clientC, data, HDestinations.Server, Protocol);
                #endregion

                foreach (byte[] chunk in chunks)
                    ProcessOutgoing(chunk);

                ReadClientData();
            }
            catch (Exception ex)
            {
                Disconnect();
                SKore.Debugger(ex.ToString());
            }
        }
        protected override void ProcessOutgoing(byte[] data)
        {
            try
            {
                ++_toServerS;
                if (_outgoingEvents.Count > 0 && !RequestEncrypted)
                {
                    int offset = (Protocol == HProtocols.Modern ? 4 : 3);
                    ushort header = offset == 4 ? Modern.DecypherShort(data, offset) : Ancient.DecypherShort(data, offset);
                    if (_outgoingEvents.ContainsKey(header))
                    {
                        var packet = new HMessage(data, HDestinations.Server);
                        Task.Factory.StartNew(() => _outgoingEvents[header](packet), _eventCallFlags)
                            .ContinueWith(OnException, TaskContinuationOptions.OnlyOnFaulted);
                    }
                }

                if (DataToServer == null) SendToServer(data);
                else
                {
                    var arguments = new DataToEventArgs(data, HDestinations.Server, _toServerS);
                    try { DataToServer(this, arguments); }
                    catch (Exception ex)
                    {
                        SendToServer(data);
                        SKore.Debugger(ex.ToString());
                        return;
                    }
                    if (!arguments.Cancel) SendToServer(arguments.Packet.ToBytes());
                }
            }
            catch (Exception ex) { SKore.Debugger(ex.ToString()); }
            finally
            {
                if (CaptureEvents && !RequestEncrypted)
                    Task.Factory.StartNew(() => base.ProcessOutgoing(data), _eventCallFlags)
                        .ContinueWith(OnException, TaskContinuationOptions.OnlyOnFaulted);
            }
        }

        private void ReadServerData()
        {
            if (IsConnected)
                _serverS.BeginReceive(_serverB, 0, _serverB.Length, SocketFlags.None, DataFromServer, null);
        }
        private void DataFromServer(IAsyncResult iAr)
        {
            try
            {
                if (_serverS == null) return;
                int length = _serverS.EndReceive(iAr);
                if (length < 1) { Disconnect(); return; }

                byte[] data = ByteUtils.CopyBlock(_serverB, 0, length);
                #region Official Socket Check
                if (!_hasOfficialSocket)
                {
                    SendToClient(data);
                    _htcpExt.BeginAcceptSocket(SocketAccepted, null);
                    return;
                }
                #endregion
                #region Decrypt/Split
                if (ServerDecrypt != null)
                    ServerDecrypt.Parse(data);

                if (_toClientS == 2 && Protocol == HProtocols.Modern)
                {
                    int dLength = data.Length >= 6 ? Modern.DecypherInt(data) : 0;
                    ResponseEncrypted = (dLength != data.Length - 4);
                }

                byte[][] chunks = ResponseEncrypted ? new[] { data } : ByteUtils.Split(ref _serverC, data, HDestinations.Client, Protocol);
                #endregion

                foreach (byte[] chunk in chunks)
                    ProcessIncoming(chunk);

                ReadServerData();
            }
            catch (Exception ex)
            {
                Disconnect();
                SKore.Debugger(ex.ToString());
            }
        }
        protected override void ProcessIncoming(byte[] data)
        {
            try
            {
                ++_toClientS;
                if (_incomingEvents.Count > 0 && !ResponseEncrypted)
                {
                    int headerOffset = (Protocol == HProtocols.Modern ? 4 : 0);
                    ushort header = headerOffset == 4 ? Modern.DecypherShort(data, 4) : Ancient.DecypherShort(data);
                    if (_incomingEvents.ContainsKey(header))
                    {
                        var packet = new HMessage(data, HDestinations.Client);
                        Task.Factory.StartNew(() => _incomingEvents[header](packet), _eventCallFlags)
                            .ContinueWith(OnException, TaskContinuationOptions.OnlyOnFaulted);
                    }
                }

                if (DataToClient == null) SendToClient(data);
                else
                {
                    var dataToEventArgs = new DataToEventArgs(data, HDestinations.Client, _toClientS);
                    try { DataToClient(this, dataToEventArgs); }
                    catch (Exception ex)
                    {
                        SendToClient(data);
                        SKore.Debugger(ex.ToString());
                        return;
                    }
                    if (!dataToEventArgs.Cancel) SendToClient(dataToEventArgs.Packet.ToBytes());
                }
            }
            catch (Exception ex) { SKore.Debugger(ex.ToString()); }
            finally
            {
                if (CaptureEvents && !ResponseEncrypted)
                    Task.Factory.StartNew(() => base.ProcessIncoming(data), _eventCallFlags)
                        .ContinueWith(OnException, TaskContinuationOptions.OnlyOnFaulted);
            }
        }

        private void OnException(Task task)
        {
            SKore.Debugger(task.Exception.ToString());
        }
        #endregion
    }
}