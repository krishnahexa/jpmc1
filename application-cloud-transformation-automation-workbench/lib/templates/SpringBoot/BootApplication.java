package com.hexaware.bootApplication;
 
import java.io.IOException;
import java.io.InputStream;
import java.util.Map;
 
import javax.servlet.ServletRegistration;
 
import org.apache.commons.logging.Log;
import org.apache.commons.logging.LogFactory;
import org.apache.tomcat.util.descriptor.web.ServletDef;
import org.apache.tomcat.util.descriptor.web.WebXml;
import org.apache.tomcat.util.descriptor.web.WebXmlParser;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.EnableAutoConfiguration;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.builder.SpringApplicationBuilder;
import org.springframework.boot.context.properties.EnableConfigurationProperties;
import org.springframework.boot.web.servlet.ServletContextInitializer;
import org.springframework.boot.web.servlet.support.SpringBootServletInitializer;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.context.annotation.Import;
import org.springframework.core.io.ResourceLoader;
import org.xml.sax.InputSource;

 
@ComponentScan(basePackages = {"com.*"})
@SpringBootApplication
//@EnableConfigurationProperties(ConfigFactory.class)
//@Import({DBConfig.class})
@EnableAutoConfiguration
 
public class BootApplication extends SpringBootServletInitializer {
 
	private static Log logger = LogFactory.getLog(BootApplication.class);
    public static void main(String[] args) {
        SpringApplication.run(BootApplication.class,args);
    }
 
    @Override
    protected SpringApplicationBuilder configure(SpringApplicationBuilder application) {
        return application.sources(BootApplication.class);
    }
    @Autowired
	private ResourceLoader resourceLoader;
    @Bean
	public ServletContextInitializer registerPreCompiledJsps() {
		return servletContext -> {
			WebXmlParser parser = new WebXmlParser(false, false, true);
			try(InputStream inputStream = resourceLoader.getResource("/WEB-INF/web.xml").getInputStream()) {
				WebXml webXml = new WebXml();
				boolean success = parser.parseWebXml(new InputSource(inputStream), webXml, false);
				if (!success) {
					logger.error("Error registering precompiled JSPs");
				}
				for (ServletDef def : webXml.getServlets().values()) {
					logger.debug(String.format("Registering precompiled JSP: %s = %s",def.getServletName(), def.getServletClass() ));
					ServletRegistration.Dynamic reg = servletContext.addServlet(def.getServletName(), def.getServletClass());
					reg.setLoadOnStartup(2);
				}
 
				for (Map.Entry<String, String> mapping : webXml.getServletMappings().entrySet()) {
					logger.debug(String.format("Mapping servlet: %s = %s", mapping.getValue(), mapping.getKey() ));
					servletContext.getServletRegistration(mapping.getValue()).addMapping(mapping.getKey());
				}
 
			} catch (IOException e) {
				logger.error("Error registering precompiled JSPs", e);
			}
		};
	}
}