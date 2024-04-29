package com.hex.amaze3.engine.core;

@Configuration
@EnableWebSecurity
public class SecurityConfig extends WebSecurityConfigurerAdapter {
 
    @Autowired
    public void configureGlobal(AuthenticationManagerBuilder auth) throws Exception {
        auth
          .inMemoryAuthentication()
          .withUser("admin").password("password").roles("ADMIN");
    }
 
    @Override
    protected void configure(HttpSecurity http) throws Exception {
        http
          .httpBasic().and()
          .authorizeRequests()
          .antMatchers("/").hasRole("ADMIN")
          .anyRequest().authenticated();
    }
}
