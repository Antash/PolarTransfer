using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public override string GetExportTcxUrl(string trainingId)
        {
            return string.Format(ExportTcxUrlTemplate, trainingId);
        }

        public override string GetTrainingUrl(string trainingId)
        {
            return string.Format(TrainingUrlTemplate, trainingId);
        }

        public override string GetAddTrainingPostData(TrainingData data)
        {
            string maxHr = data.MaxHr > data.AvgHr ? data.MaxHr.ToString() : string.Empty;
            return $"day={data.Start.Day}&month={data.Start.Month}&year={data.Start.Year}&hours={data.Start.Hour}&minutes={data.Start.Minute}"
                + $"&sport={getPolarType(data.ActivityType)}&note={data.Description}"
                + $"&durationHours={data.Duration.Hours}&durationMinutes={data.Duration.Minutes}&durationSeconds={data.Duration.Seconds}"
                + $"&distance={data.Distance}&maximumHeartRate={maxHr}&averageHeartRate={data.AvgHr}&minimumHeartRate="
                + $"&kiloCalories={data.Calories}&pace=&speed=&cadence={data.AvgCadence}&feeling=";
        }

        public override string GetDiaryUrl(DateTime date)
        {
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            return string.Format(DiaryUrlTemplate, firstDayOfMonth.ToString("dd.MM.yyyy"), lastDayOfMonth.ToString("dd.MM.yyyy"));
        }

        public override string GetDiaryUrl(DateTime start, DateTime end)
        {
            return string.Format(DiaryUrlTemplate, start.ToString("dd.MM.yyyy"), end.ToString("dd.MM.yyyy"));
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
                var page = GetPageData(GetTrainingUrl(tr.Id));
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

        private int getPolarType(Excercise activityType)
        {
            switch (activityType)
            {
                case Excercise.Running:
                    return 1;
                case Excercise.Swimming:
                    return 103;
                case Excercise.Cycling:
                    return 2;
                case Excercise.IndoorCycling:
                    return 18;
                case Excercise.OPA:
                    return 15;
                case Excercise.Walking:
                    return 3;
                case Excercise.ScateSkiing:
                case Excercise.ClassicSkiing:
                    return 6;
                default:
                    return 83;
            }
        }
    }
}
