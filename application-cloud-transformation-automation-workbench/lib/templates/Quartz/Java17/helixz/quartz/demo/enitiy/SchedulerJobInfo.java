package com.helixz.quartz.demo.enitiy;

import lombok.Getter;
import lombok.Setter;

import jakarta.persistence.*;

@Getter
@Setter
@Entity
@Table(name = "scheduler_job_info")
public class SchedulerJobInfo {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String jobName;

    private String jobGroup;

    private String jobClass;

    private String cronExpression;

    private Long repeatTime;

    private Boolean cronJob;

	@Override
	public String toString() {
		return "SchedulerJobInfo [jobName=" + jobName + ", jobGroup=" + jobGroup + ", jobClass=" + jobClass
				+ ", cronExpression=" + cronExpression + ", repeatTime=" + repeatTime + ", cronJob=" + cronJob + "]";
	}
    
}