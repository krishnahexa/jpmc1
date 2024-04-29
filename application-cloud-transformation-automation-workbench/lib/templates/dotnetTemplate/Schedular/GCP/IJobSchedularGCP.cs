

namespace Infrastructure.Common.Scheduler.GCP
{
    // [START cloudscheduler_v1_generated_CloudScheduler_CreateJob_async_flattened]
    using Google.Api.Gax.ResourceNames;
    using Google.Protobuf.WellKnownTypes;
    using Google.Cloud.Scheduler.V1;
    using System.Threading.Tasks;

    public interface IJobSchedularGCP
    {
          Task CreateJobAsync(Job job);
          Task DeleteJobAsync(string jobName);
          Task GetJobRequestObjectAsync(string jobName);
          Task RunJobRequestObjectAsync(string jobName);
          Task UpdateJobRequestObjectAsync(Job job);
    }
}