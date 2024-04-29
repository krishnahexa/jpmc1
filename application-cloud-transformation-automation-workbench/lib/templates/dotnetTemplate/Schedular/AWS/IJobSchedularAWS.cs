

namespace Infrastructure.Common.Scheduler.AWS
{
    // [START cloudscheduler_v1_generated_CloudScheduler_CreateJob_async_flattened]
    
    
    using Amazon;
    using Amazon.Batch;
    using Amazon.Batch.Model;
    using System.Threading.Tasks;

    public interface IJobSchedularAWS
    {
          Task CreateJobAsync(SubmitJobRequest submitJobRequest);
          Task DeleteJobAsync(DeleteJobQueueRequest deleteJobQueueRequest);
          Task<DescribeJobsResponse> GetJobRequestObjectAsync(DescribeJobsRequest describeJobsRequest);
          
          Task<UpdateJobQueueResponse> UpdateJobRequestObjectAsync(UpdateJobQueueRequest updateJobQueueRequest);
    }
}