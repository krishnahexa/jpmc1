package com.helixz.quartz.demo.jobs;

import lombok.extern.slf4j.Slf4j;
import org.quartz.DisallowConcurrentExecution;
import org.quartz.JobExecutionContext;
import org.quartz.JobExecutionException;
import org.springframework.scheduling.quartz.QuartzJobBean;

import com.helixz.quartz.demo.SpringContext;
import com.helixz.quartz.demo.enitiy.SchedulerJobRunHistory;
import com.helixz.quartz.demo.repository.SchedulerJobRunHistoryRepository;

import java.util.stream.IntStream;

@Slf4j
@DisallowConcurrentExecution
public class SampleCronJob extends QuartzJobBean {
	@Override
	protected void executeInternal(JobExecutionContext context) throws JobExecutionException {
		log.info("SampleCronJob Start................");
		SchedulerJobRunHistory schedulerJobRunHistory = new SchedulerJobRunHistory();
		schedulerJobRunHistory.setJobName(context.getJobDetail().getKey().getName());
		schedulerJobRunHistory.setJobGroup(context.getJobDetail().getKey().getGroup());
		try {
			IntStream.range(0, 10).forEach(i -> {
				log.info("Counting - {}", i);
				try {
					Thread.sleep(1000);
				} catch (InterruptedException e) {
					log.error(e.getMessage(), e);
				}
			});
			schedulerJobRunHistory.setCompletionStatus("Success");
		} catch (Exception e) {
			e.printStackTrace();
			schedulerJobRunHistory.setCompletionStatus("Failure");
		}
		SpringContext.getBean(SchedulerJobRunHistoryRepository.class).save(schedulerJobRunHistory);
		log.info("SampleCronJob End................");
	}
}
