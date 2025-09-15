namespace Controls.TestCases.Issues;

[Issue(IssueTracker.Github, 12131, "Refreshview - Collectionview sizing not working correctly", PlatformAffected.Android)]
public class Issue12131 : ContentPage
{
	public Issue12131()
	{
		VerticalStackLayout stack = new VerticalStackLayout();
		RefreshView refreshView = new RefreshView();
		Label label = new Label();
		label.Text = "Success";
		refreshView.Content = label;
		stack.Children.Add(refreshView);
		Content = stack;
	}
}
