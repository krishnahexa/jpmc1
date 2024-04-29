using Amazon;
using Amazon.Batch;
using Amazon.Batch.Model;

namespace Infrastructure.Common.Scheduler.AWS;



public sealed partial class JobSchedularAWS : IJobSchedularAWS
{
    private IAmazonBatch amazonBatchClient;
    private string profileName;
    private RegionEndpoint regionEndpoint;

     public JobSchedularAWS(String ProfileName, RegionEndpoint RegionEndpoint, 
     IAmazonBatch AmazonBatch)
    {
        profileName = ProfileName;
        regionEndpoint = RegionEndpoint;
        amazonBatchClient = AmazonBatch;
    }
    public async Task CreateJobAsync(SubmitJobRequest submitJobRequest)
    {
        // Load credentials from a local credential profile and create service client in
        // same region as batch job is configured
        var chain = new Amazon.Runtime.CredentialManagement.CredentialProfileStoreChain();
        if (chain.TryGetAWSCredentials(profileName, out var awsCredentials))
        {
            var batchClient = new AmazonBatchClient(awsCredentials, regionEndpoint);

            /* Alternate, if you're running with a credential profile named 'default', or running
               the code on an compute instance with an attached role vending temporary credentials,
               you can omit the credentials object and use:

               var batchClient = new AmazonBatchClient(Amazon.RegionEndpoint.USWest2);
             */


            try
            {
                var response = await amazonBatchClient.SubmitJobAsync(submitJobRequest);

                Console.WriteLine($"Submitted job yielding job id {response.JobId}");
            }
            catch (AmazonBatchException e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public async Task DeleteJobAsync(DeleteJobQueueRequest deleteJobQueueRequest)
    {
        // Load credentials from a local credential profile and create service client in
        // same region as batch job is configured
        var chain = new Amazon.Runtime.CredentialManagement.CredentialProfileStoreChain();
        if (chain.TryGetAWSCredentials(profileName, out var awsCredentials))
        {
            var batchClient = new AmazonBatchClient(awsCredentials, regionEndpoint);

            /* Alternate, if you're running with a credential profile named 'default', or running
               the code on an compute instance with an attached role vending temporary credentials,
               you can omit the credentials object and use:

               var batchClient = new AmazonBatchClient(Amazon.RegionEndpoint.USWest2);
             */


            try
            {
                var response = await amazonBatchClient.DeleteJobQueueAsync(deleteJobQueueRequest);

                Console.WriteLine($"Deleting Scheduled job");
            }
            catch (AmazonBatchException e)
            {
                Console.WriteLine(e);
            }
        }
    }


    public async Task<DescribeJobsResponse> GetJobRequestObjectAsync(DescribeJobsRequest describeJobRequest)
    {
        DescribeJobsResponse response = null;
        // Load credentials from a local credential profile and create service client in
        // same region as batch job is configured
        var chain = new Amazon.Runtime.CredentialManagement.CredentialProfileStoreChain();
        if (chain.TryGetAWSCredentials(profileName, out var awsCredentials))
        {
            var batchClient = new AmazonBatchClient(awsCredentials, regionEndpoint);

            /* Alternate, if you're running with a credential profile named 'default', or running
               the code on an compute instance with an attached role vending temporary credentials,
               you can omit the credentials object and use:

               var batchClient = new AmazonBatchClient(Amazon.RegionEndpoint.USWest2);
             */
            

            try
            {
                 response = await amazonBatchClient.DescribeJobsAsync(describeJobRequest);
                 Console.WriteLine($"Deleting Scheduled job");
                 
            }
            catch (AmazonBatchException e)
            {
                Console.WriteLine(e);
            }
            
        }
        return response;
    }

    public async Task<UpdateJobQueueResponse> UpdateJobRequestObjectAsync(UpdateJobQueueRequest updateJobQueueRequest)
    {
        UpdateJobQueueResponse response = null;
        // Load credentials from a local credential profile and create service client in
        // same region as batch job is configured
        var chain = new Amazon.Runtime.CredentialManagement.CredentialProfileStoreChain();
        if (chain.TryGetAWSCredentials(profileName, out var awsCredentials))
        {
            var batchClient = new AmazonBatchClient(awsCredentials, regionEndpoint);

            /* Alternate, if you're running with a credential profile named 'default', or running
               the code on an compute instance with an attached role vending temporary credentials,
               you can omit the credentials object and use:

               var batchClient = new AmazonBatchClient(Amazon.RegionEndpoint.USWest2);
             */


            try
            {
                 response = await amazonBatchClient.UpdateJobQueueAsync(updateJobQueueRequest);

                Console.WriteLine($"Update Scheduled job");
            }
            catch (AmazonBatchException e)
            {
                Console.WriteLine(e);
            }
        }
        return response;
    }
}