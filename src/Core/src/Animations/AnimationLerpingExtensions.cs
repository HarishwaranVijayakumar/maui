using System;
using Microsoft.Maui.Graphics;
namespace Microsoft.Maui.Animations
{
	/// <summary>
	/// Provides linear interpolation (lerp) extension methods for common types used in animations.
	/// Progress values are typically between 0.0 and 1.0, but are not clamped — values outside this range will extrapolate.
	/// </summary>
	public static class AnimationLerpingExtensions
	{
		/// <summary>
		/// Linearly interpolates between two <see cref="Color"/> values.
		/// </summary>
		/// <param name="color">The start color.</param>
		/// <param name="endColor">The end color.</param>
		/// <param name="progress">The interpolation progress.</param>
		/// <returns>The interpolated <see cref="Color"/>.</returns>
		/// <remarks>If <paramref name="color"/> or <paramref name="endColor"/> is <see langword="null"/>, <see cref="Colors.Black"/> is used as the default.</remarks>
		public static Color Lerp(this Color color, Color endColor, double progress)
		{
			color ??= Colors.Black;
			endColor ??= Colors.Black;
			float Lerp(float start, float end, double progress) => (float)(((end - start) * progress) + start);

			var r = Lerp(color.Red, endColor.Red, progress);
			var b = Lerp(color.Blue, endColor.Blue, progress);
			var g = Lerp(color.Green, endColor.Green, progress);
			var a = Lerp(color.Alpha, endColor.Alpha, progress);
			return new Color(r, g, b, a);
		}

		/// <summary>
		/// Linearly interpolates between two <see cref="SizeF"/> values.
		/// </summary>
		/// <param name="start">The start size.</param>
		/// <param name="end">The end size.</param>
		/// <param name="progress">The interpolation progress.</param>
		/// <returns>The interpolated <see cref="SizeF"/>.</returns>
		public static SizeF Lerp(this SizeF start, SizeF end, double progress) =>
			new SizeF(start.Width.Lerp(end.Width, progress), start.Height.Lerp(end.Height, progress));

		/// <summary>
		/// Linearly interpolates between two <see cref="PointF"/> values.
		/// </summary>
		/// <param name="start">The start point.</param>
		/// <param name="end">The end point.</param>
		/// <param name="progress">The interpolation progress.</param>
		/// <returns>The interpolated <see cref="PointF"/>.</returns>
		public static PointF Lerp(this PointF start, PointF end, double progress) =>
			new PointF(start.X.Lerp(end.X, progress), start.Y.Lerp(end.Y, progress));

		/// <summary>
		/// Linearly interpolates between two <see cref="RectF"/> values.
		/// </summary>
		/// <param name="start">The start rectangle.</param>
		/// <param name="end">The end rectangle.</param>
		/// <param name="progress">The interpolation progress.</param>
		/// <returns>The interpolated <see cref="RectF"/>.</returns>
		public static RectF Lerp(this RectF start, RectF end, double progress)
			=> new RectF(start.Location.Lerp(end.Location, progress), start.Size.Lerp(end.Size, progress));

		/// <summary>
		/// Linearly interpolates between two <see cref="Size"/> values.
		/// </summary>
		/// <param name="start">The start size.</param>
		/// <param name="end">The end size.</param>
		/// <param name="progress">The interpolation progress.</param>
		/// <returns>The interpolated <see cref="Size"/>.</returns>
		public static Size Lerp(this Size start, Size end, double progress) =>
			new Size(start.Width.Lerp(end.Width, progress), start.Height.Lerp(end.Height, progress));

		/// <summary>
		/// Linearly interpolates between two <see cref="Point"/> values.
		/// </summary>
		/// <param name="start">The start point.</param>
		/// <param name="end">The end point.</param>
		/// <param name="progress">The interpolation progress.</param>
		/// <returns>The interpolated <see cref="Point"/>.</returns>
		public static Point Lerp(this Point start, Point end, double progress) =>
			new Point(start.X.Lerp(end.X, progress), start.Y.Lerp(end.Y, progress));

		/// <summary>
		/// Linearly interpolates between two <see cref="Rect"/> values.
		/// </summary>
		/// <param name="start">The start rectangle.</param>
		/// <param name="end">The end rectangle.</param>
		/// <param name="progress">The interpolation progress.</param>
		/// <returns>The interpolated <see cref="Rect"/>.</returns>
		public static Rect Lerp(this Rect start, Rect end, double progress)
			=> new Rect(start.Location.Lerp(end.Location, progress), start.Size.Lerp(end.Size, progress));

		/// <summary>
		/// Linearly interpolates between two <see cref="float"/> values.
		/// </summary>
		/// <param name="start">The start value.</param>
		/// <param name="end">The end value.</param>
		/// <param name="progress">The interpolation progress.</param>
		/// <returns>The interpolated value.</returns>
		public static float Lerp(this float start, float end, double progress) =>
			(float)((end - start) * progress) + start;

		/// <summary>
		/// Linearly interpolates between two <see cref="double"/> values.
		/// </summary>
		/// <param name="start">The start value.</param>
		/// <param name="end">The end value.</param>
		/// <param name="progress">The interpolation progress.</param>
		/// <returns>The interpolated value.</returns>
		public static double Lerp(this double start, double end, double progress) =>
			((end - start) * progress) + start;

		/// <summary>
		/// Linearly interpolates between two nullable <see cref="float"/> values.
		/// If both values are non-null, performs linear interpolation; otherwise toggles at the halfway point.
		/// </summary>
		/// <param name="start">The start value.</param>
		/// <param name="end">The end value.</param>
		/// <param name="progress">The interpolation progress.</param>
		/// <returns>The interpolated value.</returns>
		public static float? Lerp(this float? start, float? end, double progress)
			=> start.HasValue && end.HasValue ? start.Value.Lerp(end.Value, progress) : start.GenericLerp(end, progress);

		/// <summary>
		/// Performs a generic lerp by toggling between <paramref name="start"/> and <paramref name="end"/> at the specified threshold.
		/// </summary>
		/// <typeparam name="T">The type of the values.</typeparam>
		/// <param name="start">The start value.</param>
		/// <param name="end">The end value.</param>
		/// <param name="progress">The interpolation progress.</param>
		/// <param name="toggleThreshold">The progress threshold at which to switch from start to end. Default is 0.5.</param>
		/// <returns>The start value if progress is below the threshold; otherwise the end value.</returns>
		public static T GenericLerp<T>(this T start, T end, double progress, double toggleThreshold = .5)
			=> progress < toggleThreshold ? start : end;

		/// <summary>
		/// Linearly interpolates between two <see cref="Thickness"/> values.
		/// </summary>
		/// <param name="start">The start thickness.</param>
		/// <param name="end">The end thickness.</param>
		/// <param name="progress">The interpolation progress.</param>
		/// <returns>The interpolated <see cref="Thickness"/>.</returns>
		public static Thickness Lerp(this Thickness start, Thickness end, double progress)
			=> new Thickness(
				start.Left.Lerp(end.Left, progress),
				start.Top.Lerp(end.Top, progress),
				start.Right.Lerp(end.Right, progress),
				start.Bottom.Lerp(end.Bottom, progress)
				);

		/// <summary>
		/// Linearly interpolates between two <see cref="SolidPaint"/> values by interpolating their colors.
		/// </summary>
		/// <param name="paint">The start paint.</param>
		/// <param name="endPaint">The end paint.</param>
		/// <param name="progress">The interpolation progress.</param>
		/// <returns>A new <see cref="SolidPaint"/> with the interpolated color.</returns>
		/// <remarks>If <paramref name="paint"/> or <paramref name="endPaint"/> is <see langword="null"/>, <see cref="Colors.Black"/> is used as the default color.</remarks>
		public static SolidPaint Lerp(this SolidPaint paint, SolidPaint endPaint, double progress)
		{
			var color = paint?.Color ?? Colors.Black;
			var endColor = endPaint?.Color ?? Colors.Black;
			return new SolidPaint(color.Lerp(endColor, progress));
		}
	}
}
