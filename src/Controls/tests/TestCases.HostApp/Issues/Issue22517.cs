using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controls.TestCases.HostApp.Issues
{
	[Issue(IssueTracker.Github, 22517, "ImageButton Aspect is incorrect in windows", PlatformAffected.UWP)]
	class Issue22517:ContentPage
	{
		ImageButton imageButton1, imageButton2, imageButton3, imageButton4;
		public Issue22517()
		{
			imageButton1 = new ImageButton
			{
				AutomationId = "ImageButton1",
				Source = "small_dotnet_bot.png"
			};
			imageButton2 = new ImageButton
			{
				AutomationId = "ImageButton2",
				Source = "small_dotnet_bot.png"

			};
			imageButton3 = new ImageButton
			{
				AutomationId = "ImageButton3",
				Source = "small_dotnet_bot.png"
			};
			imageButton4 = new ImageButton
			{
				AutomationId = "ImageButton4",
				Source = "small_dotnet_bot.png"
			};
			Button button = new Button
			{
				AutomationId = "Button",
				Text = "Click to change Aspects"
				
			};
			StackLayout stackLayout = new StackLayout
			{
				Children = { imageButton1, imageButton2, imageButton3, imageButton4, button }
			};
			Content = stackLayout;
			button.Clicked += Button_Clicked;	

		}
		private void Button_Clicked(object sender, EventArgs e)
		{
			imageButton1.HeightRequest = 100;
			imageButton1.Aspect=Aspect.AspectFill;
			imageButton2.HeightRequest = 100;
			imageButton2.Aspect = Aspect.AspectFit;
			imageButton3.HeightRequest = 100;
			imageButton3.Aspect = Aspect.Fill;
			imageButton4.HeightRequest = 100;
			imageButton4.Aspect = Aspect.Center;
		}
	}
}
