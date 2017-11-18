using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    /// <summary>
    /// Base API for the iteraction with the sport trackers
    /// </summary>
    public interface ISportTrackerManager
    {
        /// <summary>
        /// Gets service name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Logins to web service.
        /// </summary>
        /// <param name="login">User name.</param>
        /// <param name="password">Password.</param>
        /// <returns>Value indicating whethe login was successfull or not.</returns>
        Task<bool> Login(string login, string password);

        /// <summary>
        /// Downloads tcx training data dile.
        /// </summary>
        /// <param name="trainingId">Training id.</param>
        /// <returns>File content.</returns>
        Task<string> GetTrainingFileTcxAsync(string trainingId);

        /// <summary>
        /// Uploads tcx training data dile.
        /// </summary>
        /// <param name="tcxData">File content.</param>
        Task UploadTcxAsync(string tcxData);

        /// <summary>
        /// Creates new training result.
        /// </summary>
        /// <param name="data">Training data.</param>
        Task AddTrainingResult(TrainingData data);

        /// <summary>
        /// Removes training result.
        /// </summary>
        /// <param name="trainingId">Training id.</param>
        Task<bool> RemoveTraining(string trainingId);

        /// <summary>
        /// Updates training result summary.
        /// </summary>
        /// <param name="data">New training data with corrsponding id.</param>
        Task UpdateTrainingData(TrainingData data);

        /// <summary>
        /// Gets brief training session list from the diary for the specified month.
        /// </summary>
        /// <param name="date">Date.</param>
        /// <returns>Training data collection.</returns>
        Task<IEnumerable<TrainingData>> GetTrainingList(DateTime date);

        /// <summary>
        /// Gets brief training session list from the diary for the time period.
        /// </summary>
        /// <param name="start">Start date.</param>
        /// <param name="end">End date</param>
        /// <returns>Training data collection.</returns>
        Task<IEnumerable<TrainingData>> GetTrainingList(DateTime start, DateTime end);

        /// <summary>
        /// Downloads missing training session info from the sport tracker.
        /// </summary>
        /// <param name="data">Training data.</param>
        /// <returns>DetailedTraining data.</returns>
        Task<TrainingData> LoadTrainingDetails(TrainingData data);
    }
}
