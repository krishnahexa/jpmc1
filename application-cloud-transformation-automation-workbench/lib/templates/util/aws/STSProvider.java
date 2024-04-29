package com.security.service.awsutil;

import java.io.IOException;
import java.io.InputStream;
import java.util.Properties;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.Primary;
import org.springframework.core.io.ClassPathResource;
import org.springframework.core.io.Resource;

import com.amazonaws.auth.profile.ProfileCredentialsProvider;
import com.amazonaws.services.securitytoken.AWSSecurityTokenService;
import com.amazonaws.services.securitytoken.AWSSecurityTokenServiceClientBuilder;
import com.amazonaws.services.securitytoken.model.AssumeRoleRequest;
import com.amazonaws.services.securitytoken.model.AssumeRoleResult;
import com.amazonaws.services.securitytoken.model.Credentials;


@Configuration
public class STSProvider {
    private static final Logger log = LoggerFactory.getLogger(STSProvider.class);	

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
	 			log.error("Exception occured in getApplicationProperties() of STSProvider: " + ex.getMessage());
	 		}
		}
		return prop;
	}
    
	// get credentials from Security Token
    @Primary
    public static Credentials getCredentialsFromSecurityToken() throws Exception {
    	getApplicationProperties();
		String roleARN = prop.getProperty("aws.role.arn");
		String roleSessionName = prop.getProperty("aws.role.session.name");

    	AWSSecurityTokenService stsClient = AWSSecurityTokenServiceClientBuilder.standard()
    			.withCredentials(new ProfileCredentialsProvider())
    			.build();
    	
	    AssumeRoleRequest roleRequest = new AssumeRoleRequest()
                .withRoleArn(roleARN)
                .withRoleSessionName(roleSessionName);
	    AssumeRoleResult roleResponse = stsClient.assumeRole(roleRequest);
	    Credentials sessionCredentials = roleResponse.getCredentials();
	    
    	System.out.println("credentials: " + sessionCredentials.toString());			
		return sessionCredentials;
    }   
  
}    
