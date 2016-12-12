using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    public class PolarFlowManager : SportTrackerManagerBase
    {
        private const string LoginPostDataTemplate = "email={0}&password={1}";
        private const string ServiceUrl = "https://flow.polar.com/";
        private const string ExportTcxUrlTemplate = ServiceUrl + "api/export/training/tcx/{0}";
        private const string DiaryUrlTemplate = ServiceUrl + "training/getCalendarEvents?start={0}&end={1}";

        public PolarFlowManager()
        {
            valueConverter = new PolarFlowConverter();
        }

        public override string GetLoginPostData(string login, string password)
        {
            return string.Format(LoginPostDataTemplate, login, password);
        }

        public override string GetExportTcxUrl(TrainingData data)
        {
            return string.Format(ExportTcxUrlTemplate, data.Id);
        }

        public override string GetAddTrainingPostData(TrainingData data)
        {
            throw new NotImplementedException();
        }

        public override string GetDiaryUrl(DateTime date)
        {
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            return string.Format(DiaryUrlTemplate, firstDayOfMonth.ToString("dd.MM.yyyy"), lastDayOfMonth.ToString("dd.MM.yyyy"));
        }

        public override string LoginUrl
        {
            get
            {
                return ServiceUrl + "login";
            }
        }

        public override string AddTrainingUrl
        {
            get
            {
                return ServiceUrl + "exercises/add";
            }
        }

        protected override void Init(string startPageContent)
        {
        }

        protected override IEnumerable<TrainingData> ExtractTrainingData(string pageContent)
        {
            return null;
        }

        protected override IEnumerable<TrainingData> LoadExtraData(IEnumerable<TrainingData> trainings)
        {
            return trainings;
        }
    }
}
