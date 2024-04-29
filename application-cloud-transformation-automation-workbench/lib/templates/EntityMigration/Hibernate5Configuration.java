package $PACKAGE_NAME$;

import java.util.Properties;

import javax.sql.DataSource;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.Primary;
import org.springframework.core.io.ClassPathResource;
import org.springframework.core.io.Resource;
import org.springframework.jdbc.datasource.DriverManagerDataSource;
import org.springframework.jmx.export.MBeanExporter;
import org.springframework.jmx.support.MBeanServerFactoryBean;
import org.springframework.orm.hibernate5.HibernateTransactionManager;
import org.springframework.orm.hibernate5.LocalSessionFactoryBean;
import org.springframework.transaction.annotation.EnableTransactionManagement;

@Configuration
@EnableTransactionManagement
public class Hibernate5Configuration {
	
	@Value("${db.driver}")
    private String DRIVER;
 
    @Value("${db.password}")
    private String PASSWORD;
 
    @Value("${db.url}")
    private String URL;
    
    @Value("${db.data-url}")
    private String DATA_URL;
 
    @Value("${db.data-username}")
    private String DATA_USERNAME;
 
    @Value("${db.data-password}")
    private String DATA_PASSWORD;
 
    @Value("${db.username}")
    private String USERNAME;
 
    @Value("${hibernate.dialect}")
    private String DIALECT;
 
    @Value("${hibernate.show_sql}")
    private String SHOW_SQL;
 
    
    @Bean
    public DataSource dataSource5() {
        DriverManagerDataSource dataSource = new DriverManagerDataSource();
        dataSource.setDriverClassName(DRIVER);
        dataSource.setUrl(DATA_URL);
        dataSource.setUsername(DATA_USERNAME);
        dataSource.setPassword(DATA_PASSWORD);
        return dataSource;
    }
    
    @Bean
    public DataSource coreDataSource5() {
        DriverManagerDataSource dataSource = new DriverManagerDataSource();
        dataSource.setDriverClassName(DRIVER);
        dataSource.setUrl(URL);
        dataSource.setUsername(USERNAME);
        dataSource.setPassword(PASSWORD);
        return dataSource;
    }
    
    @Bean(name="coreSessionFactory5")
    public LocalSessionFactoryBean sessionFactory5() {
        LocalSessionFactoryBean sessionFactory = new LocalSessionFactoryBean();
        sessionFactory.setDataSource(coreDataSource5());
        Resource resource = new ClassPathResource("hibernate.cfg.xml");
        sessionFactory.setConfigLocation(resource);
        Properties hibernateProperties = new Properties();
        hibernateProperties.put("hibernate.dialect", DIALECT);
        hibernateProperties.put("hibernate.show_sql", SHOW_SQL);
        hibernateProperties.put("hibernate.cache.use_second_level_cache", "false");
        hibernateProperties.put("hibernate.cache.use_query_cache", "false");
        hibernateProperties.put("hibernate.connection.provider_disables_autocommit", "true");
        sessionFactory.setHibernateProperties(hibernateProperties);
        return sessionFactory;
    }
    
    @Bean(name="dataSessionFactory5")
    public LocalSessionFactoryBean dataSessionFactory5() {
        LocalSessionFactoryBean sessionFactory = new LocalSessionFactoryBean();
        sessionFactory.setDataSource(dataSource5());
        Resource resource = new ClassPathResource("hibernate_data.cfg.xml");
        sessionFactory.setConfigLocation(resource);
        Properties hibernateProperties = new Properties();
        hibernateProperties.put("hibernate.dialect", DIALECT);
        hibernateProperties.put("hibernate.show_sql", SHOW_SQL);
        hibernateProperties.put("hibernate.cache.use_second_level_cache", "false");
        hibernateProperties.put("hibernate.cache.use_query_cache", "false");
        hibernateProperties.put("hibernate.connection.provider_disables_autocommit", "true");
        sessionFactory.setHibernateProperties(hibernateProperties);
        return sessionFactory;
    }

    @Primary
    @Bean
    public MBeanServerFactoryBean createMBean() {
    	return new MBeanServerFactoryBean();
    }
    
    @Primary
    @Bean
    public MBeanExporter  createMBeanExporter() {
    	MBeanExporter mbean = new MBeanExporter();
    	mbean.setAutodetect(false);;
    	return mbean;
    }
}
