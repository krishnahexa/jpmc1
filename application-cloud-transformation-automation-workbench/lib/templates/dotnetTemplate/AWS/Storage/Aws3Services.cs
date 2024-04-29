
using System.IO;
using System.Net;
using Amazon;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System.Threading.Tasks;


public class Aws3Services : IAws3Services
{
	private readonly string _bucketName;
	private readonly IAmazonS3 _awsS3Client;

	public Aws3Services(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken, string region, string bucketName)
	{
		_bucketName = bucketName;
		_awsS3Client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, awsSessionToken, RegionEndpoint.GetBySystemName(region));
	}

 
//Download file from S3

public async Task<byte[]> DownloadFileAsync(string file)
{
	MemoryStream ms = null;

	try
	{
		GetObjectRequest getObjectRequest = new GetObjectRequest
		{
			BucketName = _bucketName,
			Key = file
		};

		using (var response = await _awsS3Client.GetObjectAsync(getObjectRequest))
		{
			if (response.HttpStatusCode == HttpStatusCode.OK)
			{
				using (ms = new MemoryStream())
				{
					await response.ResponseStream.CopyToAsync(ms);
				}
			}
		}

		if (ms is null || ms.ToArray().Length < 1)
			throw new FileNotFoundException(string.Format("The document '{0}' is not found", file));

		return ms.ToArray();
	}
	catch (Exception)
	{
		throw;
	}
}

//Upload file to S3
public async Task<bool> UploadFileAsync(IFormFile file)
{
	try
	{
		using (var newMemoryStream = new MemoryStream())
		{
			file.CopyTo(newMemoryStream);

			var uploadRequest = new TransferUtilityUploadRequest
			{
				InputStream = newMemoryStream,
				Key = file.FileName,
				BucketName = _bucketName,
				ContentType = file.ContentType
			};

			var fileTransferUtility = new TransferUtility((Amazon.S3.IAmazonS3)_awsS3Client);

			await fileTransferUtility.UploadAsync(uploadRequest);

			return true;
		}
	}
	catch (Exception)
	{
		throw;
	}
}


//Delete file in S3
private async Task Deletefile(string fileName, string versionId)
{
	DeleteObjectRequest request = new DeleteObjectRequest
	{
		BucketName = _bucketName,
		Key = fileName
	};

	if (!string.IsNullOrEmpty(versionId))
		request.VersionId = versionId;

	await _awsS3Client.DeleteObjectAsync(request);
}

public bool IsFileExists(string fileName, string versionId)
{
	try
	{
		GetObjectMetadataRequest request = new GetObjectMetadataRequest()
		{
			BucketName = _bucketName,
			Key = fileName,
			VersionId = !string.IsNullOrEmpty(versionId) ? versionId : null
		};

		var response = _awsS3Client.GetObjectMetadataAsync(request).Result;
        

		return true;
	}
	catch (Exception ex)
	{
		if (ex.InnerException != null && ex.InnerException is AmazonS3Exception awsEx)
		{
			if (string.Equals(awsEx.ErrorCode, "NoSuchBucket"))
				return false;

			else if (string.Equals(awsEx.ErrorCode, "NotFound"))
				return false;
		}

		throw;
	}
}

    Task<bool> IAws3Services.DeleteFileAsync(string fileName, string versionId)
    {
        throw new NotImplementedException();
    }
}



