using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue17836 : _IssuesUITest
{
	public Issue17836(TestDevice device)
		: base(device)
	{
	}

	public override string Issue => "CollectionView scrolls to the first item after loading more items";

	protected override bool ResetAfterEachTest => true;

	[Test]
	[ShardedTestCategory(UITestCategories.CollectionView, shard: 5)]
	public void LoadingMoreItemsKeepsTheCurrentItemsVisible()
	{
		App.WaitForElement("17836GroupCount");

		for (int attempt = 0; attempt < 10; attempt++)
		{
			if (App.FindElement("17836GroupCount").GetText() == "Groups: 4")
			{
				break;
			}

			App.ScrollDown("17836CollectionView", ScrollStrategy.Gesture, 0.8, 500);
		}

		Assert.That(App.WaitForElement("17836GroupCount").GetText(), Is.EqualTo("Groups: 4"));
		App.WaitForElement("Group 3 Item 12");
	}
}
