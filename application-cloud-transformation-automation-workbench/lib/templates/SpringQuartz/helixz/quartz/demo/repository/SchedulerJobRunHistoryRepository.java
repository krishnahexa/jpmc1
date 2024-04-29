package com.helixz.quartz.demo.repository;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import com.helixz.quartz.demo.enitiy.SchedulerJobRunHistory;

@Repository
public interface SchedulerJobRunHistoryRepository extends JpaRepository<SchedulerJobRunHistory, Long> {
}
