using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    public class PolarFlowManager : SportTrackerManagerBase
    {
        public PolarFlowManager()
        {
            //TODO get rid of value converter
            ValueConverter = new PolarFlowConverter();
        }

        public override string Name => "polar";            

        protected override string ServiceUrl => "https://flow.polar.com/";

        protected override Uri GetExportTcxUri(string trainingId) =>
            new Uri($"api/export/training/tcx/{trainingId}", UriKind.Relative);

        protected override Uri GetTrainingUri(string trainingId) =>
            new Uri($"training/analysis/{trainingId}", UriKind.Relative);

        protected override Uri GetDiaryUri(DateTime date)
        {
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            return new Uri($"training/getCalendarEvents?start={firstDayOfMonth:dd.MM.yyyy}&end={lastDayOfMonth:dd.MM.yyyy}", UriKind.Relative);
        }

        protected override Uri GetDiaryUri(DateTime start, DateTime end) =>
            new Uri($"training/getCalendarEvents?start={start:dd.MM.yyyy}&end={end:dd.MM.yyyy}", UriKind.Relative);

        protected override Uri GetLoginUri() =>
            new Uri("login", UriKind.Relative);

        protected override Uri GetAddTrainingUri() =>
            new Uri("exercises/add", UriKind.Relative);

        protected override IEnumerable<KeyValuePair<string, string>> GetLoginPostData(string login, string password) =>
            new Dictionary<string, string>
            {
                { "email", login },
                { "password", password }
            };

        protected override IEnumerable<KeyValuePair<string, string>> GetAddTrainingPostData(TrainingData data) =>
            new Dictionary<string, string>
            {
                { "day", data.Start.Day.ToString() },
                { "month", data.Start.Month.ToString() },
                { "year", data.Start.Year.ToString() },
                { "hours", data.Start.Hour.ToString() },
                { "minutes", data.Start.Minute.ToString() },
                { "sport", getPolarType(data.ActivityType).ToString() },
                { "note", data.Description },
                { "durationHours", data.Duration.Hours.ToString() },
                { "durationMinutes", data.Duration.Minutes.ToString() },
                { "durationSeconds", data.Duration.Seconds.ToString() },
                { "distance", data.Distance.ToString(CultureInfo.InvariantCulture) },
                { "maximumHeartRate", data.MaxHr > data.AvgHr ? data.MaxHr.ToString() : string.Empty },
                { "averageHeartRate", data.AvgHr > 0 ? data.AvgHr.ToString() : string.Empty },
                { "minimumHeartRate", string.Empty },
                { "kiloCalories", data.Calories > 0 ? data.Calories.ToString() : string.Empty },
                { "pace", string.Empty },
                { "speed", string.Empty },
                { "cadence", data.AvgCadence > 0 ? data.AvgCadence.ToString() : string.Empty },
                { "feeling", string.Empty },
            };

        protected override IEnumerable<KeyValuePair<string, string>> GetUpdateTrainingPostData(TrainingData data) =>
            new Dictionary<string, string>
            {
                {"id", data.Id},
                {"userId", data.UserId},
                {"preciseDuration", data.Duration.ToString(@"hh\:mm\:ss")},
                {"preciseDistanceStr", (data.Distance * 1000).ToString(CultureInfo.InvariantCulture)},
                {"duration", data.Duration.ToString(@"hh\:mm\:ss")},
                {"distanceStr", data.Distance.ToString(CultureInfo.InvariantCulture)},
                {"heartRateAvg", data.AvgHr.ToString()},
                {"kiloCalories", data.Calories.ToString()},
                {"sport", getPolarType(data.ActivityType).ToString()},
                {"note", data.Description},
            };

        protected override IEnumerable<TrainingData> ExtractTrainingData(string pageContent)
        {
            dynamic data = JsonConvert.DeserializeObject(pageContent);
            foreach (var item in data)
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

        protected override async Task<TrainingData> LoadExtraData(TrainingData training)
        {
            var page = await Client.GetPageDataAsync(GetTrainingUri(training.Id));
            var trainingDock = new HtmlDocument();
            trainingDock.LoadHtml(page);
            training.Description = trainingDock.DocumentNode.SelectSingleNode("//textarea[@id='note']").InnerText.Trim();
            var selectedSport = trainingDock.DocumentNode.SelectSingleNode("//select[@id='sport']")
                .SelectNodes("./option").Skip(1).Single(node => node.Attributes["selected"] != null);
            training.ActivityType = ValueConverter.GetExcerciseType(selectedSport.Attributes["value"].Value);
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
                case Excercise.Opa:
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
