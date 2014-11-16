using System.Net;

namespace Sulakore.Habbo
{
    public interface IPlayerSession
    {
        CookieContainer Cookies { get; }

        string SsoTicket { get; }
        string PlayerName { get; }
        int PlayerId { get; }

        bool Login();
    }
}