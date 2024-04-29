package com.hex.amaze3.engine.core;

import static org.junit.Assert.*;
import org.springframework.data.redis.cache.RedisCacheManager;
import org.springframework.data.redis.connection.jedis.JedisConnectionFactory;
import org.springframework.data.redis.core.RedisTemplate;

import java.util.Set;

import org.junit.Test;

public class SessionControllerTest{
	
	private Jedis jedis;
    /*private TestRestTemplate testRestTemplate;
    private TestRestTemplate testRestTemplateWithAuth;*/
    private String testUrl = "http://localhost:8080/";

	@Test
	public void testRedisIsEmpty() {
	    Set<String> result = jedis.keys("*");
	    assertEquals(0, result.size());
	}
	
	@Test
	public void testUnauthenticatedCantAccess() {
	    ResponseEntity<String> result = testRestTemplate.getForEntity(testUrl, String.class);
	    assertEquals(HttpStatus.UNAUTHORIZED, result.getStatusCode());
	}
	
	@Test
	public void testRedisControlsSession() {
	    ResponseEntity<String> result = testRestTemplateWithAuth.getForEntity(testUrl, String.class);
	    assertEquals("hello admin", result.getBody()); //login worked
	 
	    Set<String> redisResult = jedis.keys("*");
	    assertTrue(redisResult.size() > 0); //redis is populated with session data
	 
	    String sessionCookie = result.getHeaders().get("Set-Cookie").get(0).split(";")[0];
	    HttpHeaders headers = new HttpHeaders();
	    headers.add("Cookie", sessionCookie);
	    HttpEntity<String> httpEntity = new HttpEntity<>(headers);
	 
	    result = testRestTemplate.exchange(testUrl, HttpMethod.GET, httpEntity, String.class);
	    assertEquals("hello admin", result.getBody()); //access with session works worked
	 
	    jedis.flushAll(); //clear all keys in redis
	 
	    result = testRestTemplate.exchange(testUrl, HttpMethod.GET, httpEntity, String.class);
	    assertEquals(HttpStatus.UNAUTHORIZED, result.getStatusCode());
	    //access denied after sessions are removed in redis
	}

}
