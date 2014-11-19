using System;
using System.IO;
using System.Net;
using System.Linq;
using Sulakore.Habbo;
using System.Drawing;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Sulakore
{
    public static class SKore
    {
        #region Private Fields
        private static string _ipCookie;
        private static readonly DirectoryInfo CacheDirectory;
        private static readonly string FlashSharedObjectsPath;
        private static readonly object RandomSignLock, RandomThemeLock;
        private static readonly Random RandomSignGenerator, RandomThemeGenerator;
        #endregion

        #region Public Fields
        public const string ChromeAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/38.0.2125.104 Safari/537.36";
        #endregion

        static SKore()
        {
            RandomSignLock = new object();
            RandomThemeLock = new object();
            RandomSignGenerator = new Random();
            RandomThemeGenerator = new Random();
            CacheDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache));

            FlashSharedObjectsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Macromedia\\Flash Player\\#SharedObjects";
        }

        public static string GetIpCookie()
        {
            if (!string.IsNullOrEmpty(_ipCookie)) return _ipCookie;
            using (var webClient = new WebClient())
            {
                webClient.Headers["User-Agent"] = ChromeAgent;
                string body = webClient.DownloadString("http://www.Habbo.com");
                return _ipCookie = (body.Contains(("setCookie")) ? "YPF8827340282Jdskjhfiw_928937459182JAX666=" + body.GetChilds("setCookie", '\'')[3] : string.Empty);
            }
        }
        public static Task<string> GetIpCookieAsync()
        {
            return Task.Factory.StartNew(() => GetIpCookie());
        }

        public static int GetPlayersOnline(HHotels hotel)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers["Cookie"] = GetIpCookie();
                webClient.Headers["User-Agent"] = ChromeAgent;
                string body = webClient.DownloadString(hotel.ToUrl() + "/login_popup");
                return body.Contains("stats-fig") ? int.Parse(body.GetChild("<span class=\"stats-fig\">", '<')) : -1;
            }
        }
        public static Task<int> GetPlayersOnlineAsync(HHotels hotel)
        {
            return Task.Factory.StartNew(() => GetPlayersOnline(hotel));
        }

        public static int GetPlayerId(string playerName, HHotels hotel)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers["Cookie"] = GetIpCookie();
                webClient.Headers["User-Agent"] = ChromeAgent;
                string body = webClient.DownloadString(hotel.ToUrl() + "/habblet/ajax/new_habboid?habboIdName=" + playerName);
                return !body.Contains("rounded rounded-red") ? int.Parse(body.GetChild("<em>", '<').Replace(" ", string.Empty)) : -1;
            }
        }
        public static Task<int> GetPlayerIdAsync(string playerName, HHotels hotel)
        {
            return Task.Factory.StartNew(() => GetPlayerId(playerName, hotel));
        }

        public static string GetPlayerName(int playerId, HHotels hotel)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers["Cookie"] = GetIpCookie();
                webClient.Headers["User-Agent"] = ChromeAgent;
                string body = webClient.DownloadString(string.Format("{0}/rd/{1}", hotel.ToUrl(), playerId));
                return body.Contains("/home/") ? body.GetChild("<input type=\"hidden\" name=\"page\" value=\"/home/", '?') : string.Empty;
            }
        }
        public static Task<string> GetPlayerNameAsync(int playerId, HHotels hotel)
        {
            return Task.Factory.StartNew(() => GetPlayerName(playerId, hotel));
        }

        public static bool CheckPlayerName(string playerName, HHotels hotel)
        {
            return GetPlayerId(playerName, hotel) == -1;
        }
        public static Task<bool> CheckPlayerNameAsync(string playerName, HHotels hotel)
        {
            return Task.Factory.StartNew(() => CheckPlayerName(playerName, hotel));
        }

        public static string GetPlayerMotto(string playerName, HHotels hotel)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers["Cookie"] = GetIpCookie();
                webClient.Headers["User-Agent"] = ChromeAgent;
                string body = webClient.DownloadString(hotel.ToUrl() + "/habblet/habbosearchcontent?searchString=" + playerName);
                return body.IndexOf(playerName, StringComparison.OrdinalIgnoreCase) != -1 ? body.GetChild("<b>" + playerName + "</b><br />", '<') : string.Empty;
            }
        }
        public static Task<string> GetPlayerMottoAsync(string playerName, HHotels hotel)
        {
            return Task.Factory.StartNew(() => GetPlayerMotto(playerName, hotel));
        }

        public static Bitmap GetPlayerAvatar(string playerName, HHotels hotel)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers["Cookie"] = GetIpCookie();
                webClient.Headers["User-Agent"] = ChromeAgent;
                byte[] avatarData = webClient.DownloadData(hotel.ToUrl() + "/habbo-imaging/avatarimage?user=" + playerName + "&action=&direction=&head_direction=&gesture=&size=");
                using (var memoryStream = new MemoryStream(avatarData))
                    return new Bitmap(memoryStream);
            }
        }
        public static Task<Bitmap> GetPlayerAvatarAsync(string playerName, HHotels hotel)
        {
            return Task.Factory.StartNew(() => GetPlayerAvatar(playerName, hotel));
        }

        public static string GetPlayerFigureId(string playerName, HHotels hotel)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers["Cookie"] = GetIpCookie();
                webClient.Headers["User-Agent"] = ChromeAgent;
                string body = webClient.DownloadString(hotel.ToUrl() + "/habblet/habbosearchcontent?searchString=" + playerName);
                return body.Contains("habbo-imaging/avatar/") ? body.GetChild("habbo-imaging/avatar/", ',') : string.Empty;
            }
        }
        public static Task<string> GetPlayerFigureIdAsync(string playerName, HHotels hotel)
        {
            return Task.Factory.StartNew(() => GetPlayerFigureId(playerName, hotel));
        }

        public static string GetPlayerLastOnline(string playerName, HHotels hotel, bool exact = true)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers["Cookie"] = GetIpCookie();
                webClient.Headers["User-Agent"] = ChromeAgent;
                string body = webClient.DownloadString(hotel.ToUrl() + "/habblet/habbosearchcontent?searchString=" + playerName);

                if (!body.Contains("lastlogin")) return string.Empty;

                body = body.GetChild("<div class=\"lastlogin\">").GetChild("span title=");
                return exact ? body.Split('"')[1] : body.Split('>')[1].Split('<')[0];
            }
        }
        public static Task<string> GetPlayerLastOnlineAsync(string playerName, HHotels hotel, bool exact = true)
        {
            return Task.Factory.StartNew(() => GetPlayerLastOnline(playerName, hotel, exact));
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
        public static void Unsubscribe<T>(ref EventHandler<T> Event) where T : EventArgs
        {
            if (Event == null) return;
            Delegate[] subscriptions = Event.GetInvocationList();
            Event = subscriptions.Aggregate(Event, (current, subscription) => current - (EventHandler<T>)subscription);
        }

        public static int GetGamePort(this HHotels hotel)
        {
            return hotel == HHotels.Com ? 38101 : 30000;
        }
        public static string ToDomain(this HHotels hotel)
        {
            string outCome = hotel.ToString().ToLower();
            if (hotel == HHotels.ComBr || hotel == HHotels.ComTr)
                outCome = "com." + outCome.Substring(3, 2);
            return outCome;
        }
        public static string GetGameHost(this HHotels hotel)
        {
            return string.Format("game-{0}.habbo.com", hotel == HHotels.Com ? "us" : hotel.ToDomain().Replace("com.", string.Empty));
        }
        public static string[] GetAddresses(this HHotels hotel)
        {
            return Dns.GetHostAddresses(GetGameHost(hotel)).Select(ip => ip.ToString()).ToArray();
        }
        public static string ToUrl(this HHotels hotel, bool https = false)
        {
            return (https ? "https://www.Habbo." : "http://www.Habbo.") + hotel.ToDomain();
        }

        public static int Juice(this HSigns sign)
        {
            if (sign != HSigns.Random) return (int)sign;

            lock (RandomSignLock)
                return RandomSignGenerator.Next(0, 19);
        }
        public static int Juice(this HThemes theme)
        {
            if (theme != HThemes.Random) return (int)theme;

            lock (RandomThemeLock)
                return RandomThemeGenerator.Next(0, 30);
        }
        public static string Juice(this HPages page, HHotels hotel)
        {
            switch (page)
            {
                case HPages.Client: return hotel.ToUrl() + "/client";
                case HPages.Home: return hotel.ToUrl() + "/home/";
                case HPages.IdAvatars: return hotel.ToUrl() + "/identity/avatars";
                case HPages.IdSettings: return hotel.ToUrl() + "/identity/settings";
                case HPages.Me: return hotel.ToUrl() + "/me";
                case HPages.Profile: return hotel.ToUrl() + "/profile";
                default: return string.Empty;
            }
        }

        public static HGenders ConvertToHGender(string gender)
        {
            return (HGenders)gender.ToUpper()[0];
        }

        public static string Juice(this HBans ban)
        {
            switch (ban)
            {
                default:
                case HBans.Day: return "RWUAM_BAN_USER_DAY";

                case HBans.Hour: return "RWUAM_BAN_USER_HOUR";
                case HBans.Permanent: return "RWUAM_BAN_USER_PERM";
            }
        }
        public static HBans ConvertToHBan(string ban)
        {
            switch (ban)
            {
                default:
                case "RWUAM_BAN_USER_DAY": return HBans.Day;

                case "RWUAM_BAN_USER_HOUR": return HBans.Hour;
                case "RWUAM_BAN_USER_PERM": return HBans.Permanent;
            }
        }

        public static HHotels ConvertToHHotel(string value)
        {
            if (value.Contains("game-")) value = value.GetChild("game-", '.');
            else if (value.Contains("habbo")) value = value.GetChild("habbo.");
            value = value.Replace(".", string.Empty);

            if (value == "us") value = "com";
            if (value == "br" || value == "tr") value = "com" + value;

            HHotels hotel;
            return Enum.TryParse(value, true, out hotel) ? hotel : (HHotels)(-1);
        }
        public static bool IsOriginal(string gameHost, int gamePort = 0)
        {
            if (gamePort != 0 && gamePort != 30000 && gamePort != 38101) return false;
            return gameHost == "habboo-a.akamaihd.net" || Enum.IsDefined(typeof(HHotels), ConvertToHHotel(gameHost));
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

        internal static void Debugger(string message)
        {
            Debug.WriteLine(message);
        }
    }
}