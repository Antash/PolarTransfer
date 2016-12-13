using System;
using System.Collections.Generic;

namespace SportTrackerManager.Core
{
    /// <summary>
    /// Base API for the iteraction with the sport trackers
    /// </summary>
    public interface ISportTrackerManager
    {
        /// <summary>
        /// Logins to web service.
        /// </summary>
        /// <param name="login">User name.</param>
        /// <param name="password">Password.</param>
        /// <returns>Value indicating whethe login was successfull or not.</returns>
        bool Login(string login, string password);

        /// <summary>
        /// Downloads tcx training data dile.
        /// </summary>
        /// <param name="trainingId">Training id.</param>
        /// <returns>File content.</returns>
        string GetTrainingFileTcx(string trainingId);

        /// <summary>
        /// Uploads tcx training data dile.
        /// </summary>
        /// <param name="tcxData">File content.</param>
        void UploadTcx(string tcxData);

        /// <summary>
        /// Creates new training result.
        /// </summary>
        /// <param name="data">Training data.</param>
        void AddTrainingResult(TrainingData data);

        /// <summary>
        /// Removes training result.
        /// </summary>
        /// <param name="trainingId">Training id.</param>
        void RemoveTraining(string trainingId);

        /// <summary>
        /// Updates training result summary.
        /// </summary>
        /// <param name="data">New training data with corrsponding id.</param>
        void UpdateTrainingData(TrainingData data);

        /// <summary>
        /// Downloads trainings from the diary for the specified month.
        /// </summary>
        /// <param name="date">Date.</param>
        /// <returns>Training data collection.</returns>
        IEnumerable<TrainingData> GetTrainingList(DateTime date);

        /// <summary>
        /// Downloads trainings from the diary for the time period.
        /// </summary>
        /// <param name="start">Start date.</param>
        /// <param name="end">End date</param>
        /// <returns>Training data collection.</returns>
        IEnumerable<TrainingData> GetTrainingList(DateTime start, DateTime end);
    }
}
