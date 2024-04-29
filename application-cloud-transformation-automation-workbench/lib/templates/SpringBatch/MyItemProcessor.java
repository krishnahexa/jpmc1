package com.hex.batch;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import org.springframework.batch.item.ItemProcessor;

public class MyItemProcessor implements ItemProcessor<MyInfo, MyInfo>{
	  private static final Logger log = LoggerFactory.getLogger(PersonItemProcessor.class);

	  @Override
	  public MyInfo process(final MyInfo myinfo) throws Exception {
		//  processing logic

	    log.info("Converting (" + myinfo + ") into (" + transformedMyinfo + ")");

	    return transformedMyinfo;
	  }

}
