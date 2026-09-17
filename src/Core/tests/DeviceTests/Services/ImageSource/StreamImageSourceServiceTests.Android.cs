using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Microsoft.Maui.DeviceTests.Stubs;
using Xunit;
using Color = Microsoft.Maui.Graphics.Color;

namespace Microsoft.Maui.DeviceTests
{
	public partial class StreamImageSourceServiceTests
	{
		[Theory]
		[InlineData(typeof(FileImageSourceStub))]
		[InlineData(typeof(FontImageSourceStub))]
		[InlineData(typeof(UriImageSourceStub))]
		public async Task ThrowsForIncorrectTypes(Type type)
		{
			var service = new StreamImageSourceService();

			var imageSource = (ImageSourceStub)Activator.CreateInstance(type);

			await Assert.ThrowsAsync<InvalidCastException>(() => service.GetDrawableAsync(imageSource, MauiProgram.DefaultContext));
		}

		[Theory]
		[InlineData("#FF0000")]
		[InlineData("#00FF00")]
		[InlineData("#000000")]
		public async Task GetDrawableAsync(string colorHex)
		{
			var expectedColor = Color.FromArgb(colorHex).ToPlatform();

			var service = new StreamImageSourceService();

			var stream = CreateBitmapStream(100, 100, expectedColor);

			var imageSource = new StreamImageSourceStub(stream);

			using var result = await service.GetDrawableAsync(imageSource, MauiProgram.DefaultContext);

			var bitmapDrawable = Assert.IsType<BitmapDrawable>(result.Value);

			var bitmap = bitmapDrawable.Bitmap;

			await bitmap.AssertContainsColor(expectedColor).ConfigureAwait(false);
		}

		[Fact]
		public async Task LoadDrawableAsyncSurvivesGlideRestart()
		{
			var expectedColor = Colors.Red.ToPlatform();
			using var bitmapStream = Assert.IsType<MemoryStream>(CreateBitmapStream(100, 100, expectedColor));
			using var trackingStream = new TrackingStream(bitmapStream.ToArray());
			var imageSource = new StreamImageSourceStub(trackingStream);
			var service = new StreamImageSourceService();
			using var imageView = new RequestTrackingImageView(MauiProgram.DefaultContext);
			var unhandledException = new TaskCompletionSource<Exception>(TaskCreationOptions.RunContinuationsAsynchronously);

			void OnUnhandledException(object sender, RaiseThrowableEventArgs args)
			{
				if (args.Exception?.ToString().Contains(nameof(TrackingStream), StringComparison.Ordinal) == true)
				{
					unhandledException.TrySetResult(args.Exception);
					args.Handled = true;
				}
			}

			AndroidEnvironment.UnhandledExceptionRaiser += OnUnhandledException;

			try
			{
				await InvokeOnMainThreadAsync(() => imageView.AttachAndRun(async () =>
				{
					var requestManager = Glide.With(imageView);
					var loadTask = service.LoadDrawableAsync(imageSource, imageView);

					await trackingStream.ReadStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));

					try
					{
						IRequest request;
						if (imageView.RequestSubmitted.Task.IsCompleted)
						{
							request = await imageView.RequestSubmitted.Task;
							Assert.True(request.IsRunning);
							request.Pause();
							Assert.False(request.IsRunning);

							trackingStream.ReleaseRead();
							await trackingStream.ReadCompleted.Task.WaitAsync(TimeSpan.FromSeconds(5));
							await trackingStream.Disposed.Task.WaitAsync(TimeSpan.FromSeconds(5));
						}
						else
						{
							trackingStream.ReleaseRead();
							await trackingStream.ReadCompleted.Task.WaitAsync(TimeSpan.FromSeconds(5));
							await trackingStream.Disposed.Task.WaitAsync(TimeSpan.FromSeconds(5));

							request = await imageView.RequestSubmitted.Task.WaitAsync(TimeSpan.FromSeconds(5));
							Assert.True(request.IsRunning);
							request.Pause();
							Assert.False(request.IsRunning);
						}

						request.Begin();

						var completedTask = await Task.WhenAny(loadTask, unhandledException.Task).WaitAsync(TimeSpan.FromSeconds(5));
						if (completedTask == unhandledException.Task)
							Assert.Null(await unhandledException.Task);

						using var result = await loadTask;
						Assert.NotNull(result);

						var bitmapDrawable = Assert.IsType<BitmapDrawable>(imageView.Drawable);
						await bitmapDrawable.Bitmap.AssertContainsColor(expectedColor);
					}
					finally
					{
						trackingStream.ReleaseRead();
						requestManager.Clear(imageView);
					}
				}));
			}
			finally
			{
				trackingStream.ReleaseRead();
				AndroidEnvironment.UnhandledExceptionRaiser -= OnUnhandledException;
			}
		}

		sealed class RequestTrackingImageView : ImageView
		{
			public RequestTrackingImageView(global::Android.Content.Context context)
				: base(context)
			{
			}

			public TaskCompletionSource<IRequest> RequestSubmitted { get; } =
				new(TaskCreationOptions.RunContinuationsAsynchronously);

			public override void SetTag(int key, Java.Lang.Object tag)
			{
				base.SetTag(key, tag);

				if (tag is IRequest request)
					RequestSubmitted.TrySetResult(request);
			}
		}

		sealed class TrackingStream : MemoryStream
		{
			readonly TaskCompletionSource _releaseRead = new(TaskCreationOptions.RunContinuationsAsynchronously);
			int _blockNextRead = 1;

			public TrackingStream(byte[] buffer)
				: base(buffer)
			{
			}

			public TaskCompletionSource ReadStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
			public TaskCompletionSource ReadCompleted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
			public TaskCompletionSource Disposed { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

			public void ReleaseRead() => _releaseRead.TrySetResult();

			public override int Read(byte[] buffer, int offset, int count)
			{
				BlockRead();

				try
				{
					return base.Read(buffer, offset, count);
				}
				finally
				{
					ReadCompleted.TrySetResult();
				}
			}

			public override async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
			{
				await BlockReadAsync(cancellationToken);

				try
				{
					await base.CopyToAsync(destination, bufferSize, cancellationToken);
				}
				finally
				{
					ReadCompleted.TrySetResult();
				}
			}

			protected override void Dispose(bool disposing)
			{
				try
				{
					base.Dispose(disposing);
				}
				finally
				{
					Disposed.TrySetResult();
				}
			}

			void BlockRead()
			{
				if (Interlocked.Exchange(ref _blockNextRead, 0) == 0)
				{
					return;
				}

				ReadStarted.TrySetResult();
				_releaseRead.Task.GetAwaiter().GetResult();
			}

			async Task BlockReadAsync(CancellationToken cancellationToken)
			{
				if (Interlocked.Exchange(ref _blockNextRead, 0) == 0)
				{
					return;
				}

				ReadStarted.TrySetResult();
				await _releaseRead.Task.WaitAsync(cancellationToken);
			}
		}
	}
}
