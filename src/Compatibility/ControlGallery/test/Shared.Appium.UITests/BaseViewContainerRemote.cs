﻿using System.Runtime.CompilerServices;
using UITest.Appium;
using UITest.Core;

namespace UITests
{
	internal abstract partial class BaseViewContainerRemote
	{
		protected IApp App { get; private set; }

		public string ViewQuery { get; private set; }
		public string EventLabelQuery { get; set; }

		public string StateLabelQuery { get; private set; }

		public string StateButtonQuery { get; private set; }

		protected BaseViewContainerRemote(IApp app, Enum type)
		{
			App = app;

			ViewQuery = string.Format("{0}VisualElement", type);
			EventLabelQuery = string.Format("{0}EventLabel", type);
			StateLabelQuery = string.Format("{0}StateLabel", type);
			StateButtonQuery = string.Format("{0}StateButton", type);
		}

		public virtual void GoTo([CallerMemberName] string callerMemberName = "")
		{
			App.WaitForElement("TargetViewContainer");
			App.Tap("TargetViewContainer");
			string text = callerMemberName.Replace("_", "", StringComparison.CurrentCultureIgnoreCase) + "VisualElement";
			App.EnterText("TargetViewContainer", text);
			App.Tap("GoButton");
		}

		public void TapView()
		{
			App.Tap(ViewQuery);
		}
	}
}