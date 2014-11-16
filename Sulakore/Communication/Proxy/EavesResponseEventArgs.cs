using System;

namespace Sulakore.Communication.Proxy
{
    public class EavesResponseEventArgs : EventArgs
    {
        public bool IsSwf { get; private set; }
        public string Url { get; private set; }
        public byte[] ResponeData { get; set; }

        public EavesResponseEventArgs(byte[] responseData, string url, bool isSwf)
        {
            ResponeData = responseData;
            Url = url;
            IsSwf = isSwf;
        }
    }
}