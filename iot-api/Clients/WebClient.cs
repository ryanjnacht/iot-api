using System;
using System.IO;
using System.Net;
using System.Net.Cache;

namespace iot_api.Clients
{
    public static class WebClient
    {
        private static string Request(string uri, string method = "GET")
        {
            Console.WriteLine($"[WebClient ({method})] uri: {uri}");

            var retries = 0;
            while (true)
            {
                var webReq = (HttpWebRequest) WebRequest.Create(uri);

                webReq.ContentType = "application/json";
                webReq.Accept = "application/json";
                webReq.Headers.Add("Content-Encoding", "utf-8");
                webReq.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                webReq.KeepAlive = false;
                webReq.Timeout = Configuration.Configuration.WebClientTimeout;
                webReq.ServicePoint.ConnectionLeaseTimeout = 5000;
                webReq.ServicePoint.MaxIdleTime = 5000;
                webReq.ServicePoint.Expect100Continue = false;
                webReq.ProtocolVersion = HttpVersion.Version11;

                if (method == "GET") webReq.Method = WebRequestMethods.Http.Get;
                if (method == "PUT") webReq.Method = WebRequestMethods.Http.Put;
                if (method == "POST") webReq.Method = WebRequestMethods.Http.Post;
                if (method == "DELETE") webReq.Method = method;
                try
                {
                    using (var webResp = webReq.GetResponse())
                    {
                        string webResponse;
                        using (var dataStream = webResp.GetResponseStream())
                        {
                            if (dataStream == null)
                                throw new WebException("Fail to retrieve response stream");

                            using (var dataReader = new StreamReader(dataStream))
                            {
                                webResponse = dataReader.ReadToEnd();
                                dataReader.Close();

                                webResp.Close();
                                webResp.Dispose();
                            }

                            dataStream.Flush();
                        }

                        webResp.Close();

                        return string.IsNullOrEmpty(webResponse) ? null : webResponse;
                    }
                }
                catch (Exception ex)
                {
                    if (ex is WebException wex)
                    {
                        Console.WriteLine($"[WebClient ({method})] ERROR uri: {uri} error: {wex.Message}");
                        Console.WriteLine(ex.StackTrace);

                        if (wex.Status != WebExceptionStatus.Timeout ||
                            retries >= Configuration.Configuration.DeviceRetries) throw;

                        retries++;
                        Console.WriteLine(
                            $"[WebClient ({method})] RETRY {retries} of {Configuration.Configuration.DeviceRetries} for uri: {uri}");
                        continue;
                    }

                    Console.WriteLine($"[WebClient ({method})] ERROR uri: {uri} error: {ex.Message}");
                    throw;
                }
            }
        }

        public static string Get(string uri)
        {
            return Request(uri);
        }
    }
}