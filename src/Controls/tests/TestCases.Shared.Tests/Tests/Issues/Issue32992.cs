using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue32992 : _IssuesUITest
{
	public Issue32992(TestDevice device) : base(device)
	{
	}

	public override string Issue => "Shell TabBarBackgroundColor not reset to Null";

	[Test]
	[Category(UITestCategories.Shell)]
	public void VerifyShellTabBarBackgroundColorReset()
	{
		App.WaitForElement("Issue32992_Button");
		App.Tap("Issue32992_Button");
		VerifyScreenshot();
	}
}