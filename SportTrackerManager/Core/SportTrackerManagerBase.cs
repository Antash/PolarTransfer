using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    public abstract class SportTrackerManagerBase : ISportTrackerManager, IDisposable
    {
        private WebClient Client => new WebClient(ServiceUri);

        private readonly HttpClientHandler handler;
        private readonly HttpClient client;

        internal IValueConverter ValueConverter;

        public abstract string Name { get; }

        protected abstract string ServiceUrl { get; }
        protected Uri ServiceUri => new Uri(ServiceUrl);
        protected abstract string GetLoginUrl();
        protected abstract IEnumerable<KeyValuePair<string, string>> GetLoginPostData(string login, string password);
        protected abstract string GetExportTcxUrl(string trainingId);
        protected abstract string GetAddTrainingUrl();
        protected abstract IEnumerable<KeyValuePair<string, string>> GetAddTrainingPostData(TrainingData data);
        protected abstract IEnumerable<KeyValuePair<string, string>> GetUpdateTrainingPostData(TrainingData data);
        protected abstract string GetDiaryUrl(DateTime date);
        protected abstract string GetDiaryUrl(DateTime start, DateTime end);
        protected abstract string GetTrainingUrl(string trainingId);

        protected SportTrackerManagerBase()
        {
            handler = new HttpClientHandler();
            client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
        }

        public virtual async Task AddTrainingResult(TrainingData data)
        {
            await client.PostAsync(GetAddTrainingUrl(), new FormUrlEncodedContent(GetAddTrainingPostData(data)));
        }

        public virtual async Task<bool> RemoveTraining(string trainingId)
        {
            var responce = await client.DeleteAsync(GetTrainingUrl(trainingId));
            return responce.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<TrainingData>> GetTrainingList(DateTime date)
        {
            return ExtractTrainingData(await GetPageDataAsync(GetDiaryUrl(date)));
        }

        public async Task<IEnumerable<TrainingData>> GetTrainingList(DateTime start, DateTime end)
        {
            if (end < start)
            {
                throw new ArgumentException("Start date should be less then end.");
            }
            return ExtractTrainingData(await GetPageDataAsync(GetDiaryUrl(start, end)));
        }

        public async Task<TrainingData> LoadTrainingDetails(TrainingData data)
        {
            return await LoadExtraData(data);
        }

        public async Task<string> GetTrainingFileTcxAsync(string trainingId)
        {
            return await client.GetStringAsync(GetExportTcxUrl(trainingId));
        }

        protected async Task<string> GetPageData(string url)
        {
            return await client.GetStringAsync(url);
        }

        public abstract Task UploadTcxAsync(string tcxData);

        public async Task<bool> Login(string login, string password)
        {
            var responce = await client.PostAsync(GetLoginUrl(), new FormUrlEncodedContent(GetLoginPostData(login, password)));
            Init(await responce.Content.ReadAsStringAsync());
            return responce.StatusCode != HttpStatusCode.BadRequest;
        }

        public virtual async Task UpdateTrainingData(TrainingData data)
        {
            await client.PostAsync(GetTrainingUrl(data.Id), new FormUrlEncodedContent(GetUpdateTrainingPostData(data)));
        }

        protected async Task<string> GetPageDataAsync(string url)
        {
            return await client.GetStringAsync(url);
        }

        protected async Task PostFormData(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            await client.PostAsync(url, new FormUrlEncodedContent(postData));
        }

        protected async Task<string> PostFormData(string url, IEnumerable<KeyValuePair<string, HttpContent>> postData)
        {
            var boundary = $"--------boundary{Guid.NewGuid()}";
            using (var content = new MultipartFormDataContent(boundary))
            {
                foreach (var data in postData)
                {
                    if (data.Value is ByteArrayContent file)
                    {
                        content.Add(file, data.Key, "TempFile.tcx");
                    }
                    else
                    {
                        content.Add(data.Value, data.Key);
                    }
                }
                
                var responce = await client.PostAsync(url, content);
                return await responce.Content.ReadAsStringAsync();
            }
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
        protected abstract Task<TrainingData> LoadExtraData(TrainingData training);

        private HttpWebRequest CreateMultipartRequest(string url, string bound)
        {
            HttpWebRequest reqest = (HttpWebRequest)WebRequest.Create(url);
            reqest.Proxy = WebRequest.DefaultWebProxy;
            reqest.ContentType = $"multipart/form-data; boundary={bound}";
            reqest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
            reqest.Method = "POST";
            reqest.CookieContainer = handler.CookieContainer;
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
            handler.Dispose();
        }
    }
}
