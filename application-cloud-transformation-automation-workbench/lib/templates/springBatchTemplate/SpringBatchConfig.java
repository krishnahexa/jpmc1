package com.hex.amaze3.engine.core;

public class SpringBatchConfig {
    
    @Autowired
    private JobBuilderFactory jobs;
 
    @Autowired
    private StepBuilderFactory steps;
 
    @Value("input/record.csv")
    private Resource inputCsv;
 
    @Value("file:xml/output.xml")
    private Resource outputXml;
 
    @Bean
    public ItemReader<Transaction> itemReader()
      throws UnexpectedInputException, ParseException {
        FlatFileItemReader<Transaction> reader = new FlatFileItemReader<Transaction>();
        DelimitedLineTokenizer tokenizer = new DelimitedLineTokenizer();
        String[] tokens = { "username", "userid", "transactiondate", "amount" };
        tokenizer.setNames(tokens);
        reader.setResource(inputCsv);
        DefaultLineMapper<Transaction> lineMapper = 
          new DefaultLineMapper<Transaction>();
        lineMapper.setLineTokenizer(tokenizer);
        lineMapper.setFieldSetMapper(new RecordFieldSetMapper());
        reader.setLineMapper(lineMapper);
        return reader;
    }
 
    @Bean
    public ItemProcessor<Transaction, Transaction> itemProcessor() {
        return new CustomItemProcessor();
    }
 
    @Bean
    public ItemWriter<Transaction> itemWriter(Marshaller marshaller)
      throws MalformedURLException {
        StaxEventItemWriter<Transaction> itemWriter = 
          new StaxEventItemWriter<Transaction>();
        itemWriter.setMarshaller(marshaller);
        itemWriter.setRootTagName("transactionRecord");
        itemWriter.setResource(outputXml);
        return itemWriter;
    }
 
    @Bean
    public Marshaller marshaller() {
        Jaxb2Marshaller marshaller = new Jaxb2Marshaller();
        marshaller.setClassesToBeBound(new Class[] { Transaction.class });
        return marshaller;
    }
 
    @Bean
    protected Step step1(ItemReader<Transaction> reader,
      ItemProcessor<Transaction, Transaction> processor,
      ItemWriter<Transaction> writer) {
        return steps.get("step1").<Transaction, Transaction> chunk(10)
          .reader(reader).processor(processor).writer(writer).build();
    }
 
    @Bean(name = "firstBatchJob")
    public Job job(@Qualifier("step1") Step step1) {
        return jobs.get("firstBatchJob").start(step1).build();
    }
}
