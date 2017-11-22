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
        Opa,
        Ows,
        Triathlon,
        ClassicSkiing,
        ScateSkiing,
        Walking,
        OtherSport,
    }

    public class TrainingData : IEquatable<TrainingData>
    {
        public TrainingData()
        {
        }

        public TrainingData(string id)
        {
            Id = id;
        }

        public bool Detailed { get; internal set; }

        public string Id { get; }
        public string UserId { get; set; }
        public string PostId { get; set; }

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

        public bool Equals(TrainingData other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
