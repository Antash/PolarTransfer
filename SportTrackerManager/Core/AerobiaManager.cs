using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SportTrackerManager.Core
{
    public class AerobiaManager : SportTrackerManagerBase
    {
        private const string LoginPostDataTemplate = "user%5Bemail%5D={0}&user%5Bpassword%5D={1}";
        private const string ServiceUrl = "http://aerobia.ru/";
        private const string ExportTcxUrlTemplate = ServiceUrl + "export/workouts/{0}/tcx";
        private const string TrainingUrlTemplate = ServiceUrl + "users/{0}/workouts/{1}";
        private const string DiaryUrlTemplate = ServiceUrl + "users/{0}/workouts?month={1}";

        private string userId;

        public AerobiaManager()
        {
            valueConverter = new AerobiaConverter();
        }

        public override string LoginUrl
        {
            get
            {
                return ServiceUrl + "users/sign_in";
            }
        }

        public override string AddTrainingUrl
        {
            get
            {
                throw new NotImplementedException();
            }
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
            return string.Format(TrainingUrlTemplate, userId, trainingId);
        }

        public override string GetAddTrainingPostData(TrainingData data)
        {
            throw new NotImplementedException();
        }

        public override string GetDiaryUrl(DateTime date)
        {
            return string.Format(DiaryUrlTemplate, userId, date.ToString("yyyy-MM-dd"));
        }

        public override string GetDiaryUrl(DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        protected override void Init(string startPageContent)
        {
            var regexp = new Regex(@"users\/(.*)\/workouts");
            userId = regexp.Match(startPageContent).Groups[1].Value;
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
                    if (info.Count() == 4)
                    {
                        title = info[index++];
                    }
                    if (info.Count() >= 3)
                    {
                        var info2 = info[index+2].Split(',').ToArray();
                        return new TrainingData(id)
                        {
                            Title = title,
                            Start = valueConverter.GetStartDateTime(info[index+1]),
                            ActivityType = valueConverter.GetExcerciseType(info[index]),
                            Distance = info2.Count() > 1 ? valueConverter.GetDistance(info2[0]) : 0,
                            Duration = valueConverter.GetDuration(info2[info2.Count() > 1 ? 1 : 0])
                        };
                    }
                    else
                    {
                        return null;
                    }
                }).ToList()));
            return trainings;
        }

        protected override IEnumerable<TrainingData> LoadExtraData(IEnumerable<TrainingData> trainings)
        {
            return trainings.Select(tr =>
            {
                var page = GetPageData(GetTrainingUrl(tr.Id));
                HtmlDocument trainingDock = new HtmlDocument();
                trainingDock.LoadHtml(page);
                tr.Description = trainingDock.DocumentNode.SelectSingleNode("//div[@class='content']").InnerText.Trim();
                var detailsTable = trainingDock.DocumentNode.SelectSingleNode("//table[@class='data']");
                var details = detailsTable.Descendants("tr").Select(row 
                    => new Tuple<string, string>(row.ChildNodes["th"].InnerText.Trim(), row.ChildNodes["td"].InnerText.Replace("&nbsp;", " ").Trim()));
                var pulse = details.Single(p => p.Item1 == "Пульс").Item2.Split('/');
                int avghr;
                int.TryParse(pulse[0].Trim(), out avghr);
                tr.AvgHr = avghr;
                int maxhr;
                int.TryParse(pulse[1].Trim(), out maxhr);
                tr.MaxHr = maxhr;
                int calories;
                int.TryParse(details.Single(p => p.Item1 == "Калории").Item2, out calories);
                tr.Calories = calories;
                return tr;
            });
        }
    }
}
