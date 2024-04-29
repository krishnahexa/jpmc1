package com.spring.flyway;

import org.flywaydb.core.api.callback.Callback;
import org.flywaydb.core.api.callback.Context;
import org.flywaydb.core.api.callback.Event;
import org.springframework.stereotype.Component;

import lombok.extern.slf4j.Slf4j;

@Slf4j
@Component
public class MyNotifierCallback implements Callback {
	@Override
	public boolean supports(Event event, Context context) {
		return event.equals(Event.AFTER_MIGRATE) || event.equals(Event.AFTER_MIGRATE_ERROR);
	}

	@Override
	public boolean canHandleInTransaction(Event event, Context context) {
		return true;
	}

	@Override
	public void handle(Event event, Context context) {
		String notification = event.equals(Event.AFTER_MIGRATE) ? "Success" : "Failed";
		log.info("FlywayStatus:: {}", notification);
	}

}
