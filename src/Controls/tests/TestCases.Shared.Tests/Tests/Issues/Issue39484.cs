using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue39484 : _IssuesUITest
{
	public Issue39484(TestDevice device) : base(device) { }

	public override string Issue => "SwipeItem background should reset when cleared";

	[Test]
	[Category(UITestCategories.SwipeView)]
	public void SwipeItemBackgroundShouldClearAndRestoreWhileOpen()
	{
		App.WaitForElement("SwipeContent");
		App.SwipeRightToLeft("SwipeContent");
		App.WaitForElement("ToggleBackground");

		Exception? exception = null;
		VerifyScreenshotOrSetException(ref exception, "Issue39484_OrangeBackground", retryTimeout: TimeSpan.FromSeconds(2));

		// Invoke the action itself so an outside tap cannot dismiss the native swipe control.
		App.Tap("ToggleBackground");
		App.WaitForElement("ToggleBackground");
		VerifyScreenshotOrSetException(ref exception, "Issue39484_ClearedBackground", retryTimeout: TimeSpan.FromSeconds(2));

		App.Tap("ToggleBackground");
		App.WaitForElement("ToggleBackground");
		VerifyScreenshotOrSetException(ref exception, "Issue39484_RestoredBackground", retryTimeout: TimeSpan.FromSeconds(2));

		if (exception is not null)
			throw exception;
	}
}
