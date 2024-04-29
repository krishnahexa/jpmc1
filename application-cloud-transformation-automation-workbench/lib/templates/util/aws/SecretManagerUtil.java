package com.hexaware.secret.awsutil;

import java.io.IOException;
import java.io.InputStream;
import java.util.Base64;
import java.util.Properties;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.Primary;
import org.springframework.core.io.ClassPathResource;
import org.springframework.core.io.Resource;

import com.amazonaws.ClientConfiguration;
import com.amazonaws.Protocol;
import com.amazonaws.services.secretsmanager.AWSSecretsManager;
import com.amazonaws.services.secretsmanager.AWSSecretsManagerClientBuilder;
import com.amazonaws.services.secretsmanager.model.GetSecretValueRequest;
import com.amazonaws.services.secretsmanager.model.GetSecretValueResult;
import com.amazonaws.services.secretsmanager.model.InvalidParameterException;
import com.amazonaws.services.secretsmanager.model.InvalidRequestException;
import com.amazonaws.services.secretsmanager.model.ResourceNotFoundException;
import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;

@Configuration
public class SecretManagerUtil {
    private static final Logger log = LoggerFactory.getLogger(SecretManagerUtil.class);	

	private static Properties prop = null;
	
	private static Properties getApplicationProperties(){
		if (prop == null){
			try{				
				Resource resource = new ClassPathResource("app-aws.properties");
				prop = new Properties();
	 			InputStream inStrm = resource.getInputStream();
	 			prop.load(inStrm);
	 			inStrm.close();
	 		}catch(IOException ex){
	 			log.error("Exception occured in getApplicationProperties() of SecretManagerUtil: " + ex.getMessage());
	 		}
		}
		return prop;
	}
    
	// get credentials from secretmanager using only the secret name - uses property file
    @Primary
    public static JsonNode getCredentialsFromSecretManagerUsingProp() throws Exception {
    	getApplicationProperties();
    	
    	String secretName = prop.getProperty("aws.secretsmanager.secretName");

    	JsonNode secretsJson = getCredentialsFromSecretManager(secretName, null);
		
		return secretsJson;
    }   

	// get credentials from secretmanager using the secret name and Region - uses properties file
    @Primary
    public static JsonNode getCredentialsFromSecretManagerWithRegionUsingProp() throws Exception {
    	getApplicationProperties();
    	
    	String secretName = prop.getProperty("aws.secretsmanager.secretName");
		String region = prop.getProperty("aws.region");

		JsonNode secretsJson = getCredentialsFromSecretManager(secretName, region);
		
		return secretsJson;
    }      
    
   // get credentials from secretmanager using the secret name and Region
    public static JsonNode getCredentialsFromSecretManager(String secretName, String region) throws Exception {		
   		ClientConfiguration clientConfig = new ClientConfiguration();
   		clientConfig.setProtocol(Protocol.HTTPS);

		AWSSecretsManagerClientBuilder clientBuilder = AWSSecretsManagerClientBuilder.standard();
		clientBuilder.setClientConfiguration(clientConfig);
		if(region != null)
			clientBuilder.setRegion(region);
		AWSSecretsManager client = clientBuilder.build();

		GetSecretValueRequest getSecretValueRequest = new GetSecretValueRequest().withSecretId(secretName);
		GetSecretValueResult getSecretValueResponse = null;
		try {
			getSecretValueResponse = client.getSecretValue(getSecretValueRequest);
		} catch(ResourceNotFoundException e) {
			log.error("The requested secret " + secretName + " was not found");
			throw e;
		} catch (InvalidRequestException e) {
			log.error("The request was invalid due to: " + e.getMessage());
			throw e;
		} catch (InvalidParameterException e) {
			log.error("The request had invalid params: " + e.getMessage());
			throw e;
		}

		if(getSecretValueResponse == null) {
			return null;
		}

		ObjectMapper objectMapper = new ObjectMapper();
 		JsonNode secretsJson = null;
		// Decrypted secret using the associated KMS CMK
		// Depending on whether the secret was a string or binary, one of these fields will be populated
		String secret = getSecretValueResponse.getSecretString();
		if(secret == null) {
			String decodedBinarySecret = new String(Base64.getDecoder().decode(getSecretValueResponse.getSecretBinary()).array());
			log.error("The Secret String returned is null. So binary secret is Decoded");
			try {
	 			secretsJson = objectMapper.readTree(decodedBinarySecret);
	 		} catch (IOException e) {
	 			log.error("Exception while retreiving secret values: " + e.getMessage());
				throw e;
	 		}
		}
		
		try {
 			secretsJson = objectMapper.readTree(secret);
 		} catch (IOException e) {
 			log.error("Exception while retreiving secret values: " + e.getMessage());
			throw e;
 		}
		
		return secretsJson;
    }   
    
    // sample code to get secrets from Json
    public void getSecretsFromJson(JsonNode secretsJson){
		System.out.println("Secrets json - "+secretsJson);
 		String host = secretsJson.get("host").textValue();
 		String port = secretsJson.get("port").textValue();
 		String dbname = secretsJson.get("dbname").textValue();
 		String username = secretsJson.get("username").textValue();
 		String password = secretsJson.get("password").textValue();
 		
 		System.out.println("host: " + host + ", port: " + port + ", dbname: " + dbname + ", username: " + username + ", password: " + password);
     }
    
}    
