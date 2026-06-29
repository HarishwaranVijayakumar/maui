namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.None, 99999, "Slider DragStarted, DragCompleted, and ValueChanged events test", PlatformAffected.Android)]
public class SliderDragAndValueChangedTest : ContentPage
{
	Label _dragStartLabel;
	Label _dragCompletedLabel;
	Label _valueChangedLabel;
	Label _oldValueLabel;
	Label _newValueLabel;

	public SliderDragAndValueChangedTest()
	{
		_dragStartLabel = new Label
		{
			Text = string.Empty,
			AutomationId = "DragStartedLabel"
		};

		_dragCompletedLabel = new Label
		{
			Text = string.Empty,
			AutomationId = "DragCompletedLabel"
		};

		_valueChangedLabel = new Label
		{
			Text = "Not Raised",
			AutomationId = "ValueChangedLabel"
		};

		_oldValueLabel = new Label
		{
			Text = "0.00",
			AutomationId = "OldValueLabel"
		};

		_newValueLabel = new Label
		{
			Text = "0.00",
			AutomationId = "NewValueLabel"
		};

		var slider = new Slider
		{
			Minimum = 0,
			Maximum = 100,
			Value = 50,
			AutomationId = "TestSlider"
		};

		slider.DragStarted += (s, e) =>
		{
			_dragStartLabel.Text = "Drag Started";
		};

		slider.DragCompleted += (s, e) =>
		{
			_dragCompletedLabel.Text = "Drag Completed";
		};

		slider.ValueChanged += (s, e) =>
		{
			_valueChangedLabel.Text = "Raised";
			_oldValueLabel.Text = e.OldValue.ToString("F2");
			_newValueLabel.Text = e.NewValue.ToString("F2");
		};

		var sliderContainer = new Grid
		{
			AutomationId = "SliderContainer",
			HeightRequest = 80,
			Children = { slider }
		};

		Content = new VerticalStackLayout
		{
			Padding = 20,
			Spacing = 10,
			Children =
			{
				new Label { Text = "Drag the slider to test events:", AutomationId = "PageTitle" },
				sliderContainer,
				new Label { Text = "DragStarted:" },
				_dragStartLabel,
				new Label { Text = "DragCompleted:" },
				_dragCompletedLabel,
				new Label { Text = "ValueChanged:" },
				_valueChangedLabel,
				new Label { Text = "OldValue:" },
				_oldValueLabel,
				new Label { Text = "NewValue:" },
				_newValueLabel
			}
		};
	}
}
