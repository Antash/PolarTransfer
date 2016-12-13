using System;

namespace SportTrackerManager.Core
{
    internal interface IValueConverter
    {
        Excercise GetExcerciseType(string text);
        DateTime GetStartDateTime(string text);
        TimeSpan GetDuration(string text);
        double GetDistance(string text);
    }
}
