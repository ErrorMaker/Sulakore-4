using System;
using System.IO;
using System.Net;
using Sulakore.Protocol;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Sulakore.Communication.Proxy
{
    public static class Eavesdropper
    {
        private static readonly HttpListener _httpListener;
        private static readonly List<IAsyncResult> _pendingCallbacks;

        public static Action<string, bool> DebugCallback { get; set; }
        public static event EventHandler<EavesResponseEventArgs> OnEavesResponse;

        public static bool DisableCache { get; set; }
        public static bool IsRunning { get; private set; }

        static Eavesdropper()
        {
            _httpListener = new HttpListener();
            _pendingCallbacks = new List<IAsyncResult>();
        }

        public static void Initiate(int port)
        {
            if (IsRunning)
                throw new Exception("Eavesdropper is already running, you must call the Terminate method first before you can call the Initiate method.");

            IsRunning = true;

            _httpListener.Prefixes.Clear();
            _httpListener.Prefixes.Add("http://*:" + port + "/");
            _httpListener.Start();

            NativeMethods.EnableProxy(port);
            Task.Factory.StartNew(CaptureClients, TaskCreationOptions.LongRunning);
        }
        public static void Terminate()
        {
            if (!IsRunning)
                throw new Exception("Eavesdropper has not yet been started, you must first call the Initate method before you can call Terminate.");

            IsRunning = false;

            if (_pendingCallbacks.Count == 0)
            {
                NativeMethods.DisableProxy();
                _httpListener.Stop();
            }
        }

        private static void CaptureClients()
        {
            if (IsRunning)
                _pendingCallbacks.Add(_httpListener.BeginGetContext(RequestReceived, null));
        }
        private static void RequestReceived(IAsyncResult ar)
        {
            try
            {
                CaptureClients();
                if (!IsRunning) return;

                HttpListenerContext context = _httpListener.EndGetContext(ar);
                HttpListenerRequest clientRequest = context.Request;
                using (HttpListenerResponse clientResponse = context.Response)
                {
                    string contextLog = string.Format("Request -> ( {0} ): {1}\n", clientRequest.HttpMethod, clientRequest.RawUrl);

                    #region HttpWebRequest Constructer
                    var request = (HttpWebRequest)WebRequest.Create(clientRequest.RawUrl);
                    request.ProtocolVersion = clientRequest.ProtocolVersion;
                    request.Method = clientRequest.HttpMethod;
                    request.AllowAutoRedirect = false;
                    request.KeepAlive = false;
                    request.Proxy = null;

                    request.CookieContainer = new CookieContainer();
                    request.CookieContainer.Add(clientRequest.Url, clientRequest.Cookies);

                    var requestHeaders = (WebHeaderCollection)clientRequest.Headers;
                    foreach (string header in requestHeaders.Keys)
                    {
                        string value = requestHeaders[header];
                        switch (header)
                        {
                            case "Connection":
                            case "Keep-Alive":
                            case "Proxy-Connection": continue;

                            case "Range":
                            {
                                string[] ranges = value.GetChilds("bytes=", '-');
                                if (ranges.Length > 1) request.AddRange(long.Parse(ranges[0]), long.Parse(ranges[1]));
                                else request.AddRange(long.Parse(ranges[0]));
                                break;
                            }
                            case "Host": request.Host = value; break;
                            case "Accept": request.Accept = value; break;
                            case "Referer": request.Referer = value; break;
                            case "User-Agent": request.UserAgent = value; break;
                            case "Content-Type": request.ContentType = value; break;
                            case "Content-Length": request.ContentLength = long.Parse(value); break;
                            case "If-Modified-Since": request.IfModifiedSince = DateTime.Parse(value); break;


                            default: request.Headers[header] = value; break;
                        }
                        contextLog += header + ": " + value + "\n";
                    }

                    if (DisableCache)
                    {
                        request.Headers["Cache-Control"] = "no-cache, no store";
                        request.Headers["Pragma"] = "no-cache, no store";
                    }

                    if ((request.ContentLength != 0 && request.SendChunked) || request.ContentLength > 0)
                    {
                        request.SendChunked = false;
                        var requestData = new byte[0];
                        using (Stream requestStream = request.GetRequestStream())
                        {
                            int length;
                            var chunk = new byte[request.ContentLength > 0 ? request.ContentLength : 8192];

                            while ((length = clientRequest.InputStream.Read(chunk, 0, chunk.Length)) > 0)
                                requestData = ByteUtils.Merge(requestData, ByteUtils.CopyBlock(chunk, 0, length));

                            requestStream.Write(requestData, 0, requestData.Length);
                        }
                    }
                    #endregion

                    #region HttpWebResponse Constructer
                    try
                    {
                        using (var response = (HttpWebResponse)request.GetResponse())
                        {
                            contextLog += string.Format("\nResponse <- ( Size: {0} ): {1}\n", response.ContentLength, response.ResponseUri.AbsoluteUri);

                            var responseHeaders = response.Headers;
                            foreach (string header in responseHeaders.Keys)
                            {
                                string value = responseHeaders[header];
                                switch (header)
                                {
                                    case "Keep-Alive": continue;
                                    case "Content-Length": clientResponse.ContentLength64 = long.Parse(value); break;
                                    case "Transfer-Encoding": clientResponse.SendChunked = false; break;
                                    case "Connection": clientResponse.KeepAlive = (value == "keep-alive"); break;
                                    default: clientResponse.Headers[header] = value; break;
                                }
                                contextLog += header + ": " + value + "\n";
                            }

                            if (DisableCache)
                            {
                                clientResponse.Headers["Cache-Control"] = "no-cache, no store";
                                clientResponse.Headers["Pragma"] = "no-cache, no store";
                            }

                            if (DebugCallback != null)
                                DebugCallback(Uri.UnescapeDataString(contextLog.Trim()), false);

                            clientResponse.Cookies = response.Cookies;
                            clientResponse.ContentType = response.ContentType;
                            clientResponse.StatusCode = (int)response.StatusCode;
                            clientResponse.ProtocolVersion = response.ProtocolVersion;
                            clientResponse.StatusDescription = response.StatusDescription;
                            clientResponse.RedirectLocation = response.Headers[HttpResponseHeader.Location];

                            var responseData = new byte[0];
                            using (Stream responseStream = response.GetResponseStream())
                            {
                                int length;
                                var chunk = new byte[response.ContentLength > 0 ? response.ContentLength : 8192];

                                while ((length = responseStream.Read(chunk, 0, chunk.Length)) > 0)
                                    responseData = ByteUtils.Merge(responseData, ByteUtils.CopyBlock(chunk, 0, length));
                            }
                            if (OnEavesResponse != null)
                            {
                                var arguments = new EavesResponseEventArgs(responseData, response.ResponseUri.AbsoluteUri, clientResponse.ContentType == "application/x-shockwave-flash");
                                OnEavesResponse(context, arguments);
                                responseData = arguments.ResponeData;
                            }

                            clientResponse.ContentLength64 = responseData.Length;
                            clientResponse.OutputStream.Write(responseData, 0, responseData.Length);
                        }
                    }
                    catch (Exception ex) { SKore.Debugger(ex.ToString()); }
                    #endregion
                }
            }
            catch (Exception ex) { SKore.Debugger(ex.ToString()); }
            finally
            {
                _pendingCallbacks.Remove(ar);
                if (!IsRunning && _pendingCallbacks.Count == 0)
                {
                    NativeMethods.DisableProxy();
                    _httpListener.Stop();
                }
            }
        }
    }
}