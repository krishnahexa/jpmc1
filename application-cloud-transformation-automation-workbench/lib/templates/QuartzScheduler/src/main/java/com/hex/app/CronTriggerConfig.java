package com.hex.app;

import org.quartz.CronTrigger;
import org.quartz.JobDetail;
import org.quartz.Scheduler;
import org.quartz.impl.StdSchedulerFactory;

public class CronTriggerConfig 
{
    public static void main( String[] args ) throws Exception
    {
    	
    	JobDetail job = new JobDetail();
    	job.setName("dummyJobName");
    	job.setJobClass(MyJob.class);
    	    	
    	CronTrigger trigger = new CronTrigger();
    	trigger.setName("dummyTriggerName");
    	trigger.setCronExpression("0/30 * * * * ?");
    	
    	//schedule it
    	Scheduler scheduler = new StdSchedulerFactory().getScheduler();
    	scheduler.start();
    	scheduler.scheduleJob(job, trigger);
    
    }
}
