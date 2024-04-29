package com.hexaware.awsutil;
import software.amazon.awssdk.auth.credentials.AwsBasicCredentials;
import software.amazon.awssdk.auth.credentials.StaticCredentialsProvider;
import software.amazon.awssdk.core.sync.RequestBody;
import software.amazon.awssdk.core.sync.ResponseTransformer;
import software.amazon.awssdk.regions.Region;
import software.amazon.awssdk.services.s3.S3Client;
import software.amazon.awssdk.services.s3.model.*;

import java.io.File;
import java.nio.file.Paths;

public class AWSFileIOUtility {

	// AWS S3 credentials
	private static final String awsRegion = "<your-aws-region>";
	private static final String awsAccessKey = "<your-aws-access-key>";
	private static final String awsSecretKey = "<your-aws-secret-key>";

	// Common methods
	private static S3Client getS3Client() {
		return S3Client.builder().region(Region.of(awsRegion))
				.credentialsProvider(
						StaticCredentialsProvider.create(AwsBasicCredentials.create(awsAccessKey, awsSecretKey)))
				.build();
	}

	// AWS S3 methods
	public static void uploadToS3(String bucketName, String fileName, File file) {
		S3Client s3Client = getS3Client();
		s3Client.putObject(PutObjectRequest.builder().bucket(bucketName).key(fileName).build(),
				RequestBody.fromFile(file));
	}

	public static void downloadFromS3(String bucketName, String fileName, String destinationPath) {
		S3Client s3Client = getS3Client();
		GetObjectRequest getObjectRequest = GetObjectRequest.builder().bucket(bucketName).key(fileName).build();

		s3Client.getObject(getObjectRequest, ResponseTransformer.toFile(Paths.get(destinationPath)));
	}

	public static void deleteFromS3(String bucketName, String fileName) {
		S3Client s3Client = getS3Client();
		DeleteObjectRequest deleteObjectRequest = DeleteObjectRequest.builder().bucket(bucketName).key(fileName)
				.build();

		s3Client.deleteObject(deleteObjectRequest);
	}

	public static void moveWithinS3(String sourceBucketName, String sourceFileName, String destinationBucketName,
			String destinationFileName) {
		S3Client s3Client = getS3Client();
		CopyObjectRequest copyObjectRequest = CopyObjectRequest.builder().sourceBucket(sourceBucketName)
				.sourceKey(sourceFileName).destinationBucket(destinationBucketName).destinationKey(destinationFileName)
				.build();

		s3Client.copyObject(copyObjectRequest);
		DeleteObjectRequest deleteObjectRequest = DeleteObjectRequest.builder().bucket(sourceBucketName)
				.key(sourceFileName).build();

		s3Client.deleteObject(deleteObjectRequest);
	}

	public static void main(String[] args) {
		// Example usage
		String s3BucketName = "<your-s3-bucket-name>";
		String s3FileName = "example.txt";
		File s3File = new File("path/to/local/file.txt");
		uploadToS3(s3BucketName, s3FileName, s3File);
	}
}
