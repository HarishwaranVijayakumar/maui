using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Platform;
using Microsoft.Maui.Graphics.Win2D;
using Microsoft.UI.Xaml;

namespace Microsoft.Maui.Handlers
{
	public partial class GraphicsViewHandler : ViewHandler<IGraphicsView, PlatformTouchGraphicsView>
	{
		public override bool NeedsContainer =>
			VirtualView?.Background is ImageSourcePaint ||
			base.NeedsContainer;

		protected override PlatformTouchGraphicsView CreatePlatformView()
		{
			return new PlatformTouchGraphicsView();
		}

		private protected override void OnConnectHandler(FrameworkElement platformView)
		{
			base.OnConnectHandler(platformView);

			platformView.Loaded += OnLoaded;
		}

		private protected override void OnDisconnectHandler(FrameworkElement platformView)
		{
			base.OnDisconnectHandler(platformView);

			platformView.Loaded -= OnLoaded;
		}

		public static void MapBackground(IGraphicsViewHandler handler, IGraphicsView graphicsView)
		{
			if (graphicsView.Background is ImageSourcePaint)
			{
				handler.UpdateValue(nameof(IViewHandler.ContainerView));
				handler.ToPlatform().UpdateBackground(graphicsView);
			}

			handler.PlatformView?.Invalidate();
		}

		public static void MapDrawable(IGraphicsViewHandler handler, IGraphicsView graphicsView)
		{
			handler.PlatformView?.UpdateDrawable(graphicsView);
		}

		public static void MapFlowDirection(IGraphicsViewHandler handler, IGraphicsView graphicsView)
		{
			handler.PlatformView?.UpdateFlowDirection(graphicsView);
			handler.PlatformView?.Invalidate();
		}

		public static void MapInvalidate(IGraphicsViewHandler handler, IGraphicsView graphicsView, object? arg)
		{
			handler.PlatformView?.Invalidate();
		}

		void OnLoaded(object sender, RoutedEventArgs e)
		{
			VirtualView?.InvalidateMeasure();
		}
	}
}