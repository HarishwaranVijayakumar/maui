package com.microsoft.maui.glide.stream;

import androidx.annotation.NonNull;

import com.bumptech.glide.Priority;
import com.bumptech.glide.load.DataSource;
import com.bumptech.glide.load.Options;
import com.bumptech.glide.load.data.DataFetcher;
import com.bumptech.glide.load.model.ModelLoader;
import com.bumptech.glide.signature.ObjectKey;

import java.io.IOException;
import java.io.InputStream;

public class ReplayableStreamModelLoader implements ModelLoader<StreamProvider, InputStream> {
    @Override
    public LoadData<InputStream> buildLoadData(@NonNull StreamProvider streamProvider, int width, int height, @NonNull Options options) {
        return new LoadData<>(new ObjectKey(streamProvider), new DataFetcher<InputStream>() {
            private InputStream inputStream;

            @Override
            public void loadData(@NonNull Priority priority, @NonNull DataCallback<? super InputStream> callback) {
                try {
                    inputStream = streamProvider.open();
                    if (inputStream == null) {
                        callback.onLoadFailed(new IOException("The stream provider returned null."));
                        return;
                    }

                    callback.onDataReady(inputStream);
                } catch (Exception e) {
                    callback.onLoadFailed(e);
                }
            }

            @Override
            public void cleanup() {
                if (inputStream == null) {
                    return;
                }

                try {
                    inputStream.close();
                } catch (IOException e) {
                } finally {
                    inputStream = null;
                }
            }

            @Override
            public void cancel() {
            }

            @NonNull
            @Override
            public Class<InputStream> getDataClass() {
                return InputStream.class;
            }

            @NonNull
            @Override
            public DataSource getDataSource() {
                return DataSource.LOCAL;
            }
        });
    }

    @Override
    public boolean handles(@NonNull StreamProvider streamProvider) {
        return true;
    }
}
