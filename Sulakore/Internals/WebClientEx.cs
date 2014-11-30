using System;
using System.Net;

namespace Sulakore
{
    [System.ComponentModel.DesignerCategory("Code")]
    internal sealed class WebClientEx : WebClient
    {
        private readonly CookieContainer _cookies;

        public WebClientEx(CookieContainer cookies = null)
        {
            _cookies = cookies ?? new CookieContainer();
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (base.GetWebRequest(address) as HttpWebRequest);
            if (request == null) return base.GetWebRequest(address);
            request.CookieContainer = _cookies;
            return request;
        }
        protected override WebResponse GetWebResponse(WebRequest request)
        {
            var response = (base.GetWebResponse(request) as HttpWebResponse);
            if (response == null) return base.GetWebResponse(request);
            _cookies.Add(response.Cookies);
            return response;
        }
    }
}