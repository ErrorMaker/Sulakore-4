using System;
using System.IO;
using System.Net;
using System.Linq;
using Sulakore.Protocol;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using Sulakore.Protocol.Encryption;
using System.Diagnostics;

namespace Sulakore.Communication
{
    public class HConnection : HTriggers, IHConnection, IDisposable
    {
        public event EventHandler Connected;
        protected virtual void OnConnected(EventArgs e)
        {
            EventHandler handler = Connected;

            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<DataToEventArgs> DataToClient;
        protected virtual void OnDataToClient(DataToEventArgs e)
        {
            EventHandler<DataToEventArgs> handler = DataToClient;

            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<DataToEventArgs> DataToServer;
        protected virtual void OnDataToServer(DataToEventArgs e)
        {
            EventHandler<DataToEventArgs> handler = DataToServer;

            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<DisconnectedEventArgs> Disconnected;
        protected virtual void OnDisconnected(DisconnectedEventArgs e)
        {
            EventHandler<DisconnectedEventArgs> handler = Disconnected;

            if (handler != null)
                handler(this, e);
        }

        private TcpListenerEx _htcpExt;
        private Socket _clientS, _serverS;
        private int _toClientS, _toServerS, _socketCount;
        private bool _hasOfficialSocket, _disconnectAllowed;
        private byte[] _clientB, _serverB, _clientC, _serverC;

        private const TaskCreationOptions _eventCallFlags = (TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness);

        private readonly object _resetHostLock, _disconnectLock, _sendToClientLock, _sendToServerLock;

        private static readonly string HostsPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\drivers\\etc\\hosts";

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
        public HProtocol Protocol { get; private set; }

        public HConnection(string host, int port)
        {
            _resetHostLock = new object();
            _disconnectLock = new object();
            _sendToClientLock = new object();
            _sendToServerLock = new object();

            Host = host;
            Port = port;
            ResetHost();

            Addresses = Dns.GetHostAddresses(host)
                .Select(ip => ip.ToString()).ToArray();
        }

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
            return SendToClient(HMessage.Construct(header, HDestination.Client, Protocol, chunks));
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
            return SendToServer(HMessage.Construct(header, HDestination.Server, Protocol, chunks));
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
            if (!_disconnectAllowed) return;
            _disconnectAllowed = false;

            lock (_disconnectLock)
            {
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
                Protocol = HProtocol.Modern;
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
                        base.Dispose(false);
                    }
                }
            }
        }

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

                        Protocol = isModern ? HProtocol.Modern : HProtocol.Ancient;
                        OnConnected(EventArgs.Empty);
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

                if (_toServerS == 3 && Protocol == HProtocol.Modern)
                {
                    int dLength = data.Length >= 6 ? Modern.DecypherInt(data) : 0;
                    RequestEncrypted = (dLength != data.Length - 4);
                }

                byte[][] chunks = RequestEncrypted ? new[] { data } : ByteUtils.Split(ref _clientC, data, HDestination.Server, Protocol);
                #endregion

                foreach (byte[] chunk in chunks)
                    ProcessOutgoing(chunk);

                ReadClientData();
            }
            catch { Disconnect(); }
        }
        public override void ProcessOutgoing(byte[] data)
        {
            ++_toServerS;

            if (!RequestEncrypted)
                Task.Factory.StartNew(() => base.ProcessOutgoing(data), TaskCreationOptions.LongRunning)
                    .ContinueWith(OnException, TaskContinuationOptions.OnlyOnFaulted);

            if (DataToServer == null) SendToServer(data);
            else
            {
                var e = new DataToEventArgs(data, HDestination.Server, _toServerS);
                try { OnDataToServer(e); }
                catch
                {
                    e.Cancel = true;
                    SendToServer(data);
                }
                finally
                {
                    if (!e.Cancel)
                        SendToServer(e.Packet.ToBytes());
                }
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

                if (_toClientS == 2 && Protocol == HProtocol.Modern)
                {
                    int dLength = data.Length >= 6 ? Modern.DecypherInt(data) : 0;
                    ResponseEncrypted = (dLength != data.Length - 4);
                }

                byte[][] chunks = ResponseEncrypted ? new[] { data } : ByteUtils.Split(ref _serverC, data, HDestination.Client, Protocol);
                #endregion

                foreach (byte[] chunk in chunks)
                    ProcessIncoming(chunk);

                ReadServerData();
            }
            catch { Disconnect(); }
        }
        public override void ProcessIncoming(byte[] data)
        {
            ++_toClientS;

            if (!ResponseEncrypted)
                Task.Factory.StartNew(() => base.ProcessIncoming(data), TaskCreationOptions.LongRunning)
                    .ContinueWith(OnException, TaskContinuationOptions.OnlyOnFaulted);

            if (DataToClient == null) SendToClient(data);
            else
            {
                var e = new DataToEventArgs(data, HDestination.Client, _toClientS);
                try { OnDataToClient(e); }
                catch
                {
                    e.Cancel = true;
                    SendToClient(data);
                }
                finally
                {
                    if (!e.Cancel)
                        SendToClient(e.Packet.ToBytes());
                }
            }
        }

        private void OnException(Task task)
        {
            Debug.WriteLine(task.Exception.ToString());
        }

        new public void Dispose()
        {
            Dispose(true);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SKore.Unsubscribe(ref Connected);
                SKore.Unsubscribe(ref DataToClient);
                SKore.Unsubscribe(ref DataToServer);
                SKore.Unsubscribe(ref Disconnected);
                Disconnect();

                Host = null;
                Addresses = null;
                Port = SocketSkip = 0;
                CaptureEvents = false;
            }
            base.Dispose(disposing);
        }
    }
}