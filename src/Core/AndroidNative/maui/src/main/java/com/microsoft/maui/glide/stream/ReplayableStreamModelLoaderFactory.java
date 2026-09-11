package com.microsoft.maui.glide.stream;

import androidx.annotation.NonNull;

import com.bumptech.glide.load.model.ModelLoader;
import com.bumptech.glide.load.model.ModelLoaderFactory;
import com.bumptech.glide.load.model.MultiModelLoaderFactory;

import java.io.InputStream;

public class ReplayableStreamModelLoaderFactory implements ModelLoaderFactory<StreamProvider, InputStream> {
    @NonNull
    @Override
    public ModelLoader<StreamProvider, InputStream> build(@NonNull MultiModelLoaderFactory multiFactory) {
        return new ReplayableStreamModelLoader();
    }

    @Override
    public void teardown() {
    }
}
