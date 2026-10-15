using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Maui.Controls.Issues;

[Issue(IssueTracker.Github, 17836, "CollectionView scrolls to the first item after loading more items", PlatformAffected.UWP)]
public class Issue17836 : ContentPage
{
	readonly ObservableCollection<Issue17836Group> _groups = new();
	readonly Label _groupCountLabel;

	public Issue17836()
	{
		for (int groupNumber = 1; groupNumber <= 3; groupNumber++)
		{
			_groups.Add(CreateGroup(groupNumber));
		}

		_groupCountLabel = new Label
		{
			AutomationId = "17836GroupCount",
			Text = $"Groups: {_groups.Count}"
		};

		var collectionView = new CollectionView
		{
			AutomationId = "17836CollectionView",
			Margin = 5,
			IsGrouped = true,
			ItemsSource = _groups,
			ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepItemsInView,
			RemainingItemsThreshold = 0,
			SelectionMode = SelectionMode.None,
			VerticalOptions = LayoutOptions.Fill,
			RemainingItemsThresholdReachedCommand = new Command(AddGroup),
			ItemTemplate = new DataTemplate(() =>
			{
				var label = new Label
				{
					HeightRequest = 56,
					VerticalTextAlignment = TextAlignment.Center
				};
				label.SetBinding(Label.TextProperty, nameof(Issue17836Item.Name));
				return label;
			})
		};

		var layout = new Grid
		{
			RowDefinitions =
			{
				new RowDefinition { Height = GridLength.Auto },
				new RowDefinition { Height = GridLength.Auto },
				new RowDefinition { Height = GridLength.Auto },
				new RowDefinition { Height = GridLength.Auto },
				new RowDefinition { Height = GridLength.Auto },
				new RowDefinition { Height = GridLength.Star }
			}
		};
		layout.Add(new Label
		{
			Text = "1. Scroll down the list until Group 3 is visible."
		}, 0, 0);
		layout.Add(new Label
		{
			Text = "2. Continue scrolling until the last item in Group 3 is visible."
		}, 0, 1);
		layout.Add(new Label
		{
			Text = "3. Wait for Group 4 to load, then continue scrolling."
		}, 0, 2);
		layout.Add(new Label
		{
			Text = "4. The list should not jump back to the first item after loading."
		}, 0, 3);
		layout.Add(_groupCountLabel, 0, 4);
		layout.Add(collectionView, 0, 5);

		Content = layout;
	}

	void AddGroup()
	{
		_groups.Add(CreateGroup(_groups.Count + 1));
		_groupCountLabel.Text = $"Groups: {_groups.Count}";
	}

	static Issue17836Group CreateGroup(int groupNumber)
	{
		var group = new Issue17836Group();
		for (int itemNumber = 1; itemNumber <= 12; itemNumber++)
		{
			group.Add(new Issue17836Item { Name = $"Group {groupNumber} Item {itemNumber}" });
		}

		return group;
	}

	sealed class Issue17836Group : List<Issue17836Item>
	{
	}

	sealed class Issue17836Item
	{
		public string Name { get; init; } = string.Empty;
	}
}
