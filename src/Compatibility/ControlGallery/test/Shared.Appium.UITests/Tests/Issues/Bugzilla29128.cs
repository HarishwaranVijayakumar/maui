﻿using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace UITests
{
	public class Bugzilla29128 : IssuesUITest
	{
		public Bugzilla29128(TestDevice testDevice) : base(testDevice)
		{
		}

		public override string Issue => "Slider background lays out wrong Android";

		[Test]
		[Category(UITestCategories.LifeCycle)]
		public void Bugzilla29128Test()
		{
			this.IgnoreIfPlatforms([TestDevice.iOS, TestDevice.Mac, TestDevice.Windows]);

			RunningApp.WaitForElement("SliderId");
			RunningApp.Screenshot("Slider and button should be centered");
			Assert.Inconclusive("For visual review only");
		}
	}
}