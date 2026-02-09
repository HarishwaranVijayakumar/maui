using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue33270 : _IssuesUITest
{
	public Issue33270(TestDevice device) : base(device) { }

	public override string Issue => "PointerGestureRecognizer is never fired when the view has a PanGestureRecognizer attached";

	[Test]
	[Category(UITestCategories.Gestures)]
	public void PointerGestureFiresWithPanGesture()
	{
		var target = App.WaitForElement("TargetView");
		var rect = target.GetRect();

		// Pan from center to well outside the view bounds
		App.DragCoordinates(rect.CenterX(), rect.CenterY(), rect.X + rect.Width + 100, rect.CenterY());
		Assert.That(App.WaitForElement("PointerMovedLabel").GetText(), Is.EqualTo("PointerMoved: Success"));
		Assert.That(App.WaitForElement("PointerExitedLabel").GetText(), Is.EqualTo("PointerExited: Success"));
	}
}
