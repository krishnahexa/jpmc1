package com.helixz.quartz.demo.service.impl;

import java.util.Date;
import java.util.List;

import org.quartz.CronExpression;
import org.quartz.JobBuilder;
import org.quartz.JobDetail;
import org.quartz.JobKey;
import org.quartz.Scheduler;
import org.quartz.SchedulerException;
import org.quartz.SimpleTrigger;
import org.quartz.Trigger;
import org.quartz.TriggerKey;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.ApplicationContext;
import org.springframework.scheduling.quartz.QuartzJobBean;
import org.springframework.scheduling.quartz.SchedulerFactoryBean;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.helixz.quartz.demo.component.JobScheduleCreator;
import com.helixz.quartz.demo.enitiy.SchedulerJobAdmin;
import com.helixz.quartz.demo.enitiy.SchedulerJobInfo;
import com.helixz.quartz.demo.repository.SchedulerJobAdminRepository;
import com.helixz.quartz.demo.repository.SchedulerRepository;
import com.helixz.quartz.demo.service.SchedulerService;

import lombok.extern.slf4j.Slf4j;

@Slf4j
@Transactional
@Service
public class SchedulerServiceImpl implements SchedulerService {

	@Autowired
	private SchedulerFactoryBean schedulerFactoryBean;

	@Autowired
	private SchedulerRepository schedulerRepository;
	@Autowired
	private SchedulerJobAdminRepository schedulerJobAdminRepository;
	@Autowired
	private ApplicationContext context;

	@Autowired
	private JobScheduleCreator scheduleCreator;

	@Override
	public void startAllSchedulers() {
		List<SchedulerJobInfo> jobInfoList = schedulerRepository.findAll();
		if (jobInfoList != null) {
			Scheduler scheduler = schedulerFactoryBean.getScheduler();
			jobInfoList.forEach(jobInfo -> {
				try {
					JobDetail jobDetail = JobBuilder
							.newJob((Class<? extends QuartzJobBean>) Class.forName(jobInfo.getJobClass()))
							.withIdentity(jobInfo.getJobName(), jobInfo.getJobGroup()).build();
					if (!scheduler.checkExists(jobDetail.getKey())) {
						Trigger trigger;
						jobDetail = scheduleCreator.createJob(
								(Class<? extends QuartzJobBean>) Class.forName(jobInfo.getJobClass()), false, context,
								jobInfo.getJobName(), jobInfo.getJobGroup());

						if (jobInfo.getCronJob() && CronExpression.isValidExpression(jobInfo.getCronExpression())) {
							trigger = scheduleCreator.createCronTrigger(jobInfo.getJobName(), new Date(),
									jobInfo.getCronExpression(), SimpleTrigger.MISFIRE_INSTRUCTION_FIRE_NOW);
						} else {
							trigger = scheduleCreator.createSimpleTrigger(jobInfo.getJobName(), new Date(),
									jobInfo.getRepeatTime(), SimpleTrigger.MISFIRE_INSTRUCTION_FIRE_NOW);
						}

						scheduler.scheduleJob(jobDetail, trigger);

					}
				} catch (ClassNotFoundException e) {
					log.error("Class Not Found - {}", jobInfo.getJobClass(), e);
				} catch (SchedulerException e) {
					log.error(e.getMessage(), e);
				}
			});
		}
	}

	@Override
	public void scheduleNewJob(SchedulerJobInfo jobInfo) {
		try {
			Scheduler scheduler = schedulerFactoryBean.getScheduler();

			JobDetail jobDetail = JobBuilder
					.newJob((Class<? extends QuartzJobBean>) Class.forName(jobInfo.getJobClass()))
					.withIdentity(jobInfo.getJobName(), jobInfo.getJobGroup()).build();
			if (!scheduler.checkExists(jobDetail.getKey())) {

				jobDetail = scheduleCreator.createJob(
						(Class<? extends QuartzJobBean>) Class.forName(jobInfo.getJobClass()), false, context,
						jobInfo.getJobName(), jobInfo.getJobGroup());

				Trigger trigger;
				if (jobInfo.getCronJob()) {
					trigger = scheduleCreator.createCronTrigger(jobInfo.getJobName(), new Date(),
							jobInfo.getCronExpression(), SimpleTrigger.MISFIRE_INSTRUCTION_FIRE_NOW);
				} else {
					trigger = scheduleCreator.createSimpleTrigger(jobInfo.getJobName(), new Date(),
							jobInfo.getRepeatTime(), SimpleTrigger.MISFIRE_INSTRUCTION_FIRE_NOW);
				}

				scheduler.scheduleJob(jobDetail, trigger);
				schedulerRepository.save(jobInfo);
			} else {
				log.error("scheduleNewJobRequest.jobAlreadyExist");
			}
		} catch (ClassNotFoundException e) {
			log.error("Class Not Found - {}", jobInfo.getJobClass(), e);
		} catch (SchedulerException e) {
			log.error(e.getMessage(), e);
		}
	}

	@Override
	public void updateScheduleJob(SchedulerJobInfo jobInfoInput) {
		Trigger newTrigger;
		SchedulerJobInfo jobInfo = schedulerRepository.findByJobName(jobInfoInput.getJobName());
		jobInfo.setCronExpression(jobInfoInput.getCronExpression());
		jobInfo.setRepeatTime(jobInfoInput.getRepeatTime());
		if (jobInfo != null) {
			if (jobInfo.getCronJob()) {
				newTrigger = scheduleCreator.createCronTrigger(jobInfo.getJobName(), new Date(),
						jobInfo.getCronExpression(), SimpleTrigger.MISFIRE_INSTRUCTION_FIRE_NOW);
			} else {
				newTrigger = scheduleCreator.createSimpleTrigger(jobInfo.getJobName(), new Date(),
						jobInfo.getRepeatTime(), SimpleTrigger.MISFIRE_INSTRUCTION_FIRE_NOW);
			}
			try {
				schedulerFactoryBean.getScheduler().rescheduleJob(TriggerKey.triggerKey(jobInfo.getJobName()),
						newTrigger);
				schedulerRepository.save(jobInfo);
				SchedulerJobAdmin  schedulerJobAdmin = new SchedulerJobAdmin();
				schedulerJobAdmin.setEventName("updateScheduleJob");
				schedulerJobAdmin.setEventDetails(jobInfo.toString());
				schedulerJobAdmin.setTriggerdBy("vel");
				schedulerJobAdminRepository.save(schedulerJobAdmin);
			} catch (SchedulerException e) {
				log.error(e.getMessage(), e);
			}
		}
	}

	@Override
	public boolean unScheduleJob(String jobName) {
		try {
			 schedulerFactoryBean.getScheduler().unscheduleJob(new TriggerKey(jobName));
				SchedulerJobAdmin  schedulerJobAdmin = new SchedulerJobAdmin();
				schedulerJobAdmin.setEventName("unScheduleJob");
				schedulerJobAdmin.setEventDetails(jobName);
				schedulerJobAdmin.setTriggerdBy("vel");
				schedulerJobAdminRepository.save(schedulerJobAdmin);
			 return true;
		} catch (SchedulerException e) {
			log.error("Failed to un-schedule job - {}", jobName, e);
			return false;
		}
	}

	@Override
	public boolean scheduleJob(String jobName) {
		Scheduler scheduler = schedulerFactoryBean.getScheduler();
		SchedulerJobInfo jobInfo = schedulerRepository.findByJobName(jobName);
			try {
				JobDetail jobDetail = JobBuilder
						.newJob((Class<? extends QuartzJobBean>) Class.forName(jobInfo.getJobClass()))
						.withIdentity(jobInfo.getJobName(), jobInfo.getJobGroup()).build();
				if (!scheduler.checkExists(jobDetail.getKey())) {
					Trigger trigger;
					jobDetail = scheduleCreator.createJob(
							(Class<? extends QuartzJobBean>) Class.forName(jobInfo.getJobClass()), false, context,
							jobInfo.getJobName(), jobInfo.getJobGroup());

					if (jobInfo.getCronJob() && CronExpression.isValidExpression(jobInfo.getCronExpression())) {
						trigger = scheduleCreator.createCronTrigger(jobInfo.getJobName(), new Date(),
								jobInfo.getCronExpression(), SimpleTrigger.MISFIRE_INSTRUCTION_FIRE_NOW);
					} else {
						trigger = scheduleCreator.createSimpleTrigger(jobInfo.getJobName(), new Date(),
								jobInfo.getRepeatTime(), SimpleTrigger.MISFIRE_INSTRUCTION_FIRE_NOW);
					}

					scheduler.scheduleJob(jobDetail, trigger);
					SchedulerJobAdmin  schedulerJobAdmin = new SchedulerJobAdmin();
					schedulerJobAdmin.setEventName("scheduleJob");
					schedulerJobAdmin.setEventDetails(jobInfo.toString());
					schedulerJobAdmin.setTriggerdBy("vel");
					schedulerJobAdminRepository.save(schedulerJobAdmin);
				}
			} catch (ClassNotFoundException e) {
				log.error("Class Not Found - {}", jobInfo.getJobClass(), e);
			} catch (SchedulerException e) {
				log.error(e.getMessage(), e);
			}
			return true;
	}

	@Override
	public boolean deleteJob(SchedulerJobInfo jobInfo) {
		try {
			schedulerRepository.delete(jobInfo);
			return schedulerFactoryBean.getScheduler()
					.deleteJob(new JobKey(jobInfo.getJobName(), jobInfo.getJobGroup()));
		} catch (SchedulerException e) {
			log.error("Failed to delete job - {}", jobInfo.getJobName(), e);
			return false;
		}
	}

	@Override
	public boolean pauseJob(SchedulerJobInfo jobInfo) {
		try {
			schedulerFactoryBean.getScheduler().pauseJob(new JobKey(jobInfo.getJobName(), jobInfo.getJobGroup()));
			SchedulerJobAdmin  schedulerJobAdmin = new SchedulerJobAdmin();
			schedulerJobAdmin.setEventName("pauseJob");
			schedulerJobAdmin.setEventDetails(jobInfo.toString());
			schedulerJobAdmin.setTriggerdBy("vel");
			schedulerJobAdminRepository.save(schedulerJobAdmin);
			return true;
		} catch (SchedulerException e) {
			log.error("Failed to pause job - {}", jobInfo.getJobName(), e);
			return false;
		}
	}

	@Override
	public boolean pauseAllJob() {
		try {
			schedulerFactoryBean.getScheduler().pauseAll();
			SchedulerJobAdmin  schedulerJobAdmin = new SchedulerJobAdmin();
			schedulerJobAdmin.setEventName("pauseAllJob");
			schedulerJobAdmin.setEventDetails("pauseAllJob");
			schedulerJobAdmin.setTriggerdBy("vel");
			schedulerJobAdminRepository.save(schedulerJobAdmin);
		} catch (SchedulerException e) {
			e.printStackTrace();
			return false;
		}
		return true;
	}

	@Override
	public boolean resumeJob(SchedulerJobInfo jobInfo) {
		try {
			schedulerFactoryBean.getScheduler().resumeJob(new JobKey(jobInfo.getJobName(), jobInfo.getJobGroup()));
			SchedulerJobAdmin  schedulerJobAdmin = new SchedulerJobAdmin();
			schedulerJobAdmin.setEventName("resumeJob");
			schedulerJobAdmin.setEventDetails(jobInfo.toString());
			schedulerJobAdmin.setTriggerdBy("vel");
			schedulerJobAdminRepository.save(schedulerJobAdmin);
			return true;
		} catch (SchedulerException e) {
			log.error("Failed to resume job - {}", jobInfo.getJobName(), e);
			return false;
		}
	}

	@Override
	public boolean resumeAllJob() {
		try {
			schedulerFactoryBean.getScheduler().resumeAll();
			SchedulerJobAdmin  schedulerJobAdmin = new SchedulerJobAdmin();
			schedulerJobAdmin.setEventName("resumeAllJob");
			schedulerJobAdmin.setEventDetails("resumeAllJob");
			schedulerJobAdmin.setTriggerdBy("vel");
			schedulerJobAdminRepository.save(schedulerJobAdmin);
		} catch (SchedulerException e) {
			e.printStackTrace();
			return false;
		}
		return true;
	}

	@Override
	public boolean startJobNow(SchedulerJobInfo jobInfo) {
		try {
			schedulerFactoryBean.getScheduler().triggerJob(new JobKey(jobInfo.getJobName(), jobInfo.getJobGroup()));
			SchedulerJobAdmin  schedulerJobAdmin = new SchedulerJobAdmin();
			schedulerJobAdmin.setEventName("resumeJob");
			schedulerJobAdmin.setEventDetails(jobInfo.toString());
			schedulerJobAdmin.setTriggerdBy("vel");
			schedulerJobAdminRepository.save(schedulerJobAdmin);
			return true;
		} catch (SchedulerException e) {
			log.error("Failed to start new job - {}", jobInfo.getJobName(), e);
			return false;
		}
	}
}
