namespace Controls.TestCases.Issues;

[Issue(IssueTracker.Github, 12131, "Refreshview - Collectionview sizing not working correctly", PlatformAffected.Android)]
public class Issue12131 : ContentPage
{
	public Issue12131()
	{
		VerticalStackLayout stack = new VerticalStackLayout();
		RefreshView refreshView = new RefreshView();
		CollectionView collectionView = new CollectionView();
		collectionView.ItemsSource = Enumerable.Range(1, 20).Select(i => $"Item {i}").ToList();
		refreshView.Content = collectionView;
		stack.Children.Add(refreshView);
		Content = stack;
	}
}
