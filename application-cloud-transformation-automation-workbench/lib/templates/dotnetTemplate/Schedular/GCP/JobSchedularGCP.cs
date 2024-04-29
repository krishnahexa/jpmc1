namespace Infrastructure.Common.Scheduler.GCP
{
    using Google.Api.Gax.ResourceNames;
    using Google.Protobuf.WellKnownTypes;
    using Google.Cloud.Scheduler.V1;
    using System.Threading.Tasks;

    public sealed partial class JobSchedularGCP : IJobSchedularGCP
    {
        private readonly CloudSchedulerClient cloudSchedulerClient;
        private readonly string project, location;
        public JobSchedularGCP(string Project, string Location)
        {
            project = Project;
            location = Location;
            cloudSchedulerClient =  CloudSchedulerClient.Create();
        }
        /// <summary>Snippet for CreateJobAsync</summary>
        /// <remarks>
        /// This snippet has been automatically generated for illustrative purposes only.
        /// It may require modifications to work in your environment.
        /// </remarks>
        public async Task CreateJobAsync(Job job)
        {
                        
            // Initialize request argument(s)
            CreateJobRequest request = new CreateJobRequest
            {
                ParentAsLocationName = LocationName.FromProjectLocation(project, location),
                Job = job,
            };
            // Make the request
            Job response = await cloudSchedulerClient.CreateJobAsync(request);
       }

        public async Task DeleteJobAsync(string jobName)
        {
            DeleteJobRequest request = new DeleteJobRequest
            {
                JobName = JobName.FromProjectLocationJob(project, location, jobName),
            };
            // Make the request
            await cloudSchedulerClient.DeleteJobAsync(request);
        }

        public async Task GetJobRequestObjectAsync(string jobName)
        {
            
            // Initialize request argument(s)
            GetJobRequest request = new GetJobRequest
            {
                JobName = JobName.FromProjectLocationJob(project, location, jobName),
            };
            // Make the request
            Job response = await cloudSchedulerClient.GetJobAsync(request);
        }

        public async Task RunJobRequestObjectAsync(string jobName)
        {
            
            // Initialize request argument(s)
            RunJobRequest request = new RunJobRequest
            {
                JobName = JobName.FromProjectLocationJob(project, location, jobName),
            };
            // Make the request
            Job response = await cloudSchedulerClient.RunJobAsync(request);
        }

         public async Task UpdateJobRequestObjectAsync(Job job)
        {
            
            // Initialize request argument(s)
            UpdateJobRequest request = new UpdateJobRequest
            {
                Job = job,
                UpdateMask = new FieldMask(),
            };
            // Make the request
            Job response = await cloudSchedulerClient.UpdateJobAsync(request);
        }

    }
}