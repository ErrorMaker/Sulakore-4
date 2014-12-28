﻿using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Generic;

using Sulakore.Habbo;

namespace Sulakore
{
    /// <summary>
    /// Provides static methods for extracting public information from a specific hotel, and method extensions.
    /// </summary>
    public static class SKore
    {
        private static string _ipCookie;

        private static readonly DirectoryInfo CacheDirectory;
        private static readonly string FlashSharedObjectsPath;
        private static readonly object RandomSignLock, RandomThemeLock;
        private static readonly Random RandomSignGenerator, RandomThemeGenerator;
        private static readonly IDictionary<string, IDictionary<HHotel, int>> _playerIds;
        private static readonly IDictionary<int, IDictionary<HHotel, string>> _playerNames;

        public const string ChromeAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36";

        static SKore()
        {
            RandomSignLock = new object();
            RandomThemeLock = new object();
            RandomSignGenerator = new Random();
            RandomThemeGenerator = new Random();
            _playerIds = new Dictionary<string, IDictionary<HHotel, int>>();
            _playerNames = new Dictionary<int, IDictionary<HHotel, string>>();
            CacheDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache));
            FlashSharedObjectsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Macromedia\\Flash Player\\#SharedObjects";
        }

        /// <summary>
        /// Returns a cookie containing your external IP address that is required by most hotels to retrieve a remote resource from their host.
        /// </summary>
        /// <returns></returns>
        public static string GetIpCookie()
        {
            if (!string.IsNullOrEmpty(_ipCookie)) return _ipCookie;
            using (var webClientEx = new WebClientEx())
            {
                webClientEx.Proxy = null;
                webClientEx.Headers["User-Agent"] = ChromeAgent;
                string body = webClientEx.DownloadString("http://www.Habbo.com");
                return _ipCookie = (body.Contains(("setCookie")) ? "YPF8827340282Jdskjhfiw_928937459182JAX666=" + body.GetChilds("setCookie", '\'')[3] : string.Empty);
            }
        }
        public static Task<string> GetIpCookieAsync()
        {
            return Task.Factory.StartNew(() => GetIpCookie());
        }

        /// <summary>
        /// Returns the current online player count for the specified hotel.
        /// </summary>
        /// <param name="hotel">The hotel to retrieve the online player count from.</param>
        /// <returns></returns>
        public static int GetPlayersOnline(HHotel hotel)
        {
            using (var webClientEx = new WebClientEx())
            {
                webClientEx.Proxy = null;
                webClientEx.Headers["Cookie"] = GetIpCookie();
                webClientEx.Headers["User-Agent"] = ChromeAgent;
                string body = webClientEx.DownloadString(hotel.ToUrl() + "/login_popup");
                return body.Contains("stats-fig") ? int.Parse(body.GetChild("<span class=\"stats-fig\">", '<')) : -1;
            }
        }
        public static Task<int> GetPlayersOnlineAsync(HHotel hotel)
        {
            return Task.Factory.StartNew(() => GetPlayersOnline(hotel));
        }

        /// <summary>
        /// Returns the player id relative to the specified player name, and hotel.
        /// </summary>
        /// <param name="playerName">The name of the player of whom to grab the id from.</param>
        /// <param name="hotel">The hotel where the specified player name exists on.</param>
        /// <returns></returns>
        public static int GetPlayerId(string playerName, HHotel hotel)
        {
            if (_playerIds.ContainsKey(playerName))
                return _playerIds[playerName][hotel];

            using (var webClientEx = new WebClientEx())
            {
                webClientEx.Proxy = null;
                webClientEx.Headers["Cookie"] = GetIpCookie();
                webClientEx.Headers["User-Agent"] = ChromeAgent;
                string body = webClientEx.DownloadString(hotel.ToUrl() + "/habblet/ajax/new_habboid?habboIdName=" + playerName);
                int value = !body.Contains("rounded rounded-red") ? int.Parse(body.GetChild("<em>", '<').Replace(" ", string.Empty)) : -1;

                _playerIds[playerName] = new Dictionary<HHotel, int>()
                {
                    { hotel, value }
                };
                return value;
            }
        }
        public static Task<int> GetPlayerIdAsync(string playerName, HHotel hotel)
        {
            return Task.Factory.StartNew(() => GetPlayerId(playerName, hotel));
        }

        /// <summary>
        /// Returns the player name relative to the specified player id, and hotel.
        /// </summary>
        /// <param name="playerId">The id of the player of whom to grab the name from.</param>
        /// <param name="hotel">The hotel where the specified player id exists on.</param>
        /// <returns></returns>
        public static string GetPlayerName(int playerId, HHotel hotel)
        {
            if (_playerNames.ContainsKey(playerId))
                return _playerNames[playerId][hotel];

            using (var webClientEx = new WebClientEx())
            {
                webClientEx.Proxy = null;
                webClientEx.Headers["Cookie"] = GetIpCookie();
                webClientEx.Headers["User-Agent"] = ChromeAgent;
                string body = webClientEx.DownloadString(string.Format("{0}/rd/{1}", hotel.ToUrl(), playerId));
                string value = body.Contains("/home/") ? body.GetChild("<input type=\"hidden\" name=\"page\" value=\"/home/", '?') : string.Empty;

                _playerNames[playerId] = new Dictionary<HHotel, string>()
                {
                    { hotel, value }
                };
                return value;
            }
        }
        public static Task<string> GetPlayerNameAsync(int playerId, HHotel hotel)
        {
            return Task.Factory.StartNew(() => GetPlayerName(playerId, hotel));
        }

        /// <summary>
        /// Determines whether the specified player name is taken relative to the hotel.
        /// </summary>
        /// <param name="playerName">The name of the player to check for availability.</param>
        /// <param name="hotel">The hotel in which to check the availability of the player name.</param>
        /// <returns></returns>
        public static bool CheckPlayerName(string playerName, HHotel hotel)
        {
            return GetPlayerId(playerName, hotel) == -1;
        }
        public static Task<bool> CheckPlayerNameAsync(string playerName, HHotel hotel)
        {
            return Task.Factory.StartNew(() => CheckPlayerName(playerName, hotel));
        }

        /// <summary>
        /// Returns the player motto relative to the specified player name, and hotel.
        /// </summary>
        /// <param name="playerName">The name of the player of whom to grab the motto from.</param>
        /// <param name="hotel">The hotel where the specified player name exists on.</param>
        /// <returns></returns>
        public static string GetPlayerMotto(string playerName, HHotel hotel)
        {
            using (var webClientEx = new WebClientEx())
            {
                webClientEx.Proxy = null;
                webClientEx.Headers["Cookie"] = GetIpCookie();
                webClientEx.Headers["User-Agent"] = ChromeAgent;
                string body = webClientEx.DownloadString(hotel.ToUrl() + "/habblet/habbosearchcontent?searchString=" + playerName);
                return body.IndexOf(playerName, StringComparison.OrdinalIgnoreCase) != -1 ? body.GetChild("<b>" + playerName + "</b><br />", '<') : string.Empty;
            }
        }
        public static Task<string> GetPlayerMottoAsync(string playerName, HHotel hotel)
        {
            return Task.Factory.StartNew(() => GetPlayerMotto(playerName, hotel));
        }

        /// <summary>
        /// Returns the player avatar relative to the specified player name, and hotel.
        /// </summary>
        /// <param name="playerName">The name of the player of whom to grab the avatar from.</param>
        /// <param name="hotel">The hotel where the specified player name exists on.</param>
        /// <returns></returns>
        public static Bitmap GetPlayerAvatar(string playerName, HHotel hotel)
        {
            using (var webClientEx = new WebClientEx())
            {
                webClientEx.Proxy = null;
                webClientEx.Headers["Cookie"] = GetIpCookie();
                webClientEx.Headers["User-Agent"] = ChromeAgent;
                byte[] avatarData = webClientEx.DownloadData(hotel.ToUrl() + "/habbo-imaging/avatarimage?user=" + playerName + "&action=&direction=&head_direction=&gesture=&size=");
                using (var memoryStream = new MemoryStream(avatarData))
                    return new Bitmap(memoryStream);
            }
        }
        public static Task<Bitmap> GetPlayerAvatarAsync(string playerName, HHotel hotel)
        {
            return Task.Factory.StartNew(() => GetPlayerAvatar(playerName, hotel));
        }

        /// <summary>
        /// Returns the player figure id relative to the specified player name, and hotel.
        /// </summary>
        /// <param name="playerName">The name of the player of whom to grab the player figure id from.</param>
        /// <param name="hotel">The hotel where the specified player name exists on.</param>
        /// <returns></returns>
        public static string GetPlayerFigureId(string playerName, HHotel hotel)
        {
            using (var webClientEx = new WebClientEx())
            {
                webClientEx.Proxy = null;
                webClientEx.Headers["Cookie"] = GetIpCookie();
                webClientEx.Headers["User-Agent"] = ChromeAgent;
                string body = webClientEx.DownloadString(hotel.ToUrl() + "/habblet/habbosearchcontent?searchString=" + playerName);
                return body.Contains("habbo-imaging/avatar/") ? body.GetChild("habbo-imaging/avatar/", ',') : string.Empty;
            }
        }
        public static Task<string> GetPlayerFigureIdAsync(string playerName, HHotel hotel)
        {
            return Task.Factory.StartNew(() => GetPlayerFigureId(playerName, hotel));
        }

        /// <summary>
        /// Returns the player's last online date relative to the specified player name, and hotel.
        /// </summary>
        /// <param name="playerName">The name of the player of whom to grab the player's last online date from.</param>
        /// <param name="hotel">The hotel where the specified player name exists on.</param>
        /// <param name="exact">true to return the time span; otherwise, false for the full date of when the player was last online.</param>
        /// <returns></returns>
        public static DateTime GetPlayerLastOnline(string playerName, HHotel hotel)
        {
            using (var webClientEx = new WebClientEx())
            {
                webClientEx.Proxy = null;
                webClientEx.Headers["Cookie"] = GetIpCookie();
                webClientEx.Headers["User-Agent"] = ChromeAgent;
                string body = webClientEx.DownloadString(hotel.ToUrl() + "/habblet/habbosearchcontent?searchString=" + playerName);

                if (!body.Contains("lastlogin")) return DateTime.MinValue;

                body = body.GetChild("<div class=\"lastlogin\">").GetChild("span title=");
                return DateTime.Parse(body.Split('"')[1]);
            }
        }
        public static Task<DateTime> GetPlayerLastOnlineAsync(string playerName, HHotel hotel)
        {
            return Task.Factory.StartNew(() => GetPlayerLastOnline(playerName, hotel));
        }

        public static void ClearCache()
        {
            FileInfo[] cacheFiles = CacheDirectory.GetFiles();
            foreach (FileInfo cacheFile in cacheFiles)
                try { cacheFile.Delete(); }
                catch { }

            DirectoryInfo[] cacheDirectories = CacheDirectory.GetDirectories();
            foreach (DirectoryInfo cacheFolder in cacheDirectories)
                try { cacheFolder.Delete(true); }
                catch { }

            if (Directory.Exists(FlashSharedObjectsPath))
                Directory.Delete(FlashSharedObjectsPath, true);
        }

        public static void Unsubscribe(ref EventHandler Event)
        {
            if (Event == null) return;
            Delegate[] subscriptions = Event.GetInvocationList();
            Event = subscriptions.Aggregate(Event, (current, subscription) => current - (EventHandler)subscription);
        }
        public static void Unsubscribe<T>(ref EventHandler<T> Event) where T : EventArgs
        {
            if (Event == null) return;
            Delegate[] subscriptions = Event.GetInvocationList();
            Event = subscriptions.Aggregate(Event, (current, subscription) => current - (EventHandler<T>)subscription);
        }

        public static int GetGamePort(this HHotel hotel)
        {
            return hotel == HHotel.Com ? 38101 : 30000;
        }
        public static string ToDomain(this HHotel hotel)
        {
            string outCome = hotel.ToString().ToLower();
            if (hotel == HHotel.ComBr || hotel == HHotel.ComTr)
                outCome = "com." + outCome.Substring(3, 2);
            return outCome;
        }
        public static string GetGameHost(this HHotel hotel)
        {
            return string.Format("game-{0}.habbo.com", hotel == HHotel.Com ? "us" : hotel.ToDomain().Replace("com.", string.Empty));
        }
        public static string[] GetAddresses(this HHotel hotel)
        {
            return Dns.GetHostAddresses(GetGameHost(hotel)).Select(ip => ip.ToString()).ToArray();
        }
        public static string ToUrl(this HHotel hotel, bool https = false)
        {
            return (https ? "https://www.Habbo." : "http://www.Habbo.") + hotel.ToDomain();
        }

        public static int Juice(this HSign sign)
        {
            if (sign != HSign.Random) return (int)sign;

            lock (RandomSignLock)
                return RandomSignGenerator.Next(0, 19);
        }
        public static int Juice(this HTheme theme)
        {
            if (theme != HTheme.Random) return (int)theme;

            lock (RandomThemeLock)
                return RandomThemeGenerator.Next(0, 30);
        }
        public static string Juice(this HPage page, HHotel hotel)
        {
            switch (page)
            {
                case HPage.Client: return hotel.ToUrl() + "/client";
                case HPage.Home: return hotel.ToUrl() + "/home/";
                case HPage.IdAvatars: return hotel.ToUrl() + "/identity/avatars";
                case HPage.IdSettings: return hotel.ToUrl() + "/identity/settings";
                case HPage.Me: return hotel.ToUrl() + "/me";
                case HPage.Profile: return hotel.ToUrl() + "/profile";
                default: return string.Empty;
            }
        }

        public static HGender ToGender(string gender)
        {
            return (HGender)gender.ToUpper()[0];
        }

        public static string Juice(this HBan ban)
        {
            switch (ban)
            {
                default:
                case HBan.Day: return "RWUAM_BAN_USER_DAY";

                case HBan.Hour: return "RWUAM_BAN_USER_HOUR";
                case HBan.Permanent: return "RWUAM_BAN_USER_PERM";
            }
        }
        public static HBan ToBan(string ban)
        {
            switch (ban)
            {
                default:
                case "RWUAM_BAN_USER_DAY": return HBan.Day;

                case "RWUAM_BAN_USER_HOUR": return HBan.Hour;
                case "RWUAM_BAN_USER_PERM": return HBan.Permanent;
            }
        }

        public static HHotel ToHotel(string value)
        {
            if (value.Contains("game-")) value = value.GetChild("game-", '.');
            else if (value.Contains("habbo")) value = value.GetChild("habbo.");
            value = value.Replace(".", string.Empty);

            if (value == "us") value = "com";
            if (value == "br" || value == "tr") value = "com" + value;

            HHotel hotel;
            return Enum.TryParse(value, true, out hotel) ? hotel : (HHotel)(-1);
        }
        public static bool IsOriginal(string gameHost, int gamePort = 0)
        {
            if (gamePort != 0 && gamePort != 30000 && gamePort != 38101) return false;
            return gameHost == "habboo-a.akamaihd.net" || Enum.IsDefined(typeof(HHotel), ToHotel(gameHost));
        }

        public static string GetChild(this string body, string parent)
        {
            return body.Substring(body.IndexOf(parent, StringComparison.OrdinalIgnoreCase) + parent.Length).Trim();
        }
        public static string GetChild(this string body, string parent, char delimiter)
        {
            return GetChilds(body, parent, delimiter)[0].Trim();
        }
        public static string[] GetChilds(this string body, string parent, char delimiter)
        {
            return GetChild(body, parent).Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}