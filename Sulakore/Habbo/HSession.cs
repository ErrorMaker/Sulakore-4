using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using Sulakore.Protocol;
using System.Net.Sockets;
using System.Net.Security;
using Sulakore.Communication;
using System.Threading.Tasks;
using System.Collections.Generic;
using Sulakore.Protocol.Encryption;
using System.Collections.Specialized;
using System.Security.Cryptography.X509Certificates;

namespace Sulakore.Habbo
{
    public sealed class HSession : IPlayerSession, IHConnection, IDisposable
    {
        #region Connection Events
        public event EventHandler<EventArgs> Connected;
        public event EventHandler<DataToEventArgs> DataToServer;
        public event EventHandler<DataToEventArgs> DataToClient;
        public event EventHandler<DisconnectedEventArgs> Disconnected;
        #endregion

        #region Private Fields
        private Socket _serverS;
        private byte[] _serverB, _serverC;
        private int _toClientS, _toServerS;
        private bool _firstTimeLoadProfile;

        private readonly object _disconnectLock;
        #endregion

        #region Public Properties
        public bool IsLoggedIn
        {
            get
            {
                using (var webClientEx = new WebClientEx(Cookies))
                {
                    webClientEx.Headers["User-Agent"] = SKore.ChromeAgent;
                    string body = webClientEx.DownloadString(Hotel.ToUrl(true));
                    return body.Contains("window.location.replace('http:\\/\\/www.habbo." + Hotel.ToDomain() + "\\/me')");
                }
            }
        }

        private string _urlToken;
        public string UrlToken
        {
            get
            {
                if (!string.IsNullOrEmpty(_urlToken)) return _urlToken;
                LoadResource(HPages.Profile);
                return _urlToken;
            }
        }

        private string _csrfToken;
        public string CsrfToken
        {
            get
            {
                if (!string.IsNullOrEmpty(_csrfToken)) return _csrfToken;
                LoadResource(HPages.Profile);
                return _urlToken;
            }
        }

        private string _playerName;
        public string PlayerName
        {
            get
            {
                if (!string.IsNullOrEmpty(_playerName)) return _playerName;
                LoadResource(HPages.Profile);
                return _playerName;
            }
        }

        private string _lastSignIn;
        public string LastSignIn
        {
            get
            {
                if (!string.IsNullOrEmpty(_lastSignIn)) return _lastSignIn;
                LoadResource(HPages.Me);
                return _lastSignIn;
            }
        }

        private string _createdOn;
        public string CreatedOn
        {
            get
            {
                if (!string.IsNullOrEmpty(_createdOn)) return _createdOn;
                LoadResource(HPages.Home);
                return _createdOn;
            }
        }

        private string _userHash;
        public string UserHash
        {
            get
            {
                if (!string.IsNullOrEmpty(_userHash)) return _userHash;
                LoadResource(HPages.Client);
                return _userHash;
            }
        }

        private string _motto;
        public string Motto
        {
            get
            {
                if (!string.IsNullOrEmpty(_motto)) return _motto;
                LoadResource(HPages.Me);
                return _motto;
            }
        }

        private bool _homepageVisible;
        public bool HomepageVisible
        {
            get
            {
                if (!_firstTimeLoadProfile) return _homepageVisible;
                LoadResource(HPages.Profile);
                return _homepageVisible;
            }
        }

        private bool _friendRequestAllowed;
        public bool FriendRequestAllowed
        {
            get
            {
                if (!_firstTimeLoadProfile) return _friendRequestAllowed;
                LoadResource(HPages.Profile);
                return _friendRequestAllowed;
            }
        }

        private bool _showOnlineStatus;
        public bool ShowOnlineStatus
        {
            get
            {
                if (!_firstTimeLoadProfile) return _showOnlineStatus;
                LoadResource(HPages.Profile);
                return _showOnlineStatus;
            }
        }

        private bool _offlineMessaging;
        public bool OfflineMessaging
        {
            get
            {
                if (!_firstTimeLoadProfile) return _offlineMessaging;
                LoadResource(HPages.Profile);
                return _offlineMessaging;
            }
        }

        private bool _friendsCanFollow;
        public bool FriendsCanFollow
        {
            get
            {
                if (_firstTimeLoadProfile) return _friendsCanFollow;
                LoadResource(HPages.Profile);
                return _friendsCanFollow;
            }
        }

        private HGenders _gender;
        public HGenders Gender
        {
            get
            {
                if (_gender != HGenders.Unknown) return _gender;
                LoadResource(HPages.Profile);
                return _gender;
            }
        }

        private int _age;
        public int Age
        {
            get
            {
                if (_age != 0) return _age;
                LoadResource(HPages.Profile);
                return _age;
            }
        }

        public string Email { get; private set; }
        public string Password { get; private set; }
        public HHotels Hotel { get; private set; }

        public int PlayerId { get; private set; }
        public string ClientStarting { get; set; }
        public CookieContainer Cookies { get; private set; }

        Rc4 IHConnection.ServerEncrypt { get; set; }
        public Rc4 ServerDecrypt { get; set; }

        public Rc4 ClientEncrypt { get; set; }
        Rc4 IHConnection.ClientDecrypt { get; set; }

        private HGameData _gameData;
        public HGameData GameData
        {
            get
            {
                if (!_gameData.IsEmpty) return _gameData;
                LoadResource(HPages.Client);
                return _gameData;
            }
        }

        private string _flashClientUrl;
        public string FlashClientUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(_flashClientUrl)) return _flashClientUrl;
                LoadResource(HPages.Client);
                return _flashClientUrl;
            }
        }

        private string _flashClientBuild;
        public string FlashClientBuild
        {
            get
            {
                if (!string.IsNullOrEmpty(_flashClientBuild)) return _flashClientBuild;
                LoadResource(HPages.Client);
                return _flashClientBuild;
            }
        }

        private int _port;
        public int Port
        {
            get
            {
                if (_port != 0) return _port;
                LoadResource(HPages.Client);
                return _port;
            }
        }

        private string _host;
        public string Host
        {
            get
            {
                if (!string.IsNullOrEmpty(_host)) return _host;
                LoadResource(HPages.Client);
                return _host;
            }
        }

        private string[] _addresses;
        public string[] Addresses
        {
            get
            {
                if (_addresses != null && _addresses.Length > 0) return _addresses;
                LoadResource(HPages.Client);
                return _addresses;
            }
        }

        private string _ssoTicket;
        public string SsoTicket
        {
            get
            {
                if (!string.IsNullOrEmpty(_ssoTicket)) return _ssoTicket;
                LoadResource(HPages.Client);
                return _ssoTicket;
            }
        }

        private bool _receiveData;
        public bool ReceiveData
        {
            get { return _receiveData; }
            set
            {
                if (!IsConnected) _receiveData = false;
                else if (_receiveData != value)
                {
                    bool wasReceiving = _receiveData;
                    _receiveData = value;
                    if (!wasReceiving) ReadServerData();
                }
            }
        }

        public bool IsConnected
        {
            get { return _serverS != null && _serverS.Connected; }
        }

        bool IHConnection.RequestEncrypted
        {
            get { return false; }
        }
        public bool ResponseEncrypted { get; private set; }
        #endregion

        #region Constructor(s)
        static HSession()
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            ServicePointManager.ServerCertificateValidationCallback = ValidateCertificate;
        }
        public HSession(string email, string password, HHotels hotel)
        {
            Email = email;
            Password = password;
            Hotel = hotel;

            _firstTimeLoadProfile = true;

            _disconnectLock = new object();

            Cookies = new CookieContainer();
        }
        #endregion

        #region Indexer(s)
        public string this[HPages page]
        {
            get { return LoadResource(page); }
        }
        #endregion

        #region Public Methods
        public bool Login()
        {
            Dispose();
            Cookies.SetCookies(new Uri(Hotel.ToUrl()), SKore.GetIpCookie());

            try
            {
                #region Authentication
                byte[] postData = Encoding.Default.GetBytes(string.Format("credentials.username={0}&credentials.password={1}", Email, Password));
                var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/account/submit", Hotel.ToUrl(true)));
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = SKore.ChromeAgent;
                request.AllowAutoRedirect = false;
                request.CookieContainer = Cookies;
                request.Method = "POST";
                request.Proxy = null;

                using (Stream dataStream = request.GetRequestStream())
                    dataStream.Write(postData, 0, postData.Length);

                string body;
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    Cookies.Add(response.Cookies);
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                        body = streamReader.ReadToEnd();
                }
                #endregion

                if (body.Contains("useOrCreateAvatar"))
                {
                    #region Player Selection
                    PlayerId = int.Parse(body.GetChild("useOrCreateAvatar/", '?'));
                    request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/identity/useOrCreateAvatar/{1}?next=", Hotel.ToUrl(), PlayerId));
                    request.UserAgent = SKore.ChromeAgent;
                    request.CookieContainer = Cookies;
                    request.AllowAutoRedirect = false;
                    request.Method = "GET";

                    string redirectingTo;
                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        Cookies.Add(response.Cookies);
                        redirectingTo = response.Headers["Location"];
                    }
                    #endregion

                    #region Manual Redirect
                    request = (HttpWebRequest)WebRequest.Create(redirectingTo);
                    request.UserAgent = SKore.ChromeAgent;
                    request.CookieContainer = Cookies;
                    request.AllowAutoRedirect = false;
                    request.Method = "GET";

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        Cookies.Add(response.Cookies);

                        using (var streamReader = new StreamReader(response.GetResponseStream()))
                            body = streamReader.ReadToEnd();

                        if (redirectingTo.EndsWith("/me"))
                        {
                            HandleResource(HPages.Me, ref body);
                            return true;
                        }
                    }
                    #endregion

                    if (body.Contains("/account/updateIdentityProfileTerms"))
                    {
                        #region Accept Terms Of Service
                        postData = Encoding.Default.GetBytes("termsSelection=true");
                        request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/account/updateIdentityProfileTerms", Hotel.ToUrl(true)));
                        request.ContentType = "application/x-www-form-urlencoded";
                        request.Headers["Origin"] = Hotel.ToUrl(true);
                        request.UserAgent = SKore.ChromeAgent;
                        request.AllowAutoRedirect = false;
                        request.CookieContainer = Cookies;
                        request.Referer = redirectingTo;
                        request.Method = "POST";

                        using (Stream dataStream = request.GetRequestStream())
                            dataStream.Write(postData, 0, postData.Length);

                        using (var response = (HttpWebResponse)request.GetResponse())
                            Cookies.Add(response.Cookies);
                        #endregion
                    }
                    else if (body.Contains("/account/updateIdentityProfileEmail"))
                    {
                        #region Skip Email Verification
                        postData = Encoding.Default.GetBytes("email=&skipEmailChange=true");
                        request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/account/updateIdentityProfileEmail", Hotel.ToUrl(true)));
                        request.ContentType = "application/x-www-form-urlencoded";
                        request.Headers["Origin"] = Hotel.ToUrl(true);
                        request.UserAgent = SKore.ChromeAgent;
                        request.AllowAutoRedirect = false;
                        request.CookieContainer = Cookies;
                        request.Referer = redirectingTo;
                        request.Method = "POST";

                        using (Stream dataStream = request.GetRequestStream())
                            dataStream.Write(postData, 0, postData.Length);

                        using (var response = (HttpWebResponse)request.GetResponse())
                            Cookies.Add(response.Cookies);
                        #endregion
                    }

                    if (body.Contains("/account/updateIdentityProfileTerms") || body.Contains("/account/updateIdentityProfileEmail"))
                    {
                        #region Player Selection
                        request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/identity/useOrCreateAvatar/{1}?disableFriendLinking=false&combineIdentitiesSelection=2&next=&selectedAvatarId=", Hotel.ToUrl(), PlayerId));
                        request.UserAgent = SKore.ChromeAgent;
                        request.CookieContainer = Cookies;
                        request.AllowAutoRedirect = false;
                        request.Method = "GET";

                        using (var response = (HttpWebResponse)request.GetResponse())
                        {
                            Cookies.Add(response.Cookies);
                            redirectingTo = response.Headers["Location"];
                        }
                        #endregion

                        #region Manual Redirect
                        request = (HttpWebRequest)WebRequest.Create(redirectingTo);
                        request.UserAgent = SKore.ChromeAgent;
                        request.CookieContainer = Cookies;
                        request.AllowAutoRedirect = false;
                        request.Method = "GET";

                        using (var response = (HttpWebResponse)request.GetResponse())
                        {
                            Cookies.Add(response.Cookies);

                            using (var streamReader = new StreamReader(response.GetResponseStream()))
                                body = streamReader.ReadToEnd();

                            if (redirectingTo.EndsWith("/me"))
                            {
                                HandleResource(HPages.Me, ref body);
                                return true;
                            }
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex) { SKore.Debugger(ex.ToString()); }
            return false;
        }
        public Task<bool> LoginAsync()
        {
            return Task.Factory.StartNew(() => Login());
        }

        public void AddFriend(int playerId)
        {
            var formData = new NameValueCollection(1);
            formData.Add("accountId", playerId.ToString());

            DoXMLRequest(formData, Hotel.ToUrl() + "/myhabbo/friends/add");
        }
        public Task AddFriendAsync(int playerId)
        {
            return Task.Factory.StartNew(() => AddFriend(playerId));
        }

        public void RemoveFriend(int playerId)
        {
            var formData = new NameValueCollection(2);
            formData.Add("friendId", playerId.ToString());
            formData.Add("pageSize", "30");

            DoXMLRequest(formData, Hotel.ToUrl(true) + "/friendmanagement/ajax/deletefriends");

        }
        public Task RemoveFriendAsync(int playerId)
        {
            return Task.Factory.StartNew(() => RemoveFriend(playerId));
        }

        public void UpdateProfile(string motto, bool homepageVisible, bool friendRequestAllowed, bool showOnlineStatus, bool offlineMessaging, bool friendsCanFollow)
        {
            var formData = new NameValueCollection(9);
            formData.Add("__app_key", CsrfToken);
            formData.Add("urlToken", UrlToken);
            formData.Add("tab", "2");
            formData.Add("motto", motto);
            formData.Add("visibility", homepageVisible ? "EVERYONE" : "NOBODY");
            formData.Add("friendRequestsAllowed", friendRequestAllowed.ToString().ToLower());
            formData.Add("showOnlineStatus", showOnlineStatus.ToString().ToLower());
            formData.Add("persistentMessagingAllowed", offlineMessaging.ToString().ToLower());
            formData.Add("followFriendMode", Convert.ToByte(friendsCanFollow).ToString());

            DoXMLRequest(formData, Hotel.ToUrl(true) + "/profile/profileupdate");
        }
        public Task UpdateProfileAsync(string motto, bool homepageVisible, bool friendRequestAllowed, bool showOnlineStatus, bool offlineMessaging, bool friendsCanFollow)
        {
            return Task.Factory.StartNew(() => UpdateProfile(motto, homepageVisible, friendRequestAllowed, showOnlineStatus, offlineMessaging, friendsCanFollow));
        }

        public string RenewTicket()
        {
            LoadResource(HPages.Client);
            return _ssoTicket;
        }
        public Task<string> RenewTicketAsync()
        {
            return Task.Factory.StartNew(() => RenewTicket());
        }

        public void DownloadClient(string path)
        {
            using (var webClientEx = new WebClientEx(Cookies))
            {
                webClientEx.Headers["User-Agent"] = SKore.ChromeAgent;
                webClientEx.DownloadFile(FlashClientUrl, path);
            }
        }
        public Task DownloadClientAsync(string path)
        {
            return Task.Factory.StartNew(() => DownloadClient(path));
        }

        public void Connect()
        {
            _serverS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverS.BeginConnect(Addresses[0], Port, ConnectedToServer, null);
        }
        public void Disconnect(bool dispose = true)
        {
            lock (_disconnectLock)
            {
                if (_serverS == null) return;

                _serverS.Shutdown(SocketShutdown.Both);
                _serverS.Dispose();
                _serverS = null;

                _receiveData = false;
                _serverB = _serverC = null;
                _toClientS = _toServerS = 0;

                if (Disconnected != null && !dispose)
                {
                    var disconnectedEventArgs = new DisconnectedEventArgs();
                    Disconnected(this, disconnectedEventArgs);
                    dispose = disconnectedEventArgs.UnsubscribeFromEvents;
                }

                if (dispose)
                {
                    SKore.Unsubscribe(ref Connected);
                    SKore.Unsubscribe(ref DataToClient);
                    SKore.Unsubscribe(ref DataToServer);
                    SKore.Unsubscribe(ref Disconnected);
                }
            }
        }

        public int SendToServer(ushort header, params object[] chunks)
        {
            return SendToServer(HMessage.Construct(header, HDestinations.Server, HProtocols.Modern, chunks));
        }
        public Task<int> SendToServerAsync(ushort header, params object[] chunks)
        {
            return Task.Factory.StartNew(() => SendToServer(header, chunks));
        }

        public int SendToServer(byte[] data)
        {
            if (IsConnected)
            {
                if (DataToServer != null)
                {
                    try { DataToServer(this, new DataToEventArgs(data, HDestinations.Server, ++_toServerS)); }
                    catch (Exception ex) { SKore.Debugger(ex.ToString()); }
                }

                if (ClientEncrypt != null)
                    data = ClientEncrypt.SafeParse(data);

                try { _serverS.Send(data); }
                catch (Exception ex) { Disconnect(); SKore.Debugger(ex.ToString()); return 0; }

                return data.Length;
            }
            return 0;
        }
        public Task<int> SendToServerAsync(byte[] data)
        {
            return Task.Factory.StartNew(() => SendToServer(data));
        }

        int IHConnection.SendToClient(byte[] data)
        { return 0; }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Email != null ? Email.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Hotel.GetHashCode();
                return hashCode;
            }
        }
        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is HSession && Equals((HSession)obj);
        }
        public bool Equals(HSession other)
        {
            return string.Equals(Email, other.Email) && Enum.Equals(Hotel, other.Hotel);
        }

        public void Dispose()
        {
            var cookies = Cookies.GetCookies(new Uri(Hotel.ToUrl()));
            foreach (Cookie cookie in cookies) cookie.Expired = true;

            Flush();
            Disconnect();
        }
        public void Flush()
        {
            _firstTimeLoadProfile = true;
            _urlToken = string.Empty;
            _csrfToken = string.Empty;
            _playerName = string.Empty;
            _lastSignIn = string.Empty;
            _createdOn = string.Empty;
            _userHash = string.Empty;
            _motto = string.Empty;
            _homepageVisible = false;
            _friendRequestAllowed = false;
            _showOnlineStatus = false;
            _offlineMessaging = false;
            _friendsCanFollow = false;
            _gender = HGenders.Unknown;
            _age = 0;
            ClientStarting = string.Empty;
            _gameData = HGameData.Empty;
            _flashClientBuild = string.Empty;
            _flashClientUrl = string.Empty;
            _port = 0;
            _host = string.Empty;
            _addresses = null;
            _ssoTicket = string.Empty;
        }
        #endregion

        #region Private Methods
        private void LogStep(string step)
        {
            var formData = new NameValueCollection(1);
            formData.Add("step", step);

            DoXMLRequest(formData, Hotel.ToUrl(true) + "/new-user-reception/log-step");
        }
        private void ClientlogUpdate(string flashStep)
        {
            var formData = new NameValueCollection(1);
            formData.Add("flashStep", flashStep);

            DoXMLRequest(formData, Hotel.ToUrl(true) + "/clientlog/update");
        }
        private void DoXMLRequest(NameValueCollection formData, string address)
        {
            using (var webClientEx = new WebClientEx(Cookies))
            {
                webClientEx.Headers["X-App-Key"] = CsrfToken;
                webClientEx.Headers["User-Agent"] = SKore.ChromeAgent;
                webClientEx.Headers["Referer"] = Hotel.ToUrl(true) + "/client";

                if (formData != null) webClientEx.UploadValues(address, "POST", formData);
                else webClientEx.DownloadString(address);
            }
        }

        private string LoadResource(HPages page)
        {
            using (var webClientEx = new WebClientEx(Cookies))
            {
                string url = page.Juice(Hotel) + (page == HPages.Home ? PlayerName : string.Empty);
                webClientEx.Headers["User-Agent"] = SKore.ChromeAgent;
                string body = webClientEx.DownloadString(url);
                HandleResource(page, ref body);
                return body;
            }
        }
        private void HandleResource(HPages page, ref string body)
        {
            PlayerId = int.Parse(body.GetChild("var habboId = ", ';'));
            _playerName = body.GetChild("var habboName = \"", '\"');
            _age = int.Parse(body.GetChild("kvage=", ';'));
            _gender = (HGenders)Char.ToUpper(body.GetChild("kvgender=", ';')[0]);
            _csrfToken = body.GetChild("<meta name=\"csrf-token\" content=\"", '\"');

            switch (page)
            {
                case HPages.Me:
                {
                    string[] infoBoxes = body.GetChilds("<div class=\"content\">", '<');
                    _motto = infoBoxes[6].Split('>')[1];
                    _lastSignIn = infoBoxes[12].Split('>')[1];
                    break;
                }
                case HPages.Home:
                {
                    _createdOn = body.GetChild("<div class=\"birthday date\">", '<');
                    _motto = body.GetChild("<div class=\"profile-motto\">", '<');
                    break;
                }
                case HPages.Profile:
                {
                    _firstTimeLoadProfile = false;
                    _urlToken = body.GetChild("name=\"urlToken\" value=\"", '\"');

                    _homepageVisible = body.GetChild("name=\"visibility\" value=\"EVERYONE\"", '/').Contains("checked");
                    _friendRequestAllowed = body.GetChild("name=\"friendRequestsAllowed\"", '/').Contains("checked");
                    _showOnlineStatus = body.GetChild("name=\"showOnlineStatus\" value=\"true\"", '/').Contains("checked");
                    _offlineMessaging = body.GetChild("name=\"persistentMessagingAllowed\" checked=\"checked\"", '/').Contains("true");
                    _friendsCanFollow = body.GetChild("name=\"followFriendMode\" value=\"1\"").Contains("checked");
                    break;
                }
                case HPages.Client:
                {
                    _host = body.GetChild("\"connection.info.host\" : \"", '\"');
                    _port = int.Parse(body.GetChild("\"connection.info.port\" : \"", '\"').Split(',')[0]);
                    _addresses = Dns.GetHostAddresses(_host).Select(ip => ip.ToString()).ToArray();
                    _ssoTicket = body.GetChild("\"sso.ticket\" : \"", '\"');

                    if (string.IsNullOrEmpty(ClientStarting)) ClientStarting = body.GetChild("\"client.starting\" : \"", '\"');
                    else body = body.Replace(body.GetChild("\"client.starting\" : \"", '\"'), ClientStarting);

                    _userHash = body.GetChild("\"user.hash\" : \"", '\"');
                    _gameData = HGameData.Parse(body);

                    _flashClientUrl = "http://" + body.GetChild("\"flash.client.url\" : \"", '\"').Substring(3) + "Habbo.swf";
                    _flashClientBuild = _flashClientUrl.Split('/')[4];

                    body = body.Replace("\"\\//", "\"http://");
                    break;
                }
            }
        }

        private void ConnectedToServer(IAsyncResult iAr)
        {
            _serverS.EndConnect(iAr);

            _serverB = new byte[1024];

            _receiveData = true;
            ReadServerData();

            if (Connected != null)
                Connected(this, EventArgs.Empty);
        }
        private void ReadServerData()
        {
            if (IsConnected && ReceiveData)
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
                try
                {
                    #region Decrypt/Split
                    if (ServerDecrypt != null)
                        ServerDecrypt.Parse(data);

                    if (_toClientS == 2)
                    {
                        int dLength = Modern.DecypherInt(data);
                        ResponseEncrypted = (dLength > data.Length - 4 || dLength < 6);
                    }

                    byte[][] chunks = ResponseEncrypted ? new[] { data } : ByteUtils.Split(ref _serverC, data, HDestinations.Client, HProtocols.Modern);
                    #endregion
                    foreach (byte[] chunk in chunks)
                    {
                        if (Modern.DecypherShort(chunk, 4) == 4000) { Disconnect(); return; }

                        ++_toClientS;
                        if (DataToClient != null)
                        {
                            try { DataToClient(this, new DataToEventArgs(chunk, HDestinations.Client, _toClientS)); }
                            catch (Exception ex) { SKore.Debugger(ex.ToString()); }
                        }
                    }
                }
                catch (Exception ex) { SKore.Debugger(ex.ToString()); }
                ReadServerData();
            }
            catch (Exception ex) { Disconnect(); SKore.Debugger(ex.ToString()); }
        }
        #endregion

        #region Static Methods
        public static HSession[] Extract(string path, char delimiter = ':')
        {
            var accounts = new List<HSession>();
            using (var streamReader = new StreamReader(path))
            {
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    if (line.Contains(delimiter))
                    {
                        string[] credentials = line.Split(delimiter);
                        if (credentials.Count(x => !string.IsNullOrEmpty(x)) != 3) break;
                        accounts.Add(new HSession(credentials[0], credentials[1], SKore.ConvertToHHotel(credentials[2])));
                        continue;
                    }
                    if (line.Contains('@') && !streamReader.EndOfStream)
                    {
                        string email = line;
                        string password = streamReader.ReadLine();
                        if (!streamReader.EndOfStream)
                        {
                            HHotels hotel = SKore.ConvertToHHotel((streamReader.ReadLine()).GetChild(" / "));
                            accounts.Add(new HSession(email, password, hotel));
                        }
                        else return accounts.ToArray();
                    }
                }
            }
            return accounts.ToArray();
        }
        public static Task<HSession[]> ExtractAsync(string path, char delimiter = ':')
        {
            return Task.Factory.StartNew(() => Extract(path, delimiter));
        }

        private static bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        #endregion

        #region Instance Formatters
        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}", Email, Password, Hotel.ToDomain());
        }
        #endregion
    }
}