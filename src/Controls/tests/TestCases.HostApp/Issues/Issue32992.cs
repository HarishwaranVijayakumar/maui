namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 32992, "Shell TabBarBackgroundColor not reset to Null", PlatformAffected.UWP | PlatformAffected.iOS)]
public class Issue32992 : Shell
{
	public Issue32992()
	{
		var tabBar = new TabBar();
		SetTabBarBackgroundColor(this, Colors.LightBlue);
		var removeColorButton = new Button
		{
			Text = "Remove TabBar Color",
			WidthRequest = 150,
			AutomationId = "Issue32992_Button"
		};
		removeColorButton.Clicked += OnRemoveTabBarColorClicked;

		var tab1Content = new ContentPage
		{
			Title = "TabBar Color Test",
			Content = new VerticalStackLayout
			{
				Padding = 20,
				Spacing = 15,
				Children =
				{
					removeColorButton
				}
			}
		};

		var tab1 = new Tab
		{
			Title = "Tab1",
			Items =
			{
				new ShellContent
				{
					Content = tab1Content,
				}
			}
		};

		var tab2Content = new ContentPage
		{
			Title = "Tab2 Content",
			Content = new Label
			{
				Text = "Welcome to Tab 2",
			}
		};

		var tab2 = new Tab
		{
			Title = "Tab2",
			Items =
			{
				new ShellContent
				{
					Content = tab2Content
				}
			}
		};

		var tab3Content = new ContentPage
		{
			Title = "Tab3 Content",
			Content = new Label
			{
				Text = "Welcome to Tab 3",
			}
		};

		var tab3 = new Tab
		{
			Title = "Tab3",
			Items =
			{
				new ShellContent
				{
					Content = tab3Content
				}
			}
		};

		tabBar.Items.Add(tab1);
		tabBar.Items.Add(tab2);
		tabBar.Items.Add(tab3);

		Items.Add(tabBar);
	}

	void OnRemoveTabBarColorClicked(object sender, EventArgs e)
	{
		SetTabBarBackgroundColor(this, null);
	}
}
