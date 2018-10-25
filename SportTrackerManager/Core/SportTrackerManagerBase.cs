using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    public abstract class SportTrackerManagerBase : ISportTrackerManager, IDisposable
    {
        internal IValueConverter ValueConverter;

        protected SportTrackerManagerBase()
        {
            Client = new WebClient(ServiceUri);
        }

        #region ISportTrackerManager

        public abstract string Name { get; }

        public virtual async Task AddTrainingResultAsync(TrainingData data)
        {
            await Client.PostFormDataAsync(GetAddTrainingUri(), GetAddTrainingPostData(data));
        }

        public virtual async Task<bool> RemoveTrainingAsync(string trainingId)
        {
            return await Client.DeleteAsync(GetTrainingUri(trainingId));
        }

        public virtual async Task<IEnumerable<TrainingData>> GetTrainingListAsync(DateTime date)
        {
            return ExtractTrainingData(await Client.GetPageDataAsync(GetDiaryUri(date)))
                .Where(t => t.Start.Month == date.Month);
        }

        public virtual async Task<IEnumerable<TrainingData>> GetTrainingListAsync(DateTime start, DateTime end)
        {
            if (end < start)
            {
                throw new ArgumentException("Start date should be less then end.");
            }
            return ExtractTrainingData(await Client.GetPageDataAsync(GetDiaryUri(start, end)));
        }

        public virtual async Task<TrainingData> LoadTrainingDetailsAsync(TrainingData data)
        {
            return await LoadExtraData(data);
        }

        public virtual async Task<string> GetTrainingFileTcxAsync(string trainingId)
        {
            return await Client.GetPageDataAsync(GetExportTcxUri(trainingId));
        }

        public virtual Task UploadTcxAsync(string tcxData)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> LoginAsync(string login, string password)
        {
            var page = await Client.GetPageDataAsync(GetLoginUri());
            return await Client.TryPostFormDataAsync(GetLoginUri(), GetLoginPostData(login, password));
        }

        public virtual async Task UpdateTrainingDataAsync(TrainingData data)
        {
            await Client.PostFormDataAsync(GetTrainingUri(data.Id), GetUpdateTrainingPostData(data));
        }

        #endregion

        public void Dispose()
        {
            Client.Dispose();
        }

        #region protected

        protected WebClient Client { get; }

        protected abstract string ServiceUrl { get; }

        protected Uri ServiceUri => new Uri(ServiceUrl);

        protected abstract Uri GetLoginUri();

        protected abstract Uri GetExportTcxUri(string trainingId);

        protected abstract Uri GetAddTrainingUri();

        protected abstract Uri GetDiaryUri(DateTime date);

        protected abstract Uri GetDiaryUri(DateTime start, DateTime end);

        protected abstract Uri GetTrainingUri(string trainingId);

        protected abstract IEnumerable<KeyValuePair<string, string>> GetLoginPostData(string login, string password);

        protected abstract IEnumerable<KeyValuePair<string, string>> GetAddTrainingPostData(TrainingData data);

        protected abstract IEnumerable<KeyValuePair<string, string>> GetUpdateTrainingPostData(TrainingData data);

        protected abstract IEnumerable<TrainingData> ExtractTrainingData(string pageContent);

        protected abstract Task<TrainingData> LoadExtraData(TrainingData training);

        #endregion
    }
}
