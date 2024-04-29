using Microsoft.AspNetCore.Http;  
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Options;
using System.Collections.Generic;  
using System.IO;  
using System.Threading.Tasks;
using static Infrastructure.Common.StartUpExtensionMethods;

namespace Infrastructure.Common.Storage
{  
    public class AzureBlobStorage
    {  
        private readonly IOptions<MyConfig> config;  
  
  
        public AzureBlobStorage(IOptions<MyConfig> config)  
        {  
            this.config = config;  
        }  
  
       
        public async Task<List<string>> ListFiles()  
        {  
            List<string> blobs = new List<string>();  
            try  
            {  
                if (CloudStorageAccount.TryParse(config.Value.StorageConnection, out CloudStorageAccount storageAccount))  
                {  
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();  
  
                    CloudBlobContainer container = blobClient.GetContainerReference(config.Value.Container);  
  
                    BlobResultSegment resultSegment = await container.ListBlobsSegmentedAsync(null);  
                    foreach (IListBlobItem item in resultSegment.Results)  
                    {  
                        if (item.GetType() == typeof(CloudBlockBlob))  
                        {  
                            CloudBlockBlob blob = (CloudBlockBlob)item;  
                            blobs.Add(blob.Name);  
                        }  
                        else if (item.GetType() == typeof(CloudPageBlob))  
                        {  
                            CloudPageBlob blob = (CloudPageBlob)item;  
                            blobs.Add(blob.Name);  
                        }  
                        else if (item.GetType() == typeof(CloudBlobDirectory))  
                        {  
                            CloudBlobDirectory dir = (CloudBlobDirectory)item;  
                            blobs.Add(dir.Uri.ToString());  
                        }  
                    }  
                }  
            }  
            catch  
            {  
            }  
            return blobs;  
        }  
  
    
        public async Task<bool> UploadFile(IFormFile asset)  
        {  
                if (CloudStorageAccount.TryParse(config.Value.StorageConnection, out CloudStorageAccount storageAccount))  
                {  
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();  
  
                    CloudBlobContainer container = blobClient.GetContainerReference(config.Value.Container);  
  
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(asset.FileName);  
  
                    await blockBlob.UploadFromStreamAsync(asset.OpenReadStream());  
  
                    return true;  
                }  
                else  
                {  
                    return false;  
                }  
        }  
       
        public async Task<Stream> DownloadFile(string fileName)  
        {  
            MemoryStream ms = new MemoryStream();  
            Stream blobStream = null;
            if (CloudStorageAccount.TryParse(config.Value.StorageConnection, out CloudStorageAccount storageAccount))  
            {  
                CloudBlobClient BlobClient = storageAccount.CreateCloudBlobClient();  
                CloudBlobContainer container = BlobClient.GetContainerReference(config.Value.Container);  
                CloudBlob file = container.GetBlobReference(fileName);  
                await file.DownloadToStreamAsync(ms);  
                blobStream = file.OpenReadAsync().Result;        
            }
            return blobStream;
        }


        public async Task<bool> DeleteFile(string fileName)  
        {  
            if (CloudStorageAccount.TryParse(config.Value.StorageConnection, out CloudStorageAccount storageAccount))  
            {  
                CloudBlobClient BlobClient = storageAccount.CreateCloudBlobClient();  
                CloudBlobContainer container = BlobClient.GetContainerReference(config.Value.Container);  

                if (await container.ExistsAsync())  
                {  
                    CloudBlob file = container.GetBlobReference(fileName);  

                    if (await file.ExistsAsync())  
                    {  
                        await file.DeleteAsync();  
                    }  
                }  
            }  
            return true;  
        }  
    }  
}  
