using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue12131 : _IssuesUITest
{
	public override string Issue => "Refreshview - Collectionview sizing not working correctly";

	public Issue12131(TestDevice device) : base(device)
	{
	}

	[Test]
	[Category(UITestCategories.RefreshView)]
	public void ItemsShouldBeVisible()
	{
		App.WaitForElement("Success");
	}
}
