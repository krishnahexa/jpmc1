package com.hexaware.dataSource;

import javax.sql.DataSource;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.Scope;

import com.zaxxer.hikari.HikariConfig;
import com.zaxxer.hikari.HikariDataSource;

@Configuration
public class DataSourceConfig {
	
	private static final Logger logger = LoggerFactory.getLogger(DataSourceConfig.class);


	private static DataSource dataSource;
	
	@Bean(name = "dataSource")
	@Scope("singleton")
	DataSource dataSource(@Value("${dataSource.driverClassName}") String driverClassName,
			@Value("${dataSource.url}") String url, @Value("${dataSource.username}") String username,
			@Value("${dataSource.password}") String password, @Value("${dataSource.poolName}") String poolName,
			@Value("${dataSource.minimumIdle}") String minimumIdle,
			@Value("${dataSource.maxLifetime}") String maxLifetime,
			@Value("${dataSource.maximumPoolSize}") String maximumPoolSize,
			@Value("${dataSource.leakDetectionThreshold}") String leakDetectionThreshold) {
		HikariConfig config = new HikariConfig();
		config.setDriverClassName(driverClassName.trim());
		config.setJdbcUrl(url.trim());
		config.setUsername(username.trim());
		config.setPassword(password.trim());
		config.setPoolName(poolName.trim());
		config.setMinimumIdle(Integer.parseInt(minimumIdle.trim()));
		config.setMaxLifetime(Long.parseLong(maxLifetime.trim()));
		config.setMaximumPoolSize(Integer.parseInt(maximumPoolSize.trim()));
		config.setLeakDetectionThreshold(Long.parseLong(leakDetectionThreshold.trim()));
		dataSource = new HikariDataSource(config);
		return dataSource;
	}
}