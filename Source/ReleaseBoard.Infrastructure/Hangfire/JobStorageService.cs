using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Newtonsoft.Json;
using ReleaseBoard.Application.Interfaces;

namespace ReleaseBoard.Infrastructure.Hangfire
{
    /// <summary>
    /// Сервис для работы с <see cref="JobStorage"/>.
    /// </summary>
    public class JobStorageService : IJobStorageService
    {
        /// <inheritdoc />
        public List<(string, string)> GetAllActiveJobsForUser(string userId, int limit = int.MaxValue)
        {
            var jobStorageApi = JobStorage.Current.GetMonitoringApi();
            var jobEvents = new List<(string, string)>();
            
            jobStorageApi.ProcessingJobs(0, limit).ForEach(x =>
            {
                string jobId = x.Key;

                IDictionary<string, string> properties = jobStorageApi.JobDetails(jobId).Properties;
                if (properties.TryGetValue(JobParameterKeys.EventType, out string eventType) && 
                    properties.TryGetValue(JobParameterKeys.UserId, out string userIdFromParameter) &&
                    userIdFromParameter == userId)
                {
                    jobEvents.Add((jobId, eventType));
                }
            });

            return jobEvents;
        }

        /// <inheritdoc />
        public T GetJobResult<T>(string jobId) where T : class
        {
            string result = GetSucceededJobResult(jobId);

            if (result == null)
            {
                throw new InvalidCastException($"Result of {jobId} is null.");
            }

            return JsonConvert.DeserializeObject<T>(result);
        }

        /// <inheritdoc />
        public void SetJobParameter(string jobId, string eventType, string userId)
        {
            IStorageConnection connection = JobStorage.Current.GetConnection();
            connection.SetJobParameter(jobId, JobParameterKeys.EventType, eventType);
            connection.SetJobParameter(jobId, JobParameterKeys.UserId, userId);
        }

        private string GetSucceededJobResult(string jobId)
        {
            var api = JobStorage.Current.GetMonitoringApi();
            SucceededJobDto jobDto = api.SucceededJobs(0, int.MaxValue).FirstOrDefault(job => job.Key == jobId).Value;
            
            return jobDto.Result as string;
        }
    }
}
