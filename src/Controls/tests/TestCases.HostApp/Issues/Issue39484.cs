namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 39484, "SwipeItem background should reset when cleared", PlatformAffected.iOS | PlatformAffected.Android | PlatformAffected.macOS)]
public class Issue39484 : ContentPage
{
	public Issue39484()
	{
		BackgroundColor = Colors.White;

		var swipeItem = new SwipeItem
		{
			Text = "ToggleBackground",
			AutomationId = "ToggleBackground",
			BackgroundColor = Colors.Orange
		};
		swipeItem.Invoked += (sender, args) =>
		{
			bool clearBackground = swipeItem.BackgroundColor is not null;
			swipeItem.BackgroundColor = clearBackground ? null : Colors.Orange;
		};

		var swipeItems = new SwipeItems
		{
			Mode = SwipeMode.Reveal,
			SwipeBehaviorOnInvoked = SwipeBehaviorOnInvoked.RemainOpen
		};
		swipeItems.Add(swipeItem);

		Content = new VerticalStackLayout
		{
			Padding = 20,
			Spacing = 20,
			Children =
			{
				new Label
				{
					Text = "Swipe left, then tap the action to clear and restore its background. The action should stay open and orange should disappear when cleared.",
					TextColor = Colors.Black
				},
				new SwipeView
				{
					AutomationId = "BackgroundSwipeView",
					HeightRequest = 100,
					BackgroundColor = Colors.LightBlue,
					RightItems = swipeItems,
					Content = new Label
					{
						Text = "Swipe left",
						AutomationId = "SwipeContent",
						BackgroundColor = Colors.White,
						TextColor = Colors.Black,
						Padding = 20,
						VerticalTextAlignment = TextAlignment.Center
					}
				}
			}
		};
	}
}
