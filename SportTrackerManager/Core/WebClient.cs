using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    public class WebClient
    {
        private CookieContainer sessionCookies;
        private HttpClient client;

        public HttpWebRequest CreateMultipartRequest(string url, string bound)
        {
            HttpWebRequest reqest = (HttpWebRequest)WebRequest.Create(url);
            reqest.Proxy = WebRequest.DefaultWebProxy;
            reqest.ContentType = $"multipart/form-data; boundary={bound}";
            reqest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
            reqest.Method = "POST";
            reqest.CookieContainer = sessionCookies ?? new CookieContainer();
            return reqest;
        }
    }
}
