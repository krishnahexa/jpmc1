package com.helixz.quartz.demo.enitiy;

import java.util.Date;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.Table;

import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
@Entity
@Table(name = "scheduler_job_run_history")
public class SchedulerJobRunHistory {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;
    private String jobName;
    private String jobGroup;
    private String completionStatus;
    @Column(insertable = false)
    private String triggerdBy;
    @Column(insertable = false)
    private Date triggerdDt;
}