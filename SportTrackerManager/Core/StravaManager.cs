using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    public class StravaManager : SportTrackerManagerBase
    {
        public override string Name => "strava";

        protected override string ServiceUrl => "https://www.strava.com/";

        protected override Uri GetLoginUri() =>
            new Uri("oauth/authorize", UriKind.Relative);

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
            throw new NotImplementedException();
        }

        protected override Uri GetTrainingUri(string trainingId)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<KeyValuePair<string, string>> GetLoginPostData(string login, string password) =>
            new Dictionary<string, string>
            {
                { "client_id", login },
                { "response_type", "code" },
                { "redirect_uri", "127.0.0.1" },
            };

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
