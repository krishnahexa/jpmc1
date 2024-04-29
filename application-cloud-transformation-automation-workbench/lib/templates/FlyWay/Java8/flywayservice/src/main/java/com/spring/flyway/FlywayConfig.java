package com.spring.flyway;

import java.io.PrintWriter;
import java.io.StringWriter;

import javax.annotation.PostConstruct;

import org.flywaydb.core.Flyway;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Configuration;

@Configuration
public class FlywayConfig {
	@Autowired
	private Flyway flyway;
	@Autowired
	private EmailService emailService;
	@PostConstruct
	public void executeFlyway() {
		try {
			flyway.migrate();
			emailService.sendPreConfiguredMail();
		} catch (Exception e) {
			StringWriter sw = new StringWriter();
			PrintWriter pw = new PrintWriter(sw);
			e.printStackTrace(pw);
			String html = Constants.HTML_CONT1 + e.getMessage() + Constants.HTML_CONT2;
			emailService.sendMailWithAttachment(true, html, sw.toString(), Constants.CONTENT_TYPE);
		}
	}
}
