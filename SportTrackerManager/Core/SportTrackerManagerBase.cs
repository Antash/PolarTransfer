using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    public abstract class SportTrackerManagerBase : ISportTrackerManager, IDisposable
    {
        private CookieContainer sessionCookies;
        private readonly HttpClientHandler handler;
        private readonly HttpClient client;

        internal IValueConverter valueConverter;

        public abstract string Name { get; }

        protected abstract Uri ServiceUri { get; }
        protected abstract string GetLoginUrl();
        protected abstract IEnumerable<KeyValuePair<string, string>> GetLoginPostData(string login, string password);
        protected abstract string GetExportTcxUrl(string trainingId);
        protected abstract string GetAddTrainingUrl();
        protected abstract NameValueCollection GetAddTrainingPostData(TrainingData data);
        protected abstract NameValueCollection GetUpdateTrainingPostData(TrainingData data);
        protected abstract string GetDiaryUrl(DateTime date);
        protected virtual string GetDiaryUrl2(DateTime date) { return string.Empty; }
        protected abstract string GetDiaryUrl(DateTime start, DateTime end);
        protected abstract string GetTrainingUrl(string trainingId);

        public SportTrackerManagerBase()
        {
            handler = new HttpClientHandler { AllowAutoRedirect = false };
            client = new HttpClient(handler);
        }

        public virtual void AddTrainingResult(TrainingData data)
        {
            PostFormData(GetAddTrainingUrl(), GetAddTrainingPostData(data));
        }

        public virtual void RemoveTraining(string trainingId)
        {
            var request = CreateRequest(GetTrainingUrl(trainingId), "DELETE");
            using (var responce = (HttpWebResponse)request.GetResponse()) { }
        }

        public IEnumerable<TrainingData> GetTrainingList(DateTime date)
        {
            return ExtractTrainingData(GetPageData(GetDiaryUrl(date)));
        }

        public IEnumerable<TrainingData> GetTrainingList(DateTime start, DateTime end)
        {
            if (end < start)
            {
                throw new ArgumentException("Start date should be less then end.");
            }
            return ExtractTrainingData(GetPageData(GetDiaryUrl(start, end)));
        }

        public TrainingData LoadTrainingDetails(TrainingData data)
        {
            data.Detailed = true;
            return LoadExtraData(data);
        }

        public string GetTrainingFileTcx(string trainingId)
        {
            return GetPageData(GetExportTcxUrl(trainingId));
        }

        public virtual void UploadTcx(string tcxData)
        {
        }

        public async Task<bool> Login(string login, string password)
        {
            var responce = await client.PostAsync(GetLoginUrl(),
                                                  new FormUrlEncodedContent(GetLoginPostData(login, password)));
            return responce.StatusCode != HttpStatusCode.BadRequest;
            /*
            var loginRequest = CreateRequest(GetLoginUrl(), "POST");
            try
            {
                SetPostData(loginRequest, GetLoginPostData(login, password).ToString());
                sessionCookies = loginRequest.CookieContainer;
                using (var responce = (HttpWebResponse)loginRequest.GetResponse())
                using (var reader = new StreamReader(responce.GetResponseStream()))
                {
                    Init(reader.ReadToEnd());
                }
                return sessionCookies.Count > 0;
            }
            catch
            {
                return false;
            }*/
        }

        public virtual void UpdateTrainingData(TrainingData data)
        {
            PostFormData(GetTrainingUrl(data.Id), GetUpdateTrainingPostData(data));
        }

        protected string GetPageData(string url)
        {
            return client.GetStringAsync(url).GetAwaiter().GetResult();
            /*
            var request = CreateRequest(url, "GET");
            using (var responce = (HttpWebResponse)request.GetResponse())
            using (var stream = new StreamReader(responce.GetResponseStream()))
            {
                return stream.ReadToEnd();
            }*/
        }

        protected void PostFormData(string url, NameValueCollection postData)
        {
            var request = CreateRequest(url, "POST");
            SetPostData(request, postData.ToString());
            using (var responce = (HttpWebResponse)request.GetResponse()) { }
        }

        protected string PostFormData(string url, string postData, string bound)
        {
            var request = CreateMultipartRequest(url, bound);
            SetPostData(request, postData);
            using (var responce = (HttpWebResponse)request.GetResponse())
            using (var stream = new StreamReader(responce.GetResponseStream()))
            {
                return stream.ReadToEnd();
            }
        }

        protected abstract void Init(string startPageContent);
        protected abstract IEnumerable<TrainingData> ExtractTrainingData(string pageContent);
        protected abstract TrainingData LoadExtraData(TrainingData training);

        private HttpResponseMessage PostRequest(string url, HttpContent postData)
        {
            return client.PostAsync(url, postData).Result;
        }

        private HttpResponseMessage GetRequest(string url)
        {
            return client.GetAsync(url).Result;
        }

        private HttpWebRequest CreateRequest(string url, string method)
        {
            HttpWebRequest reqest = (HttpWebRequest)WebRequest.Create(url);
            reqest.Proxy = WebRequest.DefaultWebProxy;
            reqest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            reqest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
            reqest.Method = method;
            reqest.CookieContainer = sessionCookies ?? new CookieContainer();
            return reqest;
        }

        private HttpWebRequest CreateMultipartRequest(string url, string bound)
        {
            HttpWebRequest reqest = (HttpWebRequest)WebRequest.Create(url);
            reqest.Proxy = WebRequest.DefaultWebProxy;
            reqest.ContentType = $"multipart/form-data; boundary={bound}";
            reqest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
            reqest.Method = "POST";
            reqest.CookieContainer = sessionCookies ?? new CookieContainer();
            return reqest;
        }

        private void SetPostData(HttpWebRequest request, string postData)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = bytes.Length;
            using (Stream os = request.GetRequestStream())
            {
                os.Write(bytes, 0, bytes.Length);
            }
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
