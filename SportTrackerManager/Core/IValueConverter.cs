using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
