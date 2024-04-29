package com.hexaware.s3io.awsutil;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.Properties;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.core.io.ClassPathResource;
import org.springframework.core.io.Resource;
import org.springframework.stereotype.Component;

import com.amazonaws.AmazonServiceException;
import com.amazonaws.SdkClientException;
import com.amazonaws.auth.AWSCredentialsProvider;
import com.amazonaws.auth.AWSStaticCredentialsProvider;
import com.amazonaws.auth.BasicSessionCredentials;
import com.amazonaws.regions.Region;
import com.amazonaws.regions.Regions;
import com.amazonaws.services.s3.AmazonS3;
import com.amazonaws.services.s3.AmazonS3ClientBuilder;
import com.amazonaws.services.s3.Headers;
import com.amazonaws.services.s3.model.CannedAccessControlList;
import com.amazonaws.services.s3.model.DeleteObjectRequest;
import com.amazonaws.services.s3.model.GetObjectRequest;
import com.amazonaws.services.s3.model.ObjectMetadata;
import com.amazonaws.services.s3.model.PutObjectRequest;
import com.amazonaws.services.s3.model.S3Object;
import com.amazonaws.services.s3.model.StorageClass;
import com.amazonaws.services.securitytoken.model.Credentials;

@Component
public class S3FileIOUtil {
    private static final Logger log = LoggerFactory.getLogger(S3FileIOUtil.class);	

	private static Properties prop = null;
    private static String bucketName = null;
    private static AmazonS3 amazonS3 = null;

	private static Properties getApplicationProperties(){
		if (prop == null){
			try{				
				Resource resource = new ClassPathResource("app-aws.properties");
				prop = new Properties();
	 			InputStream inStrm = resource.getInputStream();
	 			prop.load(inStrm);
	 			inStrm.close();
	 		}catch(IOException ex){
	 			log.error("Exception occured in getApplicationProperties() of S3FileIOUtil: " + ex.getMessage());
	 		}
		}
		return prop;
	}

	// load S3 FileIO properties from application properties object & from credentials
    public static void loadIOProperties(Credentials crdntls) {
    	if(bucketName == null || amazonS3 == null){
	    	getApplicationProperties();
	    	
	    	String region = prop.getProperty("aws.region");
			bucketName = prop.getProperty("aws.bucketName");
			
			Region awsRegion = Region.getRegion(Regions.fromName(region));
			
			BasicSessionCredentials awsCredentials = new BasicSessionCredentials(crdntls.getAccessKeyId(), 
							crdntls.getSecretAccessKey(), crdntls.getSessionToken());
			AWSCredentialsProvider awsCredentialsProvider = new AWSStaticCredentialsProvider(awsCredentials);
			amazonS3 = AmazonS3ClientBuilder.standard().withCredentials(awsCredentialsProvider)
	                .withRegion(awsRegion.getName()).build();
    	}
    } 
    
    public static InputStream readFile(String key, Credentials crdntls) {
    	loadIOProperties(crdntls);
    	
        S3Object obj = amazonS3.getObject(new GetObjectRequest(bucketName, key));
        return obj.getObjectContent();
    }

    public static void writeFile(String fileObjKeyName, String content, Credentials crdntls) {
		loadIOProperties(crdntls);
    	
    	try{
	        Boolean enablePublicWriteAccess = true;
	        ObjectMetadata metadata = new ObjectMetadata();
	        metadata.setHeader(Headers.STORAGE_CLASS, StorageClass.Glacier);
	        
	        PutObjectRequest request = new PutObjectRequest(bucketName, fileObjKeyName, new ByteArrayInputStream(content.getBytes()), metadata);
	        if (enablePublicWriteAccess) {
	            request.withCannedAcl(CannedAccessControlList.PublicReadWrite);
	        }
	        amazonS3.putObject(request);
	    }catch(AmazonServiceException e) {
		    // The call was transmitted successfully, but Amazon S3 couldn't process 
		    // it, so it returned an error response.
		    e.printStackTrace();
		}catch(SdkClientException e) {
		    // Amazon S3 couldn't be contacted for a response, or the client
		    // couldn't parse the response from Amazon S3.
		    e.printStackTrace();
		}
    }

    public static void deleteFile(String path, Credentials crdntls) {
    	loadIOProperties(crdntls);
    	
        amazonS3.deleteObject(new DeleteObjectRequest(bucketName, path));
    }
}