#if ANDROID
using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class SliderDragAndValueChangedTest : _IssuesUITest
{
	public override string Issue => "Slider DragStarted, DragCompleted, and ValueChanged events test";

	public SliderDragAndValueChangedTest(TestDevice device) : base(device) { }

	[Test]
	[Category(UITestCategories.Material3)]
	public void SliderDragStartedAndCompletedEventsFire()
	{
		App.WaitForElement("SliderContainer");

		var sliderRect = App.WaitForElement("SliderContainer").GetRect();
		var startX = sliderRect.X + (sliderRect.Width * 30 / 100);
		var centerY = sliderRect.Y + (sliderRect.Height / 2);
		var endX = sliderRect.X + (sliderRect.Width * 70 / 100);

		App.DragCoordinates(startX, centerY, endX, centerY);

		Assert.That(App.WaitForElement("DragStartedLabel").GetText(), Is.EqualTo("Drag Started"));
		Assert.That(App.WaitForElement("DragCompletedLabel").GetText(), Is.EqualTo("Drag Completed"));
	}

	[Test]
	[Category(UITestCategories.Material3)]
	public void SliderValueChangedEventFires()
	{
		App.WaitForElement("SliderContainer");

		var sliderRect = App.WaitForElement("SliderContainer").GetRect();
		var startX = sliderRect.X + (sliderRect.Width / 2);
		var centerY = sliderRect.Y + (sliderRect.Height / 2);
		var endX = sliderRect.X + (sliderRect.Width * 80 / 100);

		App.DragCoordinates(startX, centerY, endX, centerY);

		Assert.That(App.WaitForElement("ValueChangedLabel").GetText(), Is.EqualTo("Raised"));

		var newValue = double.Parse(App.FindElement("NewValueLabel").GetText()!);
		Assert.That(newValue, Is.GreaterThan(50.0));
	}
}
#endif
