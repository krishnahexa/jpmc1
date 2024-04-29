package com.helixz.quartz.demo;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import com.helixz.quartz.demo.enitiy.SchedulerJobInfo;
import com.helixz.quartz.demo.service.SchedulerService;

import lombok.extern.slf4j.Slf4j;

@Slf4j
@RestController
public class JobActionController {

	@Autowired
	private SchedulerService schedulerService;

	// @GetMapping("/scheduleNewJob")
	public String scheduleNewJob(@RequestParam String jobName, @RequestParam String jobClass,
			@RequestParam String jobGroup, @RequestParam(required = false) String cronExpression,
			@RequestParam Long repeatTime, @RequestParam boolean cronJob) {
		SchedulerJobInfo schedulerJobInfo = new SchedulerJobInfo();
		schedulerJobInfo.setJobName(jobName);
		schedulerJobInfo.setJobClass(jobClass);
		schedulerJobInfo.setJobGroup(jobGroup);
		schedulerJobInfo.setCronExpression(cronExpression != null && !cronExpression.isEmpty() ? cronExpression : null);
		schedulerJobInfo.setRepeatTime(repeatTime);
		schedulerJobInfo.setCronJob(cronJob);
		log.info("Schedule new scheduler jobs starting");
		try {
			schedulerService.scheduleNewJob(schedulerJobInfo);
			log.info("Schedule new scheduler jobs - complete");
		} catch (Exception ex) {
			log.error("Schedule new scheduler jobs - error", ex);
		}
		return "Completed";
	}

//	@GetMapping("/deleteJob")
	public String deleteJob(@RequestParam String jobName, @RequestParam String jobGroup) {
		try {
			SchedulerJobInfo schedulerJobInfo = new SchedulerJobInfo();
			schedulerJobInfo.setJobName(jobName);
			schedulerJobInfo.setJobGroup(jobGroup);
			schedulerService.deleteJob(schedulerJobInfo);
		} catch (Exception e) {
			log.error("Failed to delete job - {}", jobName, e);
		}
		return "Completed";
	}

	@GetMapping("/updateScheduleJob")
	public String updateScheduleJob(@RequestParam String jobName, @RequestParam(required = false) String cronExpression,
			@RequestParam(required = false) Long repeatTime) {
		if (cronExpression != null || repeatTime != null) {
			SchedulerJobInfo schedulerJobInfo = new SchedulerJobInfo();
			schedulerJobInfo.setJobName(jobName);
			schedulerJobInfo.setCronExpression(cronExpression);
			schedulerJobInfo.setRepeatTime(repeatTime);
			schedulerService.updateScheduleJob(schedulerJobInfo);
		} else {
			return "Either cronExpression or repeatTime provided";
		}

		return "Completed";
	}

	@GetMapping("/unScheduleJob")
	public String unScheduleJob(@RequestParam String jobName) {
		try {
			schedulerService.unScheduleJob(jobName);
		} catch (Exception e) {
			log.error("Failed to un-schedule job - {}", jobName, e);
		}
		return "Completed";
	}

	@GetMapping("/scheduleJob")
	public String scheduleJob(@RequestParam String jobName) {
		try {
			schedulerService.scheduleJob(jobName);
		} catch (Exception e) {
			log.error("Failed to un-schedule job - {}", jobName, e);
		}
		return "Completed";
	}

	@GetMapping("/pauseJob")
	public String pauseJob(@RequestParam String jobName, @RequestParam String jobGroup) {
		try {
			SchedulerJobInfo schedulerJobInfo = new SchedulerJobInfo();
			schedulerJobInfo.setJobName(jobName);
			schedulerJobInfo.setJobGroup(jobGroup);
			schedulerService.pauseJob(schedulerJobInfo);
		} catch (Exception e) {
			log.error("Failed to pause job - {}", jobName, e);
		}
		return "Completed";
	}

	@GetMapping("/pauseAllJob")
	public String pauseJob() {
		try {

			schedulerService.pauseAllJob();
		} catch (Exception e) {
			log.error("Failed to pause job - {}", e);
		}
		return "Completed";
	}

	@GetMapping("/resumeJob")
	public String resumeJob(@RequestParam String jobName, @RequestParam String jobGroup) {
		try {
			SchedulerJobInfo schedulerJobInfo = new SchedulerJobInfo();
			schedulerJobInfo.setJobName(jobName);
			schedulerJobInfo.setJobGroup(jobGroup);
			schedulerService.resumeJob(schedulerJobInfo);
		} catch (Exception e) {
			log.error("Failed to resume job - {}", jobName, e);
		}
		return "Completed";
	}

	@GetMapping("/resumeAllJob")
	public String resumeJob() {
		try {

			schedulerService.resumeAllJob();
		} catch (Exception e) {
			log.error("Failed to resume job - {}", e);
		}
		return "Completed";
	}

	@GetMapping("/startJobNow")
	public String startJobNow(@RequestParam String jobName, @RequestParam String jobGroup) {
		try {
			SchedulerJobInfo schedulerJobInfo = new SchedulerJobInfo();
			schedulerJobInfo.setJobName(jobName);
			schedulerJobInfo.setJobGroup(jobGroup);
			schedulerService.startJobNow(schedulerJobInfo);
		} catch (Exception e) {
			log.error("Failed to start new job - {}", jobName, e);
		}
		return "Completed";
	}
}
