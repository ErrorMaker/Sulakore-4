using System;
using System.ComponentModel;

namespace Sulakore.Communication.Proxy
{
    public class EavesRequestEventArgs : CancelEventArgs
    {
        public string Url { get; set; }

        public EavesRequestEventArgs(string url)
        {
            Url = url;
        }
    }
}