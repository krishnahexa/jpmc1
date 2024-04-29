package com.helixz.quartz.demo;

import static com.google.common.base.Predicates.and;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

import com.google.common.base.Predicate;

import springfox.documentation.builders.ApiInfoBuilder;
import springfox.documentation.builders.PathSelectors;
import springfox.documentation.service.ApiInfo;
import springfox.documentation.spi.DocumentationType;
import springfox.documentation.spring.web.plugins.Docket;
import springfox.documentation.swagger2.annotations.EnableSwagger2;
@Configuration
@EnableSwagger2
public class SpringFoxConfig {

	@Bean
	public Docket api() {
		return new Docket(DocumentationType.SWAGGER_2).apiInfo(apiInfo()).select().paths(postPaths()).build();
	}

	private Predicate<String> postPaths() {
		return and(PathSelectors.regex("(?!/error).+"), PathSelectors.regex("(?!/actuator).+"));
	}

	private ApiInfo apiInfo() {
		return new ApiInfoBuilder().title("DealFactory Batch-Admin Console").description("Deal factory Admin console to control the jobs scheduler")
				.version("1.0.0").build();
	}
}
