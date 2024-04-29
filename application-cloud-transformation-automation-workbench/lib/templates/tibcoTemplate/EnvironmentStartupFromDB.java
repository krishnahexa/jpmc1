package $PACKAGE_NAME$.config;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.util.HashMap;
import java.util.Map;

import javax.sql.DataSource;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.env.EnvironmentPostProcessor;
import org.springframework.boot.jdbc.DataSourceBuilder;
import org.springframework.core.env.ConfigurableEnvironment;
import org.springframework.core.env.MapPropertySource;
import org.springframework.stereotype.Component;

@Component
public class EnvironmentStartupFromDB implements EnvironmentPostProcessor {

	private final Logger logger = LoggerFactory.getLogger(EnvironmentStartupFromDB.class);

	private static final String PROPERTY_SOURCE_NAME = "DBProperties";

	private String[] listOfKeys = { "spring.mail.host", "spring.mail.port", "spring.mail.username",
			"spring.mail.password", "spring.mail.properties.mail.smtp.auth",
			"spring.mail.properties.mail.smtp.connectiontimeout", "spring.mail.properties.mail.smtp.timeout",
			"spring.mail.properties.mail.smtp.writetimeout", "spring.mail.properties.mail.smtp.starttls.enable",
			"ftp.username", "ftp.password", "ftp.host", "ftp.port" };

	@Override
	public void postProcessEnvironment(ConfigurableEnvironment environment, SpringApplication application) {
		Map<String, Object> propertySource = new HashMap<>();
		try {
			DataSource dataSource = DataSourceBuilder.create()
					.username(environment.getProperty("spring.datasource.username"))
					.password(environment.getProperty("spring.datasource.password"))
					.url(environment.getProperty("spring.datasource.url"))
					.driverClassName(environment.getProperty("spring.datasource.driverClassName")).build();

			Connection connection = dataSource.getConnection();
			PreparedStatement preparedStatement = connection
					.prepareStatement("SELECT value FROM DBConfig WHERE id = ?");
			for (int i = 0; i < listOfKeys.length; i++) {
				String key = listOfKeys[i];
				preparedStatement.setString(1, key);
				System.out.println("-------------------------------->>" + key);
				ResultSet rs = preparedStatement.executeQuery();
				while (rs.next()) {
					propertySource.put(key, rs.getString("value"));
				}
				rs.close();
				preparedStatement.clearParameters();
			}
			preparedStatement.close();
			connection.close();
			environment.getPropertySources().addFirst(new MapPropertySource(PROPERTY_SOURCE_NAME, propertySource));
		} catch (Throwable e) {
			logger.error(e.getMessage());
		}
	}
}