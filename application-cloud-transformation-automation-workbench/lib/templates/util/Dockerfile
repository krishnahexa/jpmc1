FROM tomcat:9.0.62-jdk8-corretto

RUN mkdir /usr/local/tomcat/webapps/AppName

# Copy the additional libs specific to redissson for clustering
COPY ./artifacts/redis/*.jar /usr/local/tomcat/lib/ 

# Remove webapps, that are no longer required
RUN rm -rf /usr/local/tomcat/webapps.dist/docs/ && rm -rf /usr/local/tomcat/webapps.dist/examples/ && rm -rf /usr/local/tomcat/webapps.dist/manager/ && rm -rf /usr/local/tomcat/webapps.dist/host-manager/ && rm -rf /usr/local/tomcat/webapps.dist/ROOT/ && rm -rf /usr/local/tomcat/webapps.dist/

# Setup confd to manage the configuration files for Passport
# refer to sample @ https://github.com/kelseyhightower/confd/blob/master/docs/tomcat-sample.md
ADD https://github.com/kelseyhightower/confd/releases/download/v0.16.0/confd-0.16.0-linux-amd64 /usr/local/bin/confd
RUN chmod +x /usr/local/bin/confd && mkdir -p /etc/confd/conf.d && mkdir -p /etc/confd/templates

# Copy the template resources for the application configuration
COPY ./example.toml /etc/confd/conf.d/
# Copy the templates for the application configuration
COPY ./example.properties.tmpl /etc/confd/templates/

COPY ./bootstrap.sh /usr/local/bin/

RUN chmod 755 /usr/local/bin/bootstrap.sh \
	&& chmod 777 /etc/hosts

# Expose the required ports & enable entrypoint
EXPOSE 8080
ENTRYPOINT ["/usr/local/bin/bootstrap.sh"]