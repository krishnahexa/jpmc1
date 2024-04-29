package $PACKAGE_NAME$.config;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.ws.client.core.support.WebServiceGatewaySupport;

public class SOAPConnector extends WebServiceGatewaySupport {

	private static final Logger logger = LoggerFactory.getLogger(SOAPConnector.class);

	public <T> T getCountry(String country) {
		logger.info(" Requesting information for " + country);

		@SuppressWarnings("unchecked")
		T response = (T) getWebServiceTemplate().marshalSendAndReceive(country);
		return response;
	}

}