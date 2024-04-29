package com.helixz.quartz.security;

import lombok.Builder;
import lombok.Data;

@Data
@Builder
public class ErrorDetails {
	private int errorCode;
	private String message;
	private String timeStamp;
	private String details;
}
