#nullable enable
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Widget;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Glide.Stream;

namespace Microsoft.Maui
{
	public partial class StreamImageSourceService
	{
		public override async Task<IImageSourceServiceResult?> LoadDrawableAsync(IImageSource imageSource, ImageView imageView, CancellationToken cancellationToken = default)
		{
			var streamImageSource = (IStreamImageSource)imageSource;

			if (!streamImageSource.IsEmpty)
			{
				try
				{
					var callback = new ImageLoaderCallback();

					PlatformInterop.LoadImageFromStreamProvider(
						imageView,
						new ReplayableStreamProvider(streamImageSource, cancellationToken),
						callback);

					return await callback.Result;
				}
				catch (Exception ex)
				{
					Logger?.LogWarning(ex, "Unable to load image stream.");
					throw;
				}
			}

			return null;
		}

		public override async Task<IImageSourceServiceResult<Drawable>?> GetDrawableAsync(IImageSource imageSource, Context context, CancellationToken cancellationToken = default)
		{
			var streamImageSource = (IStreamImageSource)imageSource;

			if (!streamImageSource.IsEmpty)
			{
				try
				{
					var drawableCallback = new ImageLoaderResultCallback();

					PlatformInterop.LoadImageFromStreamProvider(
						context,
						new ReplayableStreamProvider(streamImageSource, cancellationToken),
						drawableCallback);

					return await drawableCallback.Result.ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					Logger?.LogWarning(ex, "Unable to load image stream.");
					throw;
				}
			}

			return null;
		}
		sealed class ReplayableStreamProvider : Java.Lang.Object, IStreamProvider
		{
			readonly IStreamImageSource _streamImageSource;
			readonly CancellationToken _cancellationToken;

			public ReplayableStreamProvider(IStreamImageSource streamImageSource, CancellationToken cancellationToken)
			{
				_streamImageSource = streamImageSource;
				_cancellationToken = cancellationToken;
			}

			public Stream Open() =>
				_streamImageSource.GetStreamAsync(_cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
		}
	}
}