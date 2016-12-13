using HtmlAgilityPack;
using Newtonsoft.Json;
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
        private const string TrainingUrlTemplate = ServiceUrl + "training/analysis/{0}";
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
            dynamic data = JsonConvert.DeserializeObject(pageContent);
            foreach (dynamic item in data)
            {
                if (item.type == "EXERCISE")
                {
                    yield return new TrainingData((string)item.listItemId)
                    {
                        Start = (DateTime)item.datetime,
                        Distance = (double)item.distance / 1000,
                        Duration = TimeSpan.FromMilliseconds((int)item.duration),
                        Calories = (int)item.calories
                    };
                }
            }
        }

        protected override IEnumerable<TrainingData> LoadExtraData(IEnumerable<TrainingData> trainings)
        {
            return trainings.Select(tr =>
            {
                var page = GetPageData(string.Format(TrainingUrlTemplate, tr.Id));
                HtmlDocument trainingDock = new HtmlDocument();
                trainingDock.LoadHtml(page);
                tr.Description = trainingDock.DocumentNode.SelectSingleNode("//textarea[@id='note']").InnerText.Trim();
                var selectedSport = trainingDock.DocumentNode.SelectSingleNode("//select[@id='sport']")
                    .SelectNodes("./option").Skip(1).Single(node => node.Attributes["selected"] != null);
                tr.ActivityType = valueConverter.GetExcerciseType(selectedSport.Attributes["value"].Value);

                //optional
                var hrNode = trainingDock.DocumentNode.SelectSingleNode("//span[@id='BDPHrAvg']");
                tr.AvgHr = hrNode != null ? int.Parse(hrNode.InnerText.Trim()) : 0;
                //TODO : extract cadence and max heart rate
                return tr;
            });
        }
    }
}
