package $PACKAGE_NAME$;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Configuration;
import org.springframework.web.servlet.config.annotation.InterceptorRegistry;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;

@Configuration
public class ApplicationConfigInterceptor implements WebMvcConfigurer {

	@Autowired
	ContextInterceptor contextInterceptor;

	@Override
	public void addInterceptors(InterceptorRegistry registry) {
		// TODO Auto-generated method stub
		registry.addInterceptor(contextInterceptor);
		WebMvcConfigurer.super.addInterceptors(registry);
	}
}
