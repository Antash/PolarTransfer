using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    public class AerobiaManager : SportTrackerManagerBase
    {
        private const string LoginPostDataTemplate = "user%5Bemail%5D={0}&user%5Bpassword%5D={1}";
        private const string ServiceUrl = "http://aerobia.ru/";
        private const string ExportTcxUrlTemplate = ServiceUrl + "export/workouts/{0}/tcx";
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
            return string.Format(DiaryUrlTemplate, userId, date.ToString("yyyy-MM-dd"));
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
                    if (info.Count() != 3)
                    {
                        //TODO process title 4 field
                        return null;
                    }
                    var info2 = info[2].Split(',').ToArray();
                    if (info2.Count() == 2)
                    {
                        return new TrainingData(id)
                        {
                            Start = valueConverter.GetStartDateTime(info[1]),
                            ActivityType = valueConverter.GetExcerciseType(info[0]),
                            Distance = valueConverter.GetDistanceMeters(info2[0]),
                            Duration = valueConverter.GetDuration(info2[1])
                        };
                    }
                    else
                    {
                        return new TrainingData(id)
                        {
                            Start = valueConverter.GetStartDateTime(info[1]),
                            ActivityType = valueConverter.GetExcerciseType(info[0]),
                            Duration = valueConverter.GetDuration(info2[0])
                        };
                    }
                }).ToList()));
            return trainings;
        }
    }
}
