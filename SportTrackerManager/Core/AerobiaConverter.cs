using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    internal class AerobiaConverter : IValueConverter
    {
        //TODO ENG
        private const string TimeSpanFormatRu = @"H'ч'\:mm'м'\:ss'с'";
        private const string TimeSpanFormatEn = "";
        private const string DateTimeFormatRu = " d MMM'.' yyyy'г, в' HH:mm";
        private const string DateTimeFormatEn = "";

        public int GetDistanceMeters(string text)
        {
            //TODO
            return 0;
        }

        public TimeSpan GetDuration(string text)
        {
            return TimeSpan.ParseExact(text, TimeSpanFormatRu, CultureInfo.GetCultureInfo("ru-RU"));
        }

        public Excercise GetExcerciseType(string text)
        {
            //TODO
            return Excercise.Running;
        }

        public DateTime GetStartDateTime(string text)
        {
            return DateTime.ParseExact(PrepareDate(text), DateTimeFormatRu, CultureInfo.GetCultureInfo("ru-RU"));
        }

        private string PrepareDate(string text)
        {
            //TODO extend
            return text.Replace("нояб", "ноя");
        }
    }
}
