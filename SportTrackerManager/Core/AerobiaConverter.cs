using System;
using System.Globalization;

namespace SportTrackerManager.Core
{
    internal class AerobiaConverter : IValueConverter
    {
        //TODO ENG
        private const string TimeSpanFormatRu = @"h\ч\:mm\м\:ss\с";
        private const string TimeSpanFormatHRu = @"mm\м\:ss\с";
        private const string TimeSpanFormatEn = @"mm\m\:ss\s";
        private const string TimeSpanFormatHEn = @"h\h\:mm\m\:ss\s";
        private const string DateTimeFormatRu = "d MMM'.' yyyy'г, в' HH:mm";
        private const string DateTimeFormatEn = "";

        public double GetDistance(string text)
        {
            var dist = text.Replace("км", string.Empty).Trim();
            return double.Parse(dist, CultureInfo.InvariantCulture);
        }

        public TimeSpan GetDuration(string text)
        {
            TimeSpan duration;
            if (!TimeSpan.TryParseExact(text.Trim(), TimeSpanFormatRu, CultureInfo.GetCultureInfo("ru-RU"), out duration))
            {
                return TimeSpan.ParseExact(text.Trim(), TimeSpanFormatHRu, CultureInfo.GetCultureInfo("ru-RU"));
            }
            return duration;
        }

        public Excercise GetExcerciseType(string text)
        {
            //TODO extend
            switch (text.ToLower())
            {
                case "бег":
                    return Excercise.Running;
                case "офп":
                    return Excercise.OPA;
                case "плавание":
                    return Excercise.Swimming;
                case "прогулочный велосипед":
                case "велоспорт":
                    return Excercise.Cycling;
                case "триатлон":
                    return Excercise.Triathlon;
                case "велотренажер":
                    return Excercise.IndoorCycling;
                case "прогулка":
                    return Excercise.Walking;
                case "тренажерный зал":
                    return Excercise.Gym;
                case "лыжи коньковый ход":
                    return Excercise.ScateSkiing;
                case "лыжи классический ход":
                    return Excercise.ClassicSkiing;
                default:
                    return Excercise.OtherSport;
            }
        }

        public DateTime GetStartDateTime(string text)
        {
            return DateTime.ParseExact(PrepareDate(text), DateTimeFormatRu, CultureInfo.GetCultureInfo("ru-RU"));
        }

        private string PrepareDate(string text)
        {
            return text.Trim()
                .Replace("нояб", "ноя")
                .Replace("мая", "май.")
                .Replace("июня", "июн.")
                .Replace("июля", "июл.")
                .Replace("сент", "сен");
        }
    }
}
