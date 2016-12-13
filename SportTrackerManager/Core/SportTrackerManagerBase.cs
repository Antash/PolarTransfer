using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace SportTrackerManager.Core
{
    public abstract class SportTrackerManagerBase : ISportTrackerManagerInternal, ISportTrackerManager
    {
        private CookieContainer sessionCookies;

        internal IValueConverter valueConverter;

        public abstract string LoginUrl { get; }
        public abstract string GetLoginPostData(string login, string password);
        public abstract string GetExportTcxUrl(TrainingData data);
        public abstract string AddTrainingUrl { get; }
        public abstract string GetAddTrainingPostData(TrainingData data);
        public abstract string GetDiaryUrl(DateTime date);
        public abstract string GetTrainingUrl(TrainingData data);

        public void AddTrainingResult(TrainingData data)
        {
            PostFormData(AddTrainingUrl, GetAddTrainingPostData(data));
        }

        public void RemoveTraining(TrainingData data)
        {
            var request = CreateRequest(GetTrainingUrl(data), "DELETE");
            using (var responce = (HttpWebResponse)request.GetResponse()) { }
        }

        public void AddTrainingTarget()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TrainingData> GetExercises(DateTime date)
        {
            return LoadExtraData(ExtractTrainingData(GetPageData(GetDiaryUrl(date))));
        }

        public string GetTrainingFileTcx(TrainingData data)
        {
            return GetPageData(GetExportTcxUrl(data));
        }

        public bool Login(string login, string password)
        {
            var loginRequest = CreateRequest(LoginUrl, "POST");
            try
            {
                SetPostData(loginRequest, GetLoginPostData(login, password));
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
            }
        }

        public void UpdateTrainingData(TrainingData data)
        {
            throw new NotImplementedException();
        }

        protected string GetPageData(string url)
        {
            var request = CreateRequest(url, "GET");
            using (var responce = (HttpWebResponse)request.GetResponse())
            using (var stream = new StreamReader(responce.GetResponseStream()))
            {
                return stream.ReadToEnd();
            }
        }

        protected void PostFormData(string url, string postData)
        {
            var request = CreateRequest(url, "POST");
            SetPostData(request, postData);
            using (var responce = (HttpWebResponse)request.GetResponse()) { }
        }

        protected abstract void Init(string startPageContent);
        protected abstract IEnumerable<TrainingData> ExtractTrainingData(string pageContent);
        protected abstract IEnumerable<TrainingData> LoadExtraData(IEnumerable<TrainingData> trainings);

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

        private void SetPostData(HttpWebRequest request, string postData)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(postData);
            request.ContentLength = bytes.Length;
            using (Stream os = request.GetRequestStream())
            {
                os.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
