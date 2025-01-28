using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues
{
	internal class Issue22517 : _IssuesUITest
	{
		public Issue22517(TestDevice testDevice) : base(testDevice)
		{
		}
		public override string Issue => "ImageButton Aspect is incorrect in windows";

		[Test]
		[Category(UITestCategories.ImageButton)]
		public void ImageButtonAspect()
		{
			App.WaitForElement("ImageButton1");
			App.WaitForElement("ImageButton2");
			App.WaitForElement("ImageButton3");
			App.WaitForElement("ImageButton4");
			App.WaitForElement("Button");
			App.Tap("Button");
			VerifyScreenshot();
		}
	}
}
