using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace Maui.Controls.Sample.Issues
{
	[Issue(IssueTracker.Github, 12928, "ImageBrush background support for controls", PlatformAffected.All)]
	public partial class Issue12928 : ContentPage
	{
		readonly Dictionary<string, Func<View>> _controlFactories;

		public Issue12928()
		{
			InitializeComponent();
			_controlFactories = CreateControlFactories();
			ShowControls("Label", "Button");
		}

		ImageBrush CreateImageBrush()
		{
			return new ImageBrush { ImageSource = "groceries.png" };
		}

		Dictionary<string, Func<View>> CreateControlFactories()
		{
			return new Dictionary<string, Func<View>>
			{
				["Label"] = CreateLabel,
				["Button"] = CreateButton,
				["Entry"] = CreateEntry,
				["Editor"] = CreateEditor,
				["SearchBar"] = CreateSearchBar,
				["Picker"] = CreatePicker,
				["DatePicker"] = CreateDatePicker,
				["TimePicker"] = CreateTimePicker,
				["Switch"] = CreateSwitch,
				["Stepper"] = CreateStepper,
				["Slider"] = CreateSlider,
				["ProgressBar"] = CreateProgressBar,
				["CheckBox"] = CreateCheckBox,
				["RadioButton"] = CreateRadioButton,
				["Image"] = CreateImage,
				["ImageButton"] = CreateImageButton,
				["ContentView"] = CreateContentView,
				["Border"] = CreateBorder,
				["CollectionView"] = CreateCollectionView,
				["SwipeView"] = CreateSwipeView,
				["CarouselView"] = CreateCarouselView,
				["StackLayout"] = CreateStackLayout,
				["Grid"] = CreateGrid,
				["FlexLayout"] = CreateFlexLayout,
				["AbsoluteLayout"] = CreateAbsoluteLayout,
				["ScrollView"] = CreateScrollView,
			};
		}

		public IReadOnlyList<string> ControlNames => _controlFactories.Keys.ToList();

		public void ShowControls(string control1, string control2)
		{
			if (_controlFactories.TryGetValue(control1, out var factory1))
			{
				Control1Label.Text = control1;
				Control1Container.Content = factory1();
			}

			if (_controlFactories.TryGetValue(control2, out var factory2))
			{
				Control2Label.Text = control2;
				Control2Container.Content = factory2();
			}
		}

		async void OnOptionsClicked(object sender, EventArgs e)
		{
			await Navigation.PushModalAsync(new NavigationPage(new Issue12928OptionsPage(this)));
		}

		Label CreateLabel() => new Label
		{
			Text = "Label with ImageBrush",
			TextColor = Colors.Purple,
			FontSize = 18,
			HeightRequest = 60,
			VerticalTextAlignment = TextAlignment.Center,
			HorizontalTextAlignment = TextAlignment.Center,
			Background = CreateImageBrush(),
			AutomationId = "ImageBrushControl1"
		};

		Button CreateButton() => new Button
		{
			Text = "Button with ImageBrush",
			HeightRequest = 60,
			Background = CreateImageBrush(),
			AutomationId = "ImageBrushControl2"
		};

		Entry CreateEntry() => new Entry
		{
			Placeholder = "Entry with ImageBrush",
			HeightRequest = 50,
			Background = CreateImageBrush(),
			AutomationId = "ImageBrushControl1"
		};

		Editor CreateEditor() => new Editor
		{
			Placeholder = "Editor with ImageBrush",
			HeightRequest = 80,
			Background = CreateImageBrush(),
			AutomationId = "ImageBrushControl1"
		};

		SearchBar CreateSearchBar() => new SearchBar
		{
			Placeholder = "SearchBar with ImageBrush",
			HeightRequest = 50,
			Background = CreateImageBrush(),
			AutomationId = "ImageBrushControl1"
		};

		Picker CreatePicker()
		{
			var picker = new Picker
			{
				Title = "Picker with ImageBrush",
				HeightRequest = 50,
				Background = CreateImageBrush(),
				AutomationId = "ImageBrushControl1"
			};
			picker.Items.Add("Option 1");
			picker.Items.Add("Option 2");
			return picker;
		}

		DatePicker CreateDatePicker() => new DatePicker
		{
			HeightRequest = 50,
			Background = CreateImageBrush(),
			AutomationId = "ImageBrushControl1"
		};

		TimePicker CreateTimePicker() => new TimePicker
		{
			HeightRequest = 50,
			Background = CreateImageBrush(),
			AutomationId = "ImageBrushControl1"
		};

		Switch CreateSwitch() => new Switch
		{
			IsToggled = false,
			HeightRequest = 60,
			Background = CreateImageBrush(),
			AutomationId = "ImageBrushControl1"
		};

		Stepper CreateStepper() => new Stepper
		{
			HeightRequest = 60,
			Background = CreateImageBrush(),
			AutomationId = "ImageBrushControl1"
		};

		Slider CreateSlider() => new Slider
		{
			Minimum = 0,
			Maximum = 100,
			Value = 50,
			HeightRequest = 60,
			Background = CreateImageBrush(),
			AutomationId = "ImageBrushControl1"
		};

		ProgressBar CreateProgressBar() => new ProgressBar
		{
			Progress = 0.6,
			HeightRequest = 40,
			Background = CreateImageBrush(),
			AutomationId = "ImageBrushControl1"
		};

		CheckBox CreateCheckBox() => new CheckBox
		{
			IsChecked = false,
			HeightRequest = 60,
			Background = CreateImageBrush(),
			AutomationId = "ImageBrushControl1"
		};

		RadioButton CreateRadioButton() => new RadioButton
		{
			Content = "RadioButton with ImageBrush",
			HeightRequest = 60,
			Background = CreateImageBrush(),
			AutomationId = "ImageBrushControl1"
		};

		Image CreateImage() => new Image
		{
			Source = "dotnet_bot.png",
			HeightRequest = 100,
			Aspect = Aspect.AspectFit,
			Background = CreateImageBrush(),
			AutomationId = "ImageBrushControl1"
		};

		ImageButton CreateImageButton() => new ImageButton
		{
			Source = "dotnet_bot.png",
			HeightRequest = 80,
			WidthRequest = 80,
			Background = CreateImageBrush(),
			AutomationId = "ImageBrushControl1"
		};

		ContentView CreateContentView() => new ContentView
		{
			HeightRequest = 80,
			Background = CreateImageBrush(),
			Content = new Label
			{
				Text = "ContentView with ImageBrush",
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			},
			AutomationId = "ImageBrushControl1"
		};

		Border CreateBorder() => new Border
		{
			HeightRequest = 120,
			StrokeThickness = 2,
			Stroke = Colors.DarkGray,
			StrokeShape = new RoundRectangle { CornerRadius = 10 },
			Background = CreateImageBrush(),
			Content = new Label
			{
				Text = "Border with ImageBrush",
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			},
			AutomationId = "ImageBrushControl1"
		};

		CollectionView CreateCollectionView() => new CollectionView
		{
			HeightRequest = 120,
			Background = CreateImageBrush(),
			ItemsSource = new[] { "Item 1", "Item 2", "Item 3" },
			AutomationId = "ImageBrushControl1"
		};

		SwipeView CreateSwipeView()
		{
			var swipeView = new SwipeView
			{
				HeightRequest = 80,
				Background = CreateImageBrush(),
				AutomationId = "ImageBrushControl1"
			};
			swipeView.Content = new Label
			{
				Text = "SwipeView with ImageBrush",
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};
			return swipeView;
		}

		CarouselView CreateCarouselView() => new CarouselView
		{
			HeightRequest = 120,
			Background = CreateImageBrush(),
			ItemsSource = new[] { "Slide 1", "Slide 2", "Slide 3" },
			ItemTemplate = new DataTemplate(() =>
			{
				var label = new Label
				{
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center
				};
				label.SetBinding(Label.TextProperty, ".");
				return label;
			}),
			AutomationId = "ImageBrushControl1"
		};

		StackLayout CreateStackLayout() => new StackLayout
		{
			HeightRequest = 120,
			Background = CreateImageBrush(),
			AutomationId = "ImageBrushControl1",
			Children =
			{
				new Label { Text = "StackLayout with ImageBrush", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center }
			}
		};

		Grid CreateGrid() => new Grid
		{
			HeightRequest = 120,
			Background = CreateImageBrush(),
			AutomationId = "ImageBrushControl1",
			Children =
			{
				new Label { Text = "Grid with ImageBrush", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center }
			}
		};

		FlexLayout CreateFlexLayout() => new FlexLayout
		{
			HeightRequest = 120,
			Background = CreateImageBrush(),
			AutomationId = "ImageBrushControl1",
			Children =
			{
				new Label { Text = "FlexLayout with ImageBrush", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center }
			}
		};

		AbsoluteLayout CreateAbsoluteLayout()
		{
			var layout = new AbsoluteLayout
			{
				HeightRequest = 120,
				Background = CreateImageBrush(),
				AutomationId = "ImageBrushControl1"
			};
			var label = new Label { Text = "AbsoluteLayout with ImageBrush" };
			AbsoluteLayout.SetLayoutBounds(label, new Rect(0.5, 0.5, -1, -1));
			AbsoluteLayout.SetLayoutFlags(label, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.PositionProportional);
			layout.Children.Add(label);
			return layout;
		}

		ScrollView CreateScrollView() => new ScrollView
		{
			HeightRequest = 120,
			Background = CreateImageBrush(),
			AutomationId = "ImageBrushControl1",
			Content = new Label
			{
				Text = "ScrollView with ImageBrush",
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			}
		};

	}

	public class Issue12928OptionsPage : ContentPage
	{
		readonly Issue12928 _mainPage;
		readonly List<string> _selectedControls = new();

		public Issue12928OptionsPage(Issue12928 mainPage)
		{
			_mainPage = mainPage;
			Title = "Select 2 Controls";

			var applyButton = new Button
			{
				Text = "Apply",
				AutomationId = "ApplyButton"
			};
			applyButton.Clicked += OnApplyClicked;

			var scrollView = new ScrollView
			{
				Content = CreateCheckBoxList()
			};

			Content = new Grid
			{
				RowDefinitions = { new RowDefinition(GridLength.Star), new RowDefinition(GridLength.Auto) },
				Children = { scrollView, applyButton }
			};
			Grid.SetRow(applyButton, 1);
		}

		Grid CreateCheckBoxList()
		{
			var grid = new Grid
			{
				Padding = 10,
				ColumnSpacing = 5,
				RowSpacing = 0,
				ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star) }
			};

			var controlNames = _mainPage.ControlNames;
			int rows = (controlNames.Count + 1) / 2;
			for (int i = 0; i < rows; i++)
				grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

			for (int i = 0; i < controlNames.Count; i++)
			{
				var controlName = controlNames[i];
				int row = i / 2;
				int col = i % 2;

				var cell = new HorizontalStackLayout { Spacing = 4 };
				var checkBox = new CheckBox
				{
					AutomationId = $"Check{controlName}",
					ScaleX = 0.8,
					ScaleY = 0.8
				};
				checkBox.CheckedChanged += (s, e) =>
				{
					if (e.Value)
						_selectedControls.Add(controlName);
					else
						_selectedControls.Remove(controlName);
				};
				var label = new Label
				{
					Text = controlName,
					FontSize = 12,
					VerticalOptions = LayoutOptions.Center
				};

				cell.Children.Add(checkBox);
				cell.Children.Add(label);
				grid.Add(cell, col, row);
			}

			return grid;
		}

		async void OnApplyClicked(object sender, EventArgs e)
		{
			if (_selectedControls.Count >= 2)
			{
				_mainPage.ShowControls(_selectedControls[0], _selectedControls[1]);
			}
			else if (_selectedControls.Count == 1)
			{
				_mainPage.ShowControls(_selectedControls[0], _selectedControls[0]);
			}

			await Navigation.PopModalAsync();
		}
	}
}
