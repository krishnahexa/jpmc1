package com.hex.batch;

import javax.sql.DataSource;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.batch.core.Job;
import org.springframework.batch.core.Step;
import org.springframework.batch.core.configuration.annotation.EnableBatchProcessing;
import org.springframework.batch.core.configuration.annotation.JobBuilderFactory;
import org.springframework.batch.core.configuration.annotation.StepBuilderFactory;
import org.springframework.batch.core.launch.support.RunIdIncrementer;
import org.springframework.batch.item.database.JdbcCursorItemReader;
import org.springframework.batch.item.file.FlatFileItemReader;
import org.springframework.batch.item.file.FlatFileItemWriter;
import org.springframework.batch.item.file.builder.FlatFileItemReaderBuilder;
import org.springframework.batch.item.file.builder.FlatFileItemWriterBuilder;
import org.springframework.batch.item.file.mapping.BeanWrapperFieldSetMapper;
import org.springframework.batch.item.file.transform.BeanWrapperFieldExtractor;
import org.springframework.batch.item.file.transform.DelimitedLineAggregator;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.core.io.ClassPathResource;
import org.springframework.core.io.FileSystemResource;
import org.springframework.jdbc.core.BeanPropertyRowMapper;
import org.springframework.jdbc.datasource.DriverManagerDataSource;


@Configuration
@EnableBatchProcessing
public class BatchConfiguration {

	private static final Logger log = LoggerFactory.getLogger(BatchConfiguration.class);
	
    @Autowired
    public JobBuilderFactory jobBuilderFactory;

    @Autowired
  	public StepBuilderFactory stepBuilderFactory;

    String csvFileNameIn= "sample-data-in.csv";
    String csvFileWithPathOut = "D:/Amaze/Project/sample-data-out.csv";

      
    BatchConfiguration(){    	  
    }

	public DataSource dataSource() {	
		final DriverManagerDataSource dataSource = new DriverManagerDataSource();
		dataSource.setDriverClassName(prop.getProperty("datasource.driver-class-name")); 
		dataSource.setUrl(prop.getProperty("datasource.url")); 
		dataSource.setUsername(prop.getProperty("datasource.username"));  
		dataSource.setPassword(prop.getProperty("datasource.password"));  
	 
		return dataSource;
	} 
   
  @Bean
  public JdbcCursorItemReader dbReader() {
	  DataSource dataSource = dataSource();
	  StringBuilder sbld = new  StringBuilder("<sql query>");   
      JdbcCursorItemReader<MyInfo> jbcCursorItemReader = new JdbcCursorItemReader<>();
      jbcCursorItemReader.setSql(sbld.toString());
      jbcCursorItemReader.setDataSource(dataSource);
      jbcCursorItemReader.setRowMapper(new BeanPropertyRowMapper<>(MyInfo.class));
      return jbcCursorItemReader;
  }			
  			//     or
  @Bean
  public FlatFileItemReader<MyInfo> flatFileReader() {
    return new FlatFileItemReaderBuilder<MyInfo>()
      .name("flatFileReader")
      .resource(new ClassPathResource(csvFileNameIn))
      .delimited()
      .names(new String[]{"firstName", "lastName"})
      .fieldSetMapper(new BeanWrapperFieldSetMapper<MyInfo>() {{
        setTargetType(MyInfo.class);
      }})
      .build();
  }

  @Bean
  public MyInfoItemProcessor processor() {		//   remove if not required
    return new MyInfoItemProcessor();
  }

  @Bean
  public JdbcBatchItemWriter<Person> dbWriter(DataSource dataSource) {
    return new JdbcBatchItemWriterBuilder<MyInfo>()
      .itemSqlParameterSourceProvider(new BeanPropertyItemSqlParameterSourceProvider<>())
      .sql("<sql query")
      .dataSource(dataSource)
      .build();
  } 
  		//  or
	@Bean
	public FlatFileItemWriter flatFileItemWriter() {
		BeanWrapperFieldExtractor<MyInfo> fieldExtractor = new BeanWrapperFieldExtractor<>();
		fieldExtractor.setNames(new String[] {<field_names as in pojo with comma separated like "firstName","lastName">});
		fieldExtractor.afterPropertiesSet();
	
		DelimitedLineAggregator<MyInfo> lineAggregator = new DelimitedLineAggregator<>();
		lineAggregator.setDelimiter(",");
		lineAggregator.setFieldExtractor(fieldExtractor);
		
		return new FlatFileItemWriterBuilder<MyInfo>()
				.name("flatFileItemWriter")
				.resource(new FileSystemResource(csvFileWithPathOut))
				//    .append(true)
				.lineAggregator(lineAggregator)
				.build();
	}
	
	@Bean
	public Job myBatchJob(JobCompletionNotificationListener listener, Step step1) {
	  return jobBuilderFactory.get("<myBatchJob>")
	    .incrementer(new RunIdIncrementer())
	    .listener(listener)
	    .flow(step1)
	    .end()
	    .build();
	}

	@Bean
	public Step step1(FlatFileItemWriter<MyInfo> writer) {
	  return stepBuilderFactory.get("step1")
	    .<MyInfo, MyInfo> chunk(10)
	    .reader(<dbReader>())
	    .writer(<flatFileItemWriter>())
	    .build();
	}
	
}





