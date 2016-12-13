using System;

namespace SportTrackerManager.Core
{
    public enum Excercise
    {
        Running,
        Swimming,
        IndoorCycling,
        Cycling,
        Gym,
        OPA,
        OWS,
        Triathlon,
        ClassicSkiing,
        ScateSkiing,
        Walking,
        OtherSport,
    }

    public class TrainingData
    {
        public TrainingData()
        {
        }

        public TrainingData(string id)
        {
            Id = id;
        }

        public string Id { get; private set; }
        public string UserId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public Excercise ActivityType { get; set; }
        public DateTime Start { get; set; }

        public double Distance { get; set; }
        public TimeSpan Duration { get; set; }
        public int AvgHr { get; set; }
        public int MaxHr { get; set; }
        public int AvgCadence { get; set; }
        public int Calories { get; set; }
    }
}
