package com.hex.app;

import org.quartz.Job;
import org.quartz.JobExecutionContext;
import org.quartz.JobExecutionException;

public class MyJob implements Job
{
	public void execute(JobExecutionContext context)
	throws JobExecutionException {		
		System.out.println("Monthly Billing Report - Job started");	
		System.out.println("Monthly Billing Report - in progress");	
		System.out.println("Monthly Billing Report - Job completed");
	}
	
}
