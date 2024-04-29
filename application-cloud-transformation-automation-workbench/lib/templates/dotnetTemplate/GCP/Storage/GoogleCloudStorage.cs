using Google.Cloud.Storage.V1;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
namespace Infrastructure.Common.Storage
{
    public class GoogleCloudStorage
    {
        private readonly string _bucketName;
        private readonly StorageClient _storage;

        public GoogleCloudStorage()
        {
            _bucketName = "weatherforecast_bucket";
            _storage = StorageClient.Create();
        }

        public async Task UploadFile(Stream stream, string objectName)
        {
            using (stream)
            {
                await _storage.UploadObjectAsync(_bucketName,objectName,null,stream);
            }
        }

        /*public async Task<IEnumerable<string>> ListFiles()
        {
            var fileList = new List<string>();
            var objects = _storage.ListObjects(_bucketName);
            await foreach (var obj in objects)
            {
                fileList.Add(obj.Name);

            }
            return fileList;
        }*/
        public async Task<IEnumerable<Google.Apis.Storage.v1.Data.Object>> ListFiles()
        {
            var objects =new List<Google.Apis.Storage.v1.Data.Object>();
            foreach(var obj in _storage.ListObjects(_bucketName))
            {
                objects.Add(obj);
            }
            return objects;
        }

        public async Task<Stream> DownloadFile(string objectName)
        {
            var stream = new MemoryStream();
            await _storage.DownloadObjectAsync(_bucketName, objectName, stream);
            stream.Position = 0;
            return stream;
        }

        public async Task DeleteFile(string objectName)
        {
            await _storage.DeleteObjectAsync(_bucketName, objectName);
        }
    }
}