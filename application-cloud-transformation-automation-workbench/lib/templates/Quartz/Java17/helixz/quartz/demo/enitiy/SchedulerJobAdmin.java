package com.helixz.quartz.demo.enitiy;

import lombok.Getter;
import lombok.Setter;

import jakarta.persistence.*;

@Getter
@Setter
@Entity
@Table(name = "scheduler_job_admin")
public class SchedulerJobAdmin {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;
    private String eventName;
    private String eventDetails;
    @Column(insertable = false)
    private String triggerdBy;
    @Column(insertable = false)
    private String triggerdDt;
}