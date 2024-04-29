package com.hex.amaze3.engine.core;

@RestController
public class SessionController {
    @RequestMapping("/")
    public String helloAdmin() {
        return "hello admin";
    }
}
