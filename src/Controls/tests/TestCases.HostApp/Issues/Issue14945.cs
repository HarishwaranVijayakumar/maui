namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 14945, "GraphicsView blend mode screenshots", PlatformAffected.All)]
public class Issue14945 : ContentPage
{
	readonly UITestEntry _modeEntry;
	readonly Label _description;
	readonly Label _drawStatus;
	readonly GraphicsView _graphicsView;
	BlendMode _mode;
	int _renderVersion;

	public Issue14945()
	{
		BackgroundColor = Colors.White;

		_modeEntry = new UITestEntry
		{
			AutomationId = "BlendModeInput",
			Text = nameof(BlendMode.Normal),
			Placeholder = "Blend mode name",
			IsCursorVisible = false,
			TextColor = Colors.Black
		};
		var renderButton = new Button
		{
			AutomationId = "RenderBlendMode",
			Text = "Render"
		};
		renderButton.Clicked += (sender, args) =>
		{
			_modeEntry.Unfocus();
			if (Enum.TryParse<BlendMode>(_modeEntry.Text, out var mode) && Enum.IsDefined(mode))
			{
				RenderMode(mode);
			}
			else
			{
				_drawStatus.Text = $"Invalid blend mode: {_modeEntry.Text}";
			}
		};
		var nextButton = new Button
		{
			AutomationId = "NextBlendMode",
			Text = "Next mode"
		};
		nextButton.Clicked += (sender, args) =>
		{
			var modes = Enum.GetValues<BlendMode>();
			int index = Array.IndexOf(modes, _mode);
			var nextMode = modes[(index + 1) % modes.Length];
			_modeEntry.Text = nextMode.ToString();
			_modeEntry.Unfocus();
			RenderMode(nextMode);
		};

		_description = new Label
		{
			AutomationId = "BlendModeExplanation",
			TextColor = Colors.Black
		};
		_drawStatus = new Label
		{
			AutomationId = "BlendModeDrawStatus",
			TextColor = Colors.Black
		};
		_graphicsView = new GraphicsView
		{
			AutomationId = "BlendModeCanvas"
		};

		var controls = new Grid
		{
			ColumnDefinitions =
			{
				new ColumnDefinition(GridLength.Star),
				new ColumnDefinition(GridLength.Auto),
				new ColumnDefinition(GridLength.Auto)
			},
			ColumnSpacing = 8
		};
		controls.Add(_modeEntry, 0, 0);
		controls.Add(renderButton, 1, 0);
		controls.Add(nextButton, 2, 0);

		var layout = new Grid
		{
			Padding = 12,
			RowSpacing = 8,
			RowDefinitions =
			{
				new RowDefinition(GridLength.Auto),
				new RowDefinition(GridLength.Auto),
				new RowDefinition(GridLength.Auto),
				new RowDefinition(GridLength.Star)
			}
		};
		layout.Add(controls, 0, 0);
		layout.Add(_description, 0, 1);
		layout.Add(_drawStatus, 0, 2);
		layout.Add(_graphicsView, 0, 3);
		Content = layout;

		RenderMode(BlendMode.Normal);
	}

	void RenderMode(BlendMode mode)
	{
		_mode = mode;
		int version = ++_renderVersion;
		var drawable = new BlendModeDrawable(mode);
		_description.Text = drawable.Explanation;
		_drawStatus.Text = $"Waiting for draw: {mode}";
		_graphicsView.Drawable = new DrawTrackingDrawable(drawable, () =>
		{
			// Publish readiness after Draw returns, not when the mode is merely selected.
			Dispatcher.Dispatch(() =>
			{
				if (version == _renderVersion)
					_drawStatus.Text = $"Drawn: {mode}";
			});
		});
		_graphicsView.Invalidate();
	}

	class DrawTrackingDrawable(IDrawable drawable, Action onDrawn) : IDrawable
	{
		bool _notified;

		public void Draw(ICanvas canvas, RectF dirtyRect)
		{
			drawable.Draw(canvas, dirtyRect);
			if (!_notified && dirtyRect.Width > 0 && dirtyRect.Height > 0)
			{
				_notified = true;
				onDrawn();
			}
		}
	}
}
