using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    public class AerobiaManager : SportTrackerManagerBase
    {
        private const string ServiceUrl = "http://aerobia.ru/";

        private string userId;
        private string authenticityToken;

        public AerobiaManager()
        {
            ValueConverter = new AerobiaConverter();
        }

        protected override Uri ServiceUri => new Uri(ServiceUrl);

        public override string Name => "aerobia";

        public override async Task<bool> RemoveTraining(string trainingId)
        {
            await PostFormData(GetTrainingUrl(trainingId), GetRemoveTrainingPostData());
            return true;
        }

        public override async Task AddTrainingResult(TrainingData data)
        {
            await base.AddTrainingResult(data);
            var recentlyAdded = GetTrainingList(data.Start).GetAwaiter().GetResult().Single(tr => tr.Start == data.Start);
            recentlyAdded = LoadTrainingDetails(recentlyAdded).GetAwaiter().GetResult();
            await PostFormData(GetNotesUrl(recentlyAdded.PostId), GetAddNotesPostData(data));
        }

        public override async Task UploadTcxAsync(string tcxData)
        {
            var responce = await PostFormData("http://aerobia.ru/import/files", GetTcxPostData(tcxData));
            dynamic data = JsonConvert.DeserializeObject(responce);
            GetPageData(ServiceUrl + data.continue_path);
        }

        public override async Task UpdateTrainingData(TrainingData data)
        {
            await base.UpdateTrainingData(data);
            var bound = "--------boundary" + Guid.NewGuid();
            var postData = WriteMultipartFormPost(data, bound).ReadAsStringAsync().GetAwaiter().GetResult();
            PostFormData(GetNotesUrl(data.PostId), postData, bound);
        }

        private IEnumerable<KeyValuePair<string, HttpContent>> GetTcxPostData(string data)
        {
            var file = new ByteArrayContent(Encoding.ASCII.GetBytes(data));
            file.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
            return new Dictionary<string, HttpContent>
            {
                { @"""authenticity_token""", new StringContent(authenticityToken) },
                { "workout_file[file][]", file },
            };
        }

        private MultipartFormDataContent WriteMultipartFormPost(TrainingData data, string bound)
        {
            var c = new MultipartFormDataContent(bound);
            c.Add(new StringContent("put"), @"""_method""");
            c.Add(new StringContent(authenticityToken), @"""authenticity_token""");
            c.Add(new StringContent(data.Description ?? string.Empty), "post[body]");
            //c.Add(new StringContent(data.Title ?? string.Empty), "post[title]");
            //c.Add(new StringContent(data.Description ?? string.Empty), "post[body_text]");
            //c.Add(new ByteArrayContent(new byte[] { }), "photo[image][]", string.Empty);
            //c.Add(new StringContent(string.Empty), "post[tag_list]");
            //c.Add(new StringContent("0"), "post[privacy]");
            //c.Add(new StringContent(data.Start.ToString("dd.MM.yyyy")), "post[created_at_date]");
            //c.Add(new StringContent(data.Start.Hour.ToString()), "post[created_at_hours]");
            //c.Add(new StringContent(data.Start.Minute.ToString()), "post[created_at_minutes]");
            return c;
        }

        private IEnumerable<KeyValuePair<string, string>> GetAddNotesPostData(TrainingData data)
        {
            return new Dictionary<string, string>
            {
                { "_method", "put" },
                { "authenticity_token", authenticityToken },
                { "post[body]", data.Description },
                { "post[body_text]", data.Description },
            };
        }

        private IEnumerable<KeyValuePair<string, string>> GetRemoveTrainingPostData()
        {
            return new Dictionary<string, string>
            {
                { "_method", "delete" },
                { "authenticity_token", authenticityToken },
            };
        }

        private string GetNotesUrl(string postId)
        {
            return ServiceUrl + $"posts/{postId}";
        }

        protected override string GetLoginUrl()
        {
            return ServiceUrl + "users/sign_in";
        }

        protected override string GetAddTrainingUrl()
        {
            return ServiceUrl + "workouts";
        }

        protected override IEnumerable<KeyValuePair<string, string>> GetLoginPostData(string login, string password)
        {
            return new Dictionary<string, string>
            {
                {"user[email]", login},
                { "user[password]",password}
            };
        }

        protected override string GetExportTcxUrl(string trainingId)
        {
            return $"{ServiceUrl}export/workouts/{trainingId}/tcx";
        }

        protected override string GetTrainingUrl(string trainingId)
        {
            return GetAddTrainingUrl() + $"/{trainingId}";
        }

        private string GetTrainingDetailsUrl(string trainingId)
        {
            return ServiceUrl + $"users/{userId}/workouts/{trainingId}";
        }

        protected override IEnumerable<KeyValuePair<string, string>> GetAddTrainingPostData(TrainingData data)
        {
            return new Dictionary<string, string>
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
        }

        protected override IEnumerable<KeyValuePair<string, string>> GetUpdateTrainingPostData(TrainingData data)
        {
            var postData = GetAddTrainingPostData(data).ToDictionary(pair => pair.Key, pair => pair.Value);
            postData.Add("_method", "put");
            return postData;
        }

        protected override string GetDiaryUrl(DateTime date)
        {
            return ServiceUrl + $"users/{userId}/workouts?month={date:yyyy-MM-dd}";
        }

        protected override string GetDiaryUrl2(DateTime date)
        {
            return $"users/{userId}/workouts?month={date:yyyy-MM-dd}";
        }

        protected override string GetDiaryUrl(DateTime start, DateTime end)
        {
            return GetDiaryUrl(start);
        }

        protected override void Init(string startPageContent)
        {
            var regexp = new Regex(@"users\/(.*)\/workouts");
            userId = regexp.Match(startPageContent).Groups[1].Value;
            HtmlDocument startDoc = new HtmlDocument();
            startDoc.LoadHtml(startPageContent);
            authenticityToken = startDoc.DocumentNode.SelectSingleNode("//meta[@name='csrf-token']").Attributes["content"].Value;
        }

        protected override IEnumerable<TrainingData> ExtractTrainingData(string pageContent)
        {
            HtmlDocument diarydock = new HtmlDocument();
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
            var page = await GetPageData(GetTrainingDetailsUrl(training.Id));
            HtmlDocument trainingDock = new HtmlDocument();
            trainingDock.LoadHtml(page);

            var regexp = new Regex(@"posts\/(.*)\/comments");
            training.PostId = regexp.Match(page).Groups[1].Value;
            training.Description = trainingDock.DocumentNode.SelectSingleNode("//div[@class='content']").InnerText.Trim();
            var detailsTable = trainingDock.DocumentNode.SelectSingleNode("//table[@class='data']");
            var details = detailsTable.Descendants("tr").Select(row
                => new Tuple<string, string>(row.ChildNodes["th"].InnerText.Trim(), row.ChildNodes["td"].InnerText.Replace("&nbsp;", " ").Trim()));
            var pulse = details.Single(p => p.Item1 == "Пульс").Item2.Split('/');
            int.TryParse(pulse[0].Trim(), out var avghr);
            training.AvgHr = avghr;
            int.TryParse(pulse[1].Trim(), out var maxhr);
            training.MaxHr = maxhr;
            int.TryParse(details.Single(p => p.Item1 == "Калории").Item2, out var calories);
            training.Calories = calories;
            return training;
        }

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
                case Excercise.OPA:
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