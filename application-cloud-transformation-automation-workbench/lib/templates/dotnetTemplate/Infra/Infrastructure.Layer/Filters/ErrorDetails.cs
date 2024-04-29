namespace Infrastructure.Common
{
    internal class ErrorDetails
    {
        public ErrorDetails()
        {
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string DeveloperMessage { get; set; }
    }
}