package com.helixz.quartz.demo.repository;

import com.helixz.quartz.demo.enitiy.SchedulerJobInfo;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface SchedulerRepository extends JpaRepository<SchedulerJobInfo, Long> {
public SchedulerJobInfo findByJobName(String jobName);
}
