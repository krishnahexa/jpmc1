package com.helixz.quartz.demo.jobs;

import java.util.stream.IntStream;

import org.quartz.JobExecutionContext;
import org.quartz.JobExecutionException;
import org.springframework.scheduling.quartz.QuartzJobBean;

import com.helixz.quartz.demo.SpringContext;
import com.helixz.quartz.demo.enitiy.SchedulerJobRunHistory;
import com.helixz.quartz.demo.repository.SchedulerJobRunHistoryRepository;

import lombok.extern.slf4j.Slf4j;


@Slf4j
public class SimpleJob extends QuartzJobBean {
	
    @Override
    protected void executeInternal(JobExecutionContext context) throws JobExecutionException {
        log.info("SimpleJob Start................");
        SchedulerJobRunHistory schedulerJobRunHistory = new SchedulerJobRunHistory();
        schedulerJobRunHistory.setJobName(context.getJobDetail().getKey().getName());
        schedulerJobRunHistory.setJobGroup(context.getJobDetail().getKey().getGroup());
        
       try {
    	   IntStream.range(0, 5).forEach(i -> {
    
            log.info("Counting - {}", i);
            try {
                Thread.sleep(1000);
            } catch (InterruptedException e) {
                log.error(e.getMessage(), e);
            }
        });
    	   schedulerJobRunHistory.setCompletionStatus("Success");
       }catch(Exception e) {
    	   e.printStackTrace();
    	   schedulerJobRunHistory.setCompletionStatus("Failure");
       }
        SpringContext.getBean(SchedulerJobRunHistoryRepository.class).save(schedulerJobRunHistory);
        log.info("SimpleJob End................");
    }
}
