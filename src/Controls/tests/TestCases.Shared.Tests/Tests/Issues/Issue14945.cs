using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue14945(TestDevice device) : _IssuesUITest(device)
{
	public override string Issue => "GraphicsView blend mode screenshots";

	[TestCase("Normal")]
	[TestCase("Multiply")]
	[TestCase("Screen")]
	[TestCase("Overlay")]
	[TestCase("Darken")]
	[TestCase("Lighten")]
	[TestCase("ColorDodge")]
	[TestCase("ColorBurn")]
	[TestCase("SoftLight")]
	[TestCase("HardLight")]
	[TestCase("Difference")]
	[TestCase("Exclusion")]
	[TestCase("Hue")]
	[TestCase("Saturation")]
	[TestCase("Color")]
	[TestCase("Luminosity")]
	[TestCase("Clear")]
	[TestCase("Copy")]
	[TestCase("SourceIn")]
	[TestCase("SourceOut")]
	[TestCase("SourceAtop")]
	[TestCase("DestinationOver")]
	[TestCase("DestinationIn")]
	[TestCase("DestinationOut")]
	[TestCase("DestinationAtop")]
	[TestCase("Xor")]
	[TestCase("PlusDarker")]
	[TestCase("PlusLighter")]
	[Category(UITestCategories.GraphicsView)]
	public void VerifyBlendModeScreenshot(string mode)
	{
		App.WaitForElement("BlendModeInput");
		App.ClearText("BlendModeInput");
		App.EnterText("BlendModeInput", mode);
		App.WaitForElement("RenderBlendMode");
		App.Tap("RenderBlendMode");
		Assert.That(App.WaitForTextEqualToElement("BlendModeDrawStatus", $"Drawn: {mode}"), Is.True,
			$"The drawable for {mode} did not finish drawing.");

		VerifyScreenshot($"BlendMode_{mode}", retryTimeout: TimeSpan.FromSeconds(2));
	}
}
