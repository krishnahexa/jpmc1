package com.hexaware.azureutils;

import com.azure.storage.blob.BlobClient;
import com.azure.storage.blob.BlobServiceClientBuilder;

import java.io.File;
import java.nio.file.Paths;

public class AzureFileIOUtility {

	// Azure Storage Blob credentials
	private static final String azureConnectionString = "<your-azure-storage-connection-string>";

	private static BlobClient getBlobClient(String containerName, String fileName) {
		return new BlobServiceClientBuilder().connectionString(azureConnectionString).buildClient()
				.getBlobContainerClient(containerName).getBlobClient(fileName);
	}

	// Azure Storage Blob methods
	public static void uploadToAzureStorageBlob(String containerName, String fileName, File file) {
		BlobClient blobClient = getBlobClient(containerName, fileName);
		blobClient.uploadFromFile(file.toPath().toString());
	}

	public static void downloadFromAzureStorageBlob(String containerName, String fileName, String destinationPath) {
		BlobClient blobClient = getBlobClient(containerName, fileName);
		blobClient.downloadToFile(destinationPath);
	}

	public static void deleteFromAzureStorageBlob(String containerName, String fileName) {
		BlobClient blobClient = getBlobClient(containerName, fileName);
		blobClient.delete();
	}

	public static void moveWithinAzureStorageBlob(String sourceContainerName, String sourceFileName,
			String destinationContainerName, String destinationFileName) {
		BlobClient sourceBlobClient = getBlobClient(sourceContainerName, sourceFileName);
		BlobClient destinationBlobClient = getBlobClient(destinationContainerName, destinationFileName);

		//sourceBlobClient.copyTo(destinationBlobClient.getBlobUrl());
		sourceBlobClient.delete();
	}

	public static void main(String[] args) {
		// Example usage
		String azureContainerName = "<your-azure-container-name>";
		String azureFileName = "example.txt";
		File azureFile = new File("path/to/local/file.txt");
		uploadToAzureStorageBlob(azureContainerName, azureFileName, azureFile);
	}
}
