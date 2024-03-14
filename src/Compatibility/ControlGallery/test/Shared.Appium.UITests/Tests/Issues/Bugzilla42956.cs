﻿using NUnit.Framework;
using UITest.Appium;

namespace UITests
{
	public class Bugzilla42956 : IssuesUITest
	{
		const string Success = "Success";

		public Bugzilla42956(TestDevice testDevice) : base(testDevice)
		{
		}

		public override string Issue => "[Bug] Exception Ancestor must be provided for all pushes except first";

		[Test]
		[Category(UITestCategories.ListView)]
		[FailsOnIOS]
		public void Bugzilla42956Test()
		{
			RunningApp.WaitForElement(Success);
		}
	}
}