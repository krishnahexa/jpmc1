package com.helixz.quartz.demo.service;

import com.helixz.quartz.demo.enitiy.SchedulerJobInfo;

public interface SchedulerService {

	void startAllSchedulers();

	void scheduleNewJob(SchedulerJobInfo jobInfo);

	void updateScheduleJob(SchedulerJobInfo jobInfo);

	boolean unScheduleJob(String jobName);
	boolean scheduleJob(String jobName);
	boolean deleteJob(SchedulerJobInfo jobInfo);

	boolean pauseJob(SchedulerJobInfo jobInfo);

	boolean resumeJob(SchedulerJobInfo jobInfo);

	boolean startJobNow(SchedulerJobInfo jobInfo);

	boolean pauseAllJob();

	boolean resumeAllJob();
}
