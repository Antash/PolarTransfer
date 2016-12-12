using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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

        public bool AddTrainingResult(TrainingData data)
        {
            var request = CreateRequest(AddTrainingUrl, "POST");
            try
            {
                //TODO
                string postData = string.Format("day=11&month=12&year=2016&hours=16&minutes=0&sport=1&note=&durationHours=1&durationMinutes=0&durationSeconds=0&distance=&maximumHeartRate=&averageHeartRate=&minimumHeartRate=&kiloCalories=&pace=&speed=&cadence=&feeling=");
                SetPostData(request, postData);
                using (var responce = (HttpWebResponse)request.GetResponse())
                using (var reader = new StreamReader(responce.GetResponseStream()))
                {
                    var page = reader.ReadToEnd();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddTrainingTarget()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TrainingData> GetExercises(DateTime date)
        {
            var request = CreateRequest(GetDiaryUrl(date), "GET");
            using (var responce = (HttpWebResponse)request.GetResponse())
            using (var stream = new StreamReader(responce.GetResponseStream()))
            {
                return ExtractTrainingData(stream.ReadToEnd());
            }
        }

        public string GetTrainingFileTcx(TrainingData data)
        {
            var request = CreateRequest(GetExportTcxUrl(data), "GET");
            using (var responce = (HttpWebResponse)request.GetResponse())
            using (var stream = new StreamReader(responce.GetResponseStream()))
            {
                return stream.ReadToEnd();
            }
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

        public bool UpdateTrainingData(TrainingData data)
        {
            throw new NotImplementedException();
        }

        protected abstract void Init(string startPageContent);
        protected abstract IEnumerable<TrainingData> ExtractTrainingData(string pageContent);

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
