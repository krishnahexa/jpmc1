package com.helixz.quartz.demo;

import java.util.TimeZone;

import jakarta.annotation.PostConstruct;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;

@SpringBootApplication
public class QuartzDemoApplication {

    public static void main(String[] args) {
        SpringApplication.run(QuartzDemoApplication.class, args);
    }
    @PostConstruct
    public void init(){
      TimeZone.setDefault(TimeZone.getTimeZone("America/New_York"));
    }
}
