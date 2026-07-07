using Android.Graphics.Drawables;
using AndroidX.AppCompat.Widget;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Platform
{
	public static class RadioButtonExtensions
	{
		public static void UpdateBackground(this AppCompatRadioButton platformRadioButton, IRadioButton radioButton)
		{
			platformRadioButton.UpdateBorderDrawable(radioButton);
		}

		public static void UpdateIsChecked(this AppCompatRadioButton platformRadioButton, IRadioButton radioButton)
		{
			platformRadioButton.Checked = radioButton.IsChecked;
		}

		public static void UpdateContent(this AppCompatRadioButton platformRadioButton, IRadioButton radioButton)
		{
			platformRadioButton.Text = $"{radioButton.Content}";
		}

		public static void UpdateStrokeColor(this AppCompatRadioButton platformRadioButton, IRadioButton radioButton)
		{
			platformRadioButton.UpdateBorderDrawable(radioButton);
		}

		public static void UpdateStrokeThickness(this AppCompatRadioButton platformRadioButton, IRadioButton radioButton)
		{
			platformRadioButton.UpdateBorderDrawable(radioButton);
		}

		public static void UpdateCornerRadius(this AppCompatRadioButton platformRadioButton, IRadioButton radioButton)
		{
			platformRadioButton.UpdateBorderDrawable(radioButton);
		}

		internal static void UpdateBorderDrawable(this AppCompatRadioButton platformView, IRadioButton radioButton)
		{
			BorderDrawable? mauiDrawable = platformView.GetBorderDrawable();

			if (mauiDrawable is null)
			{
				mauiDrawable = new BorderDrawable(platformView.Context);
				platformView.Background = mauiDrawable;
			}

			if (radioButton.Background is ImageSourcePaint sourcePaint)
			{
				mauiDrawable.SetBackground(new SolidPaint(Colors.Transparent));
				platformView.UpdateBorderImageBackground(sourcePaint.ImageSource, radioButton.Handler, mauiDrawable);
			}
			else
			{
				// Remove LayerDrawable wrapper if switching away from image
				if (platformView.Background is LayerDrawable)
				{
					platformView.Background = mauiDrawable;
				}

				mauiDrawable.SetBackground(radioButton.Background);
			}

			if (radioButton.StrokeColor is not null)
			{
				mauiDrawable.SetBorderBrush(new SolidPaint { Color = radioButton.StrokeColor });
			}

			if (radioButton.StrokeThickness > 0)
			{
				mauiDrawable.SetBorderWidth(radioButton.StrokeThickness);
			}

			if (radioButton.CornerRadius > 0)
			{
				mauiDrawable.SetCornerRadius(radioButton.CornerRadius);
			}
		}

		static BorderDrawable? GetBorderDrawable(this AppCompatRadioButton platformView)
		{
			if (platformView.Background is BorderDrawable borderDrawable)
			{
				return borderDrawable;
			}

			if (platformView.Background is LayerDrawable layerDrawable)
			{
				for (int i = 0; i < layerDrawable.NumberOfLayers; i++)
				{
					if (layerDrawable.GetDrawable(i) is BorderDrawable innerDrawable)
					{
						return innerDrawable;
					}
				}
			}

			return null;
		}
	}
}