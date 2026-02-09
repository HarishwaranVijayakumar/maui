namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 33270, "PointerGestureRecognizer is never fired when the view has a PanGestureRecognizer attached", PlatformAffected.Android)]
public class Issue33270 : ContentPage
{
	public Issue33270()
	{
		var movedLabel = new Label { AutomationId = "PointerMovedLabel", Text = "PointerMoved: Waiting" };
		var exitedLabel = new Label { AutomationId = "PointerExitedLabel", Text = "PointerExited: Waiting" };

		var target = new BoxView
		{
			AutomationId = "TargetView",
			WidthRequest = 200,
			HeightRequest = 200,
			BackgroundColor = Colors.LightBlue,
			HorizontalOptions = LayoutOptions.Center
		};

		var pan = new PanGestureRecognizer();
		var pointer = new PointerGestureRecognizer();
		pointer.PointerMoved += (s, e) => movedLabel.Text = "PointerMoved: Success";
		pointer.PointerExited += (s, e) => exitedLabel.Text = "PointerExited: Success";

		target.GestureRecognizers.Add(pan);
		target.GestureRecognizers.Add(pointer);

		Content = new VerticalStackLayout
		{
			Spacing = 10,
			Padding = 20,
			Children = { target, movedLabel, exitedLabel }
		};
	}
}
