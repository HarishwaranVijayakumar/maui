using Microsoft.Maui.Graphics;
using Microsoft.Maui.Platform;
using UIKit;

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

		public static void MapBackground(IGraphicsViewHandler handler, IGraphicsView graphicsView)
		{
			if (graphicsView.Background is ImageSourcePaint)
			{
				handler.UpdateValue(nameof(IViewHandler.ContainerView));
				handler.ToPlatform().UpdateBackground(graphicsView);
			}

			handler.PlatformView?.InvalidateDrawable();
		}

		public static void MapDrawable(IGraphicsViewHandler handler, IGraphicsView graphicsView)
		{
			handler.PlatformView?.UpdateDrawable(graphicsView);
		}

		public static void MapFlowDirection(IGraphicsViewHandler handler, IGraphicsView graphicsView)
		{
			handler.PlatformView?.UpdateFlowDirection(graphicsView);
			handler.PlatformView?.InvalidateDrawable();
		}

		public static void MapInvalidate(IGraphicsViewHandler handler, IGraphicsView graphicsView, object? arg)
		{
			handler.PlatformView?.InvalidateDrawable();
		}
	}
}