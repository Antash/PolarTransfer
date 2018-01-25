using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    public class AerobiaManager : SportTrackerManagerBase
    {
        private string userId;
        private string authenticityToken;

        public AerobiaManager()
        {
            ValueConverter = new AerobiaConverter();
        }

        public override string Name => "aerobia";

        public override async Task<bool> LoginAsync(string login, string password)
        {
            var success = await base.LoginAsync(login, password);
            if (success)
            {
                success = Init(await Client.GetPageDataAsync(string.Empty));
            }
            return success;
        }

        public override async Task<bool> RemoveTrainingAsync(string trainingId)
        {
            return await Client.TryPostFormDataAsync(GetTrainingUri(trainingId), GetRemoveTrainingPostData());
        }

        public override async Task AddTrainingResultAsync(TrainingData data)
        {
            await base.AddTrainingResultAsync(data);
            var recentlyAdded = (await GetTrainingListAsync(data.Start)).Single(tr => tr.Start == data.Start);
            recentlyAdded = await LoadTrainingDetailsAsync(recentlyAdded);
            await Client.PostFormDataAsync(GetNotesUri(recentlyAdded.PostId), GetAddNotesPostData(data));
        }

        public override async Task UploadTcxAsync(string tcxData)
        {
            var responce = await Client.PostFormMultipartDataAsync(GetUploadTcxUri(), GetTcxPostData(tcxData));
            dynamic data = JsonConvert.DeserializeObject(responce);
            await Client.GetPageDataAsync(data.continue_path.ToString());
            //TODO handle errors and return bool
        }

        public override async Task UpdateTrainingDataAsync(TrainingData data)
        {
            await base.UpdateTrainingDataAsync(data);
            await Client.PostFormMultipartDataAsync(GetNotesUri(data.PostId), GetExtraUpdateTrainingPostData(data));
        }

        private static Uri GetUploadTcxUri() =>
            new Uri("import/files", UriKind.Relative);

        private static Uri GetNotesUri(string postId) =>
            new Uri($"posts/{postId}", UriKind.Relative);

        protected override string ServiceUrl => "http://aerobia.ru/";

        protected override Uri GetLoginUri() =>
            new Uri("users/sign_in", UriKind.Relative);

        protected override Uri GetAddTrainingUri() =>
            new Uri("workouts", UriKind.Relative);

        protected override Uri GetExportTcxUri(string trainingId) =>
            new Uri($"export/workouts/{trainingId}/tcx", UriKind.Relative);

        protected override Uri GetTrainingUri(string trainingId) =>
            new Uri($"{GetAddTrainingUri()}/{trainingId}", UriKind.Relative);

        private Uri GetTrainingDetailsUri(string trainingId) =>
            new Uri($"users/{userId}/workouts/{trainingId}", UriKind.Relative);

        protected override Uri GetDiaryUri(DateTime date) =>
            new Uri($"users/{userId}/workouts?month={date:yyyy-MM-dd}", UriKind.Relative);

        [Obsolete("Not supported")]
        protected override Uri GetDiaryUri(DateTime start, DateTime end) =>
            GetDiaryUri(start);

        protected override IEnumerable<KeyValuePair<string, string>> GetLoginPostData(string login, string password) =>
            new Dictionary<string, string>
            {
                { "user[email]", login},
                { "user[password]",password}
            };

        protected override IEnumerable<KeyValuePair<string, string>> GetAddTrainingPostData(TrainingData data) =>
            new Dictionary<string, string>
            {
                { "authenticity_token", authenticityToken },
                { "workout[name]", data.Title },
                { "workout[sport_id]", getAerobiaType(data.ActivityType).ToString() },
                { "workout[start_at_date]", data.Start.ToString("dd.MM.yyyy") },
                { "workout[start_at_hours]", data.Start.Hour.ToString() },
                { "workout[start_at_minutes]", data.Start.Minute.ToString() },
                { "workout[distance]", data.Distance.ToString(CultureInfo.InvariantCulture) },
                { "workout[total_time_hours]", data.Duration.Hours.ToString() },
                { "workout[total_time_minutes]", data.Duration.Minutes.ToString() },
                { "workout[total_time_seconds]", data.Duration.Seconds.ToString() },
                { "workout[average_heart_rate]", data.AvgHr.ToString() },
                { "workout[maximum_heart_rate]", data.MaxHr.ToString() },
            };

        private IEnumerable<KeyValuePair<string, object>> GetTcxPostData(string tcxData)
        {
            return new Dictionary<string, object>
            {
                { @"""authenticity_token""", authenticityToken },
                { "workout_file[file][]", (Encoding.ASCII.GetBytes(tcxData), "temp.tcx") },
            };
        }

        //"post[title]"
        //"post[body_text]"
        //"photo[image][]"
        //"post[tag_list]"
        //"post[privacy]"
        //"post[created_at_date]"
        //"post[created_at_hours]"
        //"post[created_at_minutes]"
        private IEnumerable<KeyValuePair<string, object>> GetExtraUpdateTrainingPostData(TrainingData data) =>
            new Dictionary<string, object>
            {
                { @"""_method""", "put" },
                { @"""authenticity_token""", authenticityToken },
                { "post[body]", data.Description ?? string.Empty },
            };

        private IEnumerable<KeyValuePair<string, string>> GetAddNotesPostData(TrainingData data) =>
            new Dictionary<string, string>
            {
                { "_method", "put" },
                { "authenticity_token", authenticityToken },
                { "post[body]", data.Description },
                { "post[body_text]", data.Description },
            };

        private IEnumerable<KeyValuePair<string, string>> GetRemoveTrainingPostData() =>
            new Dictionary<string, string>
            {
                { "_method", "delete" },
                { "authenticity_token", authenticityToken },
            };

        protected override IEnumerable<KeyValuePair<string, string>> GetUpdateTrainingPostData(TrainingData data) =>
            GetAddTrainingPostData(data).Concat(new[]
            {
                new KeyValuePair<string, string>("_method", "put")
            });

        public override async Task<IEnumerable<TrainingData>> GetTrainingListAsync(DateTime start, DateTime end)
        {
            if (end < start)
            {
                throw new ArgumentException("Start date should be less then end.");
            }
            var taskPool = new List<Task<string>>();
            var maxEndDate = new DateTime(end.Year, end.Month, DateTime.DaysInMonth(end.Year, end.Month));
            for (var date = start; date <= maxEndDate; date = date.AddMonths(1))
            {
                taskPool.Add(Client.GetPageDataAsync(GetDiaryUri(date)));
            }
            var pages = await Task.WhenAll(taskPool);
            return pages.SelectMany(ExtractTrainingData).Distinct().Where(t => t.Start >= start && t.Start <= end.AddDays(1));
        }

        protected override IEnumerable<TrainingData> ExtractTrainingData(string pageContent)
        {
            var diarydock = new HtmlDocument();
            diarydock.LoadHtml(pageContent);
            var diaryTable = diarydock.DocumentNode.SelectSingleNode("//div[@class='calendar']").SelectSingleNode("./table");
            var trainings = diaryTable.Descendants("tr").Skip(1)
                .SelectMany(row => row.Elements("td").Where(td => td.ChildNodes["ul"] != null)
                .SelectMany(td => td.ChildNodes["ul"].ChildNodes.Where(node => !string.IsNullOrWhiteSpace(node.InnerHtml))
                .Select(elem =>
                {
                    var info = elem.SelectNodes(".//p").Select(prop => prop.InnerText).ToArray();
                    var id = elem.SelectSingleNode("./a").Attributes["href"].Value.Split('/').Last();
                    string title = null;
                    int index = 0;
                    if (info.Length == 4)
                    {
                        title = info[index++];
                    }
                    if (info.Length >= 3)
                    {
                        var info2 = info[index + 2].Split(',').ToArray();
                        return new TrainingData(id)
                        {
                            Title = title,
                            Start = ValueConverter.GetStartDateTime(info[index + 1]),
                            ActivityType = ValueConverter.GetExcerciseType(info[index]),
                            Distance = info2.Length > 1 ? ValueConverter.GetDistance(info2[0]) : 0,
                            Duration = ValueConverter.GetDuration(info2[info2.Length > 1 ? 1 : 0])
                        };
                    }
                    return null;
                }).ToList()));
            return trainings;
        }

        protected override async Task<TrainingData> LoadExtraData(TrainingData training)
        {
            var page = await Client.GetPageDataAsync(GetTrainingDetailsUri(training.Id));
            HtmlDocument trainingDock = new HtmlDocument();
            trainingDock.LoadHtml(page);

            var regexp = new Regex(@"posts\/(.*)\/comments");
            training.PostId = regexp.Match(page).Groups[1].Value;
            training.Description = trainingDock.DocumentNode.SelectSingleNode("//div[@class='content']").InnerText.Trim();
            var detailsTable = trainingDock.DocumentNode.SelectSingleNode("//table[@class='data']");
            var details = detailsTable.Descendants("tr").Select(row
                => (row.ChildNodes["th"].InnerText.Trim(), row.ChildNodes["td"].InnerText.Replace("&nbsp;", " ").Trim()));
            var tuples = details as (string, string)[] ?? details.ToArray();

            var pulse = tuples.Single(p => p.Item1 == "Пульс").Item2.Split('/');
            int.TryParse(pulse[0].Trim(), out var avghr);
            training.AvgHr = avghr;
            int.TryParse(pulse[1].Trim(), out var maxhr);
            training.MaxHr = maxhr;
            int.TryParse(tuples.Single(p => p.Item1 == "Калории").Item2, out var calories);
            training.Calories = calories;
            return training;
        }

        private bool Init(string startPageContent)
        {
            var regexp = new Regex(@"users\/(.*)\/workouts");
            userId = regexp.Match(startPageContent).Groups[1].Value;
            var startDoc = new HtmlDocument();
            startDoc.LoadHtml(startPageContent);
            authenticityToken = startDoc.DocumentNode.SelectSingleNode("//meta[@name='csrf-token']").Attributes["content"].Value;
            return !string.IsNullOrEmpty(userId);
        }

        //TODO refactoring. Should not be here
        private int getAerobiaType(Excercise activityType)
        {
            switch (activityType)
            {
                case Excercise.Running:
                    return 2;
                case Excercise.Swimming:
                    return 21;
                case Excercise.Cycling:
                    return 1;
                case Excercise.IndoorCycling:
                    return 22;
                case Excercise.Opa:
                    return 72;
                case Excercise.Walking:
                    return 19;
                case Excercise.ScateSkiing:
                    return 3;
                case Excercise.ClassicSkiing:
                    return 83;
                default:
                    return 68;
            }
        }
    }
}