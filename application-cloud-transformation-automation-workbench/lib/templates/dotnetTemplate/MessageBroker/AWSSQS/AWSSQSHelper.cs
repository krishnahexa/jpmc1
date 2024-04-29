using Amazon.SQS;  
using Amazon.SQS.Model;  
using Microsoft.Extensions.Options;  
using Newtonsoft.Json;  
using System;  
using System.Collections.Generic;  
using System.Linq;  
using System.Threading.Tasks;  
namespace AWSSQS.Helpers  
{  
    public interface IAWSSQSHelper  
    {  
        Task<bool> SendMessageAsync(string Message);  
        Task<List<Message>> ReceiveMessageAsync();  
        Task<bool> DeleteMessageAsync(string messageReceiptHandle);  
    }  
    
    public class AWSSQSConfiguration  
    {  
        public string QueueUrl { get; set; }  
    }  
    public class AWSSQSHelper: IAWSSQSHelper  
    {  
        private readonly IAmazonSQS _sqs;  
        private readonly AWSSQSConfiguration _settings;  
        public AWSSQSHelper(  
           IAmazonSQS sqs,  
           IOptions<AWSSQSConfiguration> settings)  
        {  
            this._sqs = sqs;  
            this._settings = settings.Value;  
        }  
        public async Task<bool> SendMessageAsync(string message)  
        {  
            try  
            {  

                var sendRequest = new SendMessageRequest(_settings.QueueUrl, message);  
                var sendResult = await _sqs.SendMessageAsync(sendRequest);  
  
                return sendResult.HttpStatusCode == System.Net.HttpStatusCode.OK;  
            }  
            catch (Exception ex)  
            {  
                throw ex;  
            }  
        }  
        public async Task<List<Message>> ReceiveMessageAsync()  
        {  
            try  
            {  
                //Create New instance  
                var request = new ReceiveMessageRequest  
                {  
                    QueueUrl = _settings.QueueUrl,  
                    MaxNumberOfMessages = 10,  
                    WaitTimeSeconds = 5  
                };  
                //CheckIs there any new message available to process  
                var result = await _sqs.ReceiveMessageAsync(request);  
                      
                return result.Messages.Any() ? result.Messages : new List<Message>();  
            }  
            catch (Exception ex)  
            {  
                throw ex;  
            }  
        }  
        public async Task<bool> DeleteMessageAsync(string messageReceiptHandle)  
        {  
            try  
            {  
                //Deletes the specified message from the specified queue  
                var deleteResult = await _sqs.DeleteMessageAsync(_settings.QueueUrl, messageReceiptHandle);  
                return deleteResult.HttpStatusCode == System.Net.HttpStatusCode.OK;  
            }  
            catch (Exception ex)  
            {  
                throw ex;  
            }  
        }  
    }  
}  
