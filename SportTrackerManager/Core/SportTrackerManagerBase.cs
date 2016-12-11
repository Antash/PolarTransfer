﻿using System;
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
        private CookieCollection sessionCookies;

        public abstract string AddTrainingUrl { get; }

        public abstract string ExportTcxUrlTemplate { get; }

        public abstract string LoginPostDataTemplate { get; }

        public abstract string LoginUrl { get; }

        public abstract string ServiceUrl { get; }

        public bool AddTrainingResult(TrainingData data)
        {
            var request = CreateRequest(AddTrainingUrl, "POST");
            try
            {
                string postData = string.Format("day=11&month=12&year=2016&hours=16&minutes=0&sport=122&note=&durationHours=1&durationMinutes=0&durationSeconds=0&distance=&maximumHeartRate=&averageHeartRate=&minimumHeartRate=&kiloCalories=&pace=&speed=&cadence=&feeling=");
                SetPostData(request, postData);
                var responce = (HttpWebResponse)request.GetResponse();
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

        public IEnumerable<TrainingData> GetExercises(DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        public string GetTrainingFileTcx(TrainingData data)
        {
            var request = CreateRequest(string.Format(ExportTcxUrlTemplate, data.Id), "GET");
            var responce = (HttpWebResponse)request.GetResponse();
            var stream = new StreamReader(responce.GetResponseStream());
            return stream.ReadToEnd();
        }

        public bool Login(string login, string password)
        {
            var loginRequest = CreateRequest(LoginUrl, "POST");
            try
            {
                SetPostData(loginRequest, string.Format(LoginPostDataTemplate, login, password));
                var responce = (HttpWebResponse)loginRequest.GetResponse();
                sessionCookies = responce.Cookies;
                var reader = new StreamReader(responce.GetResponseStream());
                var page = reader.ReadToEnd();
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

        private HttpWebRequest CreateRequest(string url, string method)
        {
            HttpWebRequest reqest = (HttpWebRequest)WebRequest.Create(url);
            reqest.Proxy = WebRequest.DefaultWebProxy;
            reqest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            reqest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
            reqest.Method = method;
            reqest.CookieContainer = new CookieContainer();
            if (sessionCookies != null)
            {
                reqest.CookieContainer.Add(sessionCookies);
            }
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
