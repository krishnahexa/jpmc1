package com.spring.flyway;

import java.io.ByteArrayInputStream;
import java.util.ArrayList;
import java.util.List;

import javax.mail.MessagingException;
import javax.mail.internet.InternetAddress;
import javax.mail.internet.MimeMessage;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.core.env.Environment;
import org.springframework.core.io.InputStreamSource;
import org.springframework.mail.MailException;
import org.springframework.mail.SimpleMailMessage;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.mail.javamail.MimeMessageHelper;
import org.springframework.mail.javamail.MimeMessagePreparator;
import org.springframework.stereotype.Service;

import lombok.extern.slf4j.Slf4j;

@Slf4j
@Service("emailService")
public class EmailService {
	@Value("${smtp.email.from:null}")
	private String from;
	@Value("${smtp.email.to:null}")
	private String to;
	@Value("${smtp.email.failureSubject:null}")
	private String failureSubject;
	@Value("${smtp.email.successSubject:null}")
	private String successSubject;
	@Value("${smtp.email.attachementFileName:null}")
	private String attachementFileName;
	@Value("${spring.application.name:null}")
	private String applicationName;
	@Autowired
	private Environment env;
	@Autowired
	private JavaMailSender mailSender;

	@Autowired
	private SimpleMailMessage preConfiguredMessage;


	public void sendMail(String to, String subject, String body) {
		SimpleMailMessage message = new SimpleMailMessage();
		message.setTo(to);
		message.setSubject(env.getActiveProfiles()[0].toUpperCase() + ":" + applicationName + "::" + subject);
		message.setText(body);
		mailSender.send(message);
	}


	public void sendPreConfiguredMail(String... message) {
		SimpleMailMessage mailMessage = new SimpleMailMessage(preConfiguredMessage);
		mailMessage.setText(message.length > 0 ? message[0].toUpperCase() : "");
		mailSender.send(mailMessage);
	}

	public void sendMailWithAttachment(boolean failureNotification, String messageBody, String attachementContent,
			String contentType) {
		MimeMessagePreparator preparator = new MimeMessagePreparator() {
			public void prepare(MimeMessage mimeMessage) throws Exception {
				try {
					MimeMessageHelper helper = new MimeMessageHelper(mimeMessage, true);
					//helper.addTo(new InternetAddress(to));
					List<String> mails=new ArrayList<String>();
					for (String email: to.split(";")) {
						mails.add(email);
					}
					helper.setTo(mails.toArray(new String[mails.size()]));
					helper.setFrom(from);

					if (failureNotification) {
						helper.setSubject(env.getActiveProfiles()[0].toUpperCase() + ":" + applicationName.toUpperCase()
								+ "::" + failureSubject);
						helper.setText(messageBody, true);
					} else {
						helper.setSubject(env.getActiveProfiles()[0].toUpperCase() + ":" + applicationName + "::"
								+ successSubject);
					}
					if (attachementContent != null) {
						helper.addAttachment(attachementFileName,
								(InputStreamSource) new InputStreamDataSource(contentType, attachementFileName,
										new ByteArrayInputStream(attachementContent.getBytes())));
					}
				} catch (MessagingException e) {
					e.printStackTrace();
				}
			}
		};

		try {
			mailSender.send(preparator);
		} catch (MailException ex) {
			log.error(ex.getMessage());
		}
	}
}
