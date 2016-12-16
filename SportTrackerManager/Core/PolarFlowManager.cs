using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace SportTrackerManager.Core
{
    public class PolarFlowManager : SportTrackerManagerBase
    {
        private const string ServiceUrl = "https://flow.polar.com/";

        public PolarFlowManager()
        {
            valueConverter = new PolarFlowConverter();
        }

        protected override Uri ServiceUri { get; } = new Uri("https://flow.polar.com/");

        protected override NameValueCollection GetLoginPostData(string login, string password)
        {
            return HttpUtility.ParseQueryString($"email={login}&password={password}");
        }

        protected override string GetExportTcxUrl(string trainingId)
        {
            return ServiceUrl + $"api/export/training/tcx/{trainingId}";
        }

        protected override string GetTrainingUrl(string trainingId)
        {
            return ServiceUrl + $"training/analysis/{trainingId}";
        }

        protected override NameValueCollection GetAddTrainingPostData(TrainingData data)
        {
            var postData = HttpUtility.ParseQueryString(string.Empty);
            postData["day"] = data.Start.Day.ToString();
            postData["month"] = data.Start.Month.ToString();
            postData["year"] = data.Start.Year.ToString();
            postData["hours"] = data.Start.Hour.ToString();
            postData["minutes"] = data.Start.Minute.ToString();
            postData["sport"] = getPolarType(data.ActivityType).ToString();
            postData["note"] = data.Description;
            postData["durationHours"] = data.Duration.Hours.ToString();
            postData["durationMinutes"] = data.Duration.Minutes.ToString();
            postData["durationSeconds"] = data.Duration.Seconds.ToString();
            postData["distance"] = data.Distance.ToString();
            postData["maximumHeartRate"] = data.MaxHr > data.AvgHr ? data.MaxHr.ToString() : string.Empty;
            postData["averageHeartRate"] = data.AvgHr > 0 ? data.AvgHr.ToString() : string.Empty;
            postData["minimumHeartRate"] = string.Empty;
            postData["kiloCalories"] = data.Calories > 0 ? data.Calories.ToString() : string.Empty;
            postData["pace"] = string.Empty;
            postData["speed"] = string.Empty;
            postData["cadence"] = data.AvgCadence > 0 ? data.AvgCadence.ToString() : string.Empty;
            postData["feeling"] = string.Empty;
            return postData;
        }

        protected override NameValueCollection GetUpdateTrainingPostData(TrainingData data)
        {
            var postData = HttpUtility.ParseQueryString(string.Empty);
            postData["id"] = data.Id;
            postData["userId"] = data.UserId;
            postData["preciseDuration"] = data.Duration.ToString(@"hh\:mm\:ss");
            postData["preciseDistanceStr"] = (data.Distance * 1000).ToString();
            postData["duration"] = data.Duration.ToString(@"hh\:mm\:ss");
            postData["distanceStr"] = data.Distance.ToString();
            postData["heartRateAvg"] = data.AvgHr.ToString();
            postData["kiloCalories"] = data.Calories.ToString();
            postData["sport"] = getPolarType(data.ActivityType).ToString();
            postData["note"] = data.Description;
            return postData;
        }

        protected override string GetDiaryUrl(DateTime date)
        {
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            return ServiceUrl + $"training/getCalendarEvents?start={firstDayOfMonth.ToString("dd.MM.yyyy")}&end={lastDayOfMonth.ToString("dd.MM.yyyy")}";
        }

        protected override string GetDiaryUrl(DateTime start, DateTime end)
        {
            return ServiceUrl + $"training/getCalendarEvents?start={start.ToString("dd.MM.yyyy")}&end={end.ToString("dd.MM.yyyy")}";
        }

        protected override string GetLoginUrl()
        {
             return ServiceUrl + "login";
        }

        protected override string GetAddTrainingUrl()
        {
             return ServiceUrl + "exercises/add";
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
                        Duration = TimeSpan.FromMilliseconds((int)item.duration),
                        Distance = item.distance != null ? (double)item.distance / 1000 : 0,
                        Calories = item.calories != null ? (int)item.calories : 0,
                    };
                }
            }
        }

        protected override TrainingData LoadExtraData(TrainingData training)
        {
            var page = GetPageData(GetTrainingUrl(training.Id));
            HtmlDocument trainingDock = new HtmlDocument();
            trainingDock.LoadHtml(page);
            training.Description = trainingDock.DocumentNode.SelectSingleNode("//textarea[@id='note']").InnerText.Trim();
            var selectedSport = trainingDock.DocumentNode.SelectSingleNode("//select[@id='sport']")
                .SelectNodes("./option").Skip(1).Single(node => node.Attributes["selected"] != null);
            training.ActivityType = valueConverter.GetExcerciseType(selectedSport.Attributes["value"].Value);
            training.UserId = trainingDock.DocumentNode.SelectSingleNode("//input[@id='userId']").Attributes["value"].Value;
            //optional
            var hrNode = trainingDock.DocumentNode.SelectSingleNode("//span[@id='BDPHrAvg']");
            training.AvgHr = hrNode != null ? int.Parse(hrNode.InnerText.Trim()) : 0;
            //TODO : extract cadence and max heart rate
            return training;
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
