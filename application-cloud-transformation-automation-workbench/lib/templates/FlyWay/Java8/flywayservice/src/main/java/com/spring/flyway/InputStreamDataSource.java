package com.spring.flyway;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;

import org.apache.commons.io.IOUtils;
import org.springframework.core.io.InputStreamSource;

public class InputStreamDataSource implements InputStreamSource{
	String contentType;
    String name;

    byte[] fileData;

    public InputStreamDataSource(String contentType, String name, InputStream inputStream) throws IOException {
        this.contentType = contentType;
        this.name = name;

        fileData = IOUtils.toByteArray(inputStream);
    }

    public String getContentType() {
        return contentType;
    }

    public String getName() {
        return name;
    }

    public InputStream getInputStream() throws IOException {

        return new ByteArrayInputStream(fileData);
    }

    public OutputStream getOutputStream() throws IOException {
        throw new UnsupportedOperationException("Not implemented");
    }
}
