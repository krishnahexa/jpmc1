package com.hex.getstatementhitlist_jms.spring.integration.utils;

import org.w3c.dom.Node;

import javax.xml.transform.OutputKeys;
import javax.xml.transform.Result;
import javax.xml.transform.Source;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerException;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.dom.DOMResult;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;
import javax.xml.transform.stream.StreamSource;
import java.io.ByteArrayInputStream;
import java.io.InputStream;
import java.io.OutputStream;
import java.io.StringWriter;
import java.io.UnsupportedEncodingException;
import java.util.HashMap;
import java.util.Map;
import java.util.Properties;

public final class XsltUtil {

	public static final String DEFAULT_ENCODING = "UTF-8";

	private static Map<String, String> defaultTransformProperties = new HashMap<String, String>();
	static {
		defaultTransformProperties.put(OutputKeys.ENCODING, DEFAULT_ENCODING);
		defaultTransformProperties.put(OutputKeys.OMIT_XML_DECLARATION, "yes");
	}

	public static Source getSource(Object xml) {
		if (xml == null)
			return null;

		Source xmlSource;
		if (xml instanceof InputStream) {
			xmlSource = new StreamSource((InputStream) xml);
		} else if (xml instanceof String) {
			try {
				InputStream is = new ByteArrayInputStream(((String) xml).getBytes(DEFAULT_ENCODING));
				xmlSource = new StreamSource(is);
			} catch (UnsupportedEncodingException e) {
				throw new RuntimeException(e);
			}
		} else if (xml instanceof Node) {
			xmlSource = new DOMSource((Node) xml);
		} else {
			throw new IllegalArgumentException(String.format("Type not supported: %s", xml.getClass().getName()));
		}

		return xmlSource;
	}

	public static Result getResult(Object result) {
		if (result == null)
			return null;

		Result xmlResult = null;
		if (result instanceof OutputStream) {
			xmlResult = new StreamResult((OutputStream) result);
		} else if (result instanceof Node) {
			xmlResult = new DOMResult((Node) result);
		} else {
			throw new IllegalArgumentException("Type not supported");
		}

		return xmlResult;
	}

	private static void setTransformerProperties(Transformer transformer, Map<String, String> properties) {
		for (Map.Entry<String, String> entry : properties.entrySet()) {
			transformer.setOutputProperty(entry.getKey(), entry.getValue());
		}
	}

	private static Map<String, String> propertiesToMap(Properties properties) {
		if (properties == null) {
			return null;
		}

		Map<String, String> propMap = new HashMap<String, String>();
		for (Map.Entry<Object, Object> entry : properties.entrySet()) {
			propMap.put(String.valueOf(entry.getKey()), String.valueOf(entry.getValue()));
		}

		return propMap;
	}

	public static void transform(Object xml, Object xslt, Object result, Map<String, String> properties)
			throws TransformerException {

		TransformerFactory tFactory = TransformerFactory.newInstance();
		Source xslSource = getSource(xslt);
		Source xmlSource = getSource(xml);
		Result res = getResult(result);

		Transformer transformer = tFactory.newTransformer(xslSource);
		setTransformerProperties(transformer, properties);

		transformer.transform(xmlSource, res);
	}

	public static void transform(Object xml, Object xslt, Object result, Properties properties)
			throws TransformerException {
		transform(xml, xslt, result, propertiesToMap(properties));
	}

	public static void transform(Object xml, Object xslt, Object result) throws TransformerException {
		transform(xml, xslt, result, defaultTransformProperties);
	}

	public static String xmlToString(Object xml, Map<String, String> properties) throws TransformerException {
		StringWriter sw = new StringWriter();
		Transformer transformer = TransformerFactory.newInstance().newTransformer();
		setTransformerProperties(transformer, properties);
		Source source = getSource(xml);
		transformer.transform(source, new StreamResult(sw));
		return sw.toString();
	}

	public static String xmlToString(Object xml, Properties properties) throws TransformerException {
		return xmlToString(xml, propertiesToMap(properties));
	}

	public static String xmlToString(Object xml) throws TransformerException {
		return xmlToString(xml, defaultTransformProperties);
	}
}