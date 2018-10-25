using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    public class PPTManager : SportTrackerManagerBase
    {
        public override string Name { get; }

        protected override string ServiceUrl
        {
            get { return "https://polarpersonaltrainer.com/"; }
        }
        protected override Uri GetLoginUri()
        {
            return new Uri("https://auth.polar.com/login", UriKind.Absolute);
        }

        protected override Uri GetExportTcxUri(string trainingId)
        {
            throw new NotImplementedException();
        }

        protected override Uri GetAddTrainingUri()
        {
            throw new NotImplementedException();
        }

        protected override Uri GetDiaryUri(DateTime date)
        {
            throw new NotImplementedException();
        }

        protected override Uri GetDiaryUri(DateTime start, DateTime end)
        {
            return new Uri($"user/calendar/inc/listview.ftl?startDate={start:dd.MM.yyyy}&endDate={end:dd.MM.yyyy}", UriKind.Relative);
        }

        protected override Uri GetTrainingUri(string trainingId)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<KeyValuePair<string, string>> GetLoginPostData(string login, string password)
        {
            return new Dictionary<string, string>
            {
                { "username", login },
                { "password", password }
            };

        }

        protected override IEnumerable<KeyValuePair<string, string>> GetAddTrainingPostData(TrainingData data)
        {
            throw new NotImplementedException();

        }

        protected override IEnumerable<KeyValuePair<string, string>> GetUpdateTrainingPostData(TrainingData data)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<TrainingData> ExtractTrainingData(string pageContent)
        {
            throw new NotImplementedException();
        }

        protected override Task<TrainingData> LoadExtraData(TrainingData training)
        {
            throw new NotImplementedException();
        }
    }
}
