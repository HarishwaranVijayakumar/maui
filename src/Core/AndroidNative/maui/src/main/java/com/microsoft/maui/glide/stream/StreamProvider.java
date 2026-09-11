package com.microsoft.maui.glide.stream;

import java.io.IOException;
import java.io.InputStream;

public interface StreamProvider {
    InputStream open() throws IOException;
}
