package com.spring.flyway;

import java.util.ArrayList;
import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.core.env.Environment;
import org.springframework.mail.SimpleMailMessage;

@Configuration
public class EmailConfig {
	@Value("${smtp.email.from:null}")
	private String from;
	@Value("${smtp.email.to:null}")
	private String to;
	@Value("${smtp.email.failureSubject:null}")
	private String failureSubject;
	@Value("${smtp.email.successSubject:null}")
	private String successSubject;
	@Value("${spring.application.name:null}")
	private String applicationName;
	@Autowired
	private Environment env;
	@Bean
	public SimpleMailMessage emailTemplate() {
		SimpleMailMessage message = new SimpleMailMessage();
		List<String> mails=new ArrayList<String>();
		for (String email: to.split(";")) {
			mails.add(email);
		}
		message.setTo(mails.toArray(new String[mails.size()]));
	
		message.setFrom(from);
		message.setSubject(
				env.getActiveProfiles()[0].toUpperCase() + ":" + applicationName.toUpperCase() + "::" + successSubject);
		message.setText(successSubject);
		return message;
	}

}