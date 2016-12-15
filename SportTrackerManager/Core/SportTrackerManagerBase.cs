using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace SportTrackerManager.Core
{
    public abstract class SportTrackerManagerBase : ISportTrackerManager
    {
        private CookieContainer sessionCookies;

        internal IValueConverter valueConverter;

        protected abstract string GetLoginUrl();
        protected abstract NameValueCollection GetLoginPostData(string login, string password);
        protected abstract string GetExportTcxUrl(string trainingId);
        protected abstract string GetAddTrainingUrl();
        protected abstract NameValueCollection GetAddTrainingPostData(TrainingData data);
        protected abstract NameValueCollection GetUpdateTrainingPostData(TrainingData data);
        protected abstract string GetDiaryUrl(DateTime date);
        protected abstract string GetDiaryUrl(DateTime start, DateTime end);
        protected abstract string GetTrainingUrl(string trainingId);

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

        public void UploadTcx(string tcxData)
        {
            throw new NotImplementedException();
        }

        public bool Login(string login, string password)
        {
            var loginRequest = CreateRequest(GetLoginUrl(), "POST");
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

        public virtual void UpdateTrainingData(TrainingData data)
        {
            PostFormData(GetTrainingUrl(data.Id), GetUpdateTrainingPostData(data));
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

        protected void PostFormData(string url, NameValueCollection postData)
        {
            var request = CreateRequest(url, "POST");
            SetPostData(request, postData);
            using (var responce = (HttpWebResponse)request.GetResponse()) { }
        }

        protected abstract void Init(string startPageContent);
        protected abstract IEnumerable<TrainingData> ExtractTrainingData(string pageContent);
        protected abstract TrainingData LoadExtraData(TrainingData training);

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

        private void SetPostData(HttpWebRequest request, NameValueCollection postData)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(postData.ToString());
            request.ContentLength = bytes.Length;
            using (Stream os = request.GetRequestStream())
            {
                os.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
