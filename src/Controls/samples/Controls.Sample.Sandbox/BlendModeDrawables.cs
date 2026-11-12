namespace Maui.Controls.Sample;

class BlendModeDrawable(BlendMode mode) : IDrawable
{
	public string Explanation { get; } = mode switch
	{
		BlendMode.Normal => "Normal: the translucent orange source is drawn over the blue destination. The overlap mixes both colors.",
		BlendMode.Multiply => "Multiply: multiplies the color channels. Look for a darker overlap; the source-only area remains orange.",
		BlendMode.Screen => "Screen: multiplies the inverse color channels, producing a lighter overlap than Normal.",
		BlendMode.Overlay => "Overlay: dark destination channels are multiplied and light channels are screened, increasing contrast.",
		BlendMode.Darken => "Darken: chooses the darker source or destination value for each color channel in the overlap.",
		BlendMode.Lighten => "Lighten: chooses the lighter source or destination value for each color channel in the overlap.",
		BlendMode.ColorDodge => "ColorDodge: brightens the blue destination according to the orange source; highlights can reach white.",
		BlendMode.ColorBurn => "ColorBurn: darkens the destination according to the source, producing a strong, dark overlap.",
		BlendMode.SoftLight => "SoftLight: gently lightens or darkens the destination according to the source, like a soft spotlight.",
		BlendMode.HardLight => "HardLight: multiplies or screens based on the source channels, creating stronger contrast than SoftLight.",
		BlendMode.Difference => "Difference: takes the absolute difference of the color channels. Similar channels become dark.",
		BlendMode.Exclusion => "Exclusion: creates a difference-like overlap with lower contrast and more muted colors.",
		BlendMode.Hue => "Hue: takes the orange source's hue while retaining the blue destination's saturation and luminosity.",
		BlendMode.Saturation => "Saturation: takes the source's saturation while retaining the destination's blue hue and luminosity.",
		BlendMode.Color => "Color: takes the source's hue and saturation while retaining the destination's luminosity.",
		BlendMode.Luminosity => "Luminosity: takes the source's luminosity while retaining the destination's blue hue and saturation.",
		BlendMode.Clear => "Clear: erases the destination under the circle. A cutout showing the background is expected, not an orange circle or a blank canvas.",
		BlendMode.Copy => "Copy: replaces the destination under the circle with the source, including its transparency. Unlike Normal, blue does not mix into the overlap.",
		BlendMode.SourceIn => "SourceIn: keeps orange source pixels only where the blue destination exists. The source-only part of the circle is transparent.",
		BlendMode.SourceOut => "SourceOut: keeps orange source pixels only outside the destination. The overlap becomes transparent.",
		BlendMode.SourceAtop => "SourceAtop: draws orange over blue only in the overlap, preserving the destination's opacity. No orange extends beyond the square.",
		BlendMode.DestinationOver => "DestinationOver: places the source behind the destination. The blue square covers the orange circle in the overlap.",
		BlendMode.DestinationIn => "DestinationIn: keeps destination color according to source opacity. The blue overlap becomes translucent; the orange source itself is not painted.",
		BlendMode.DestinationOut => "DestinationOut: removes destination color according to source opacity. The 75%-opaque circle leaves a faint blue overlap rather than a fully erased hole.",
		BlendMode.DestinationAtop => "DestinationAtop: puts destination over source using the source's opacity. The overlap stays blue but becomes translucent; the source-only area is orange.",
		BlendMode.Xor => "Xor: excludes overlapping contributions. With this translucent source, the overlap retains faint blue; outside it, the separate colors remain.",
		BlendMode.PlusDarker => "PlusDarker: adds colors and subtracts white, clamping at black. These non-primary input colors leave a visible dark overlap.",
		BlendMode.PlusLighter => "PlusLighter: adds source and destination contributions, clamping at white. Look for a bright overlap.",
		_ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
	};

	public void Draw(ICanvas canvas, RectF dirtyRect)
	{
		canvas.SaveState();
		try
		{
			float size = Math.Min(dirtyRect.Width / 3, dirtyRect.Height / 4);
			float centerX = dirtyRect.Center.X;
			float centerY = dirtyRect.Y + dirtyRect.Height * 0.65f;
			var destination = new Color(0.25f, 0.6f, 0.85f);
			var source = new Color(0.95f, 0.45f, 0.3f, 0.75f);

			// The view background must not become an opaque compositing destination.
			canvas.Alpha = 1;
			canvas.BlendMode = BlendMode.Normal;
			canvas.FillColor = destination;
			canvas.FillRectangle(centerX - size, centerY - size * 0.6f, size * 1.2f, size * 1.2f);

			canvas.BlendMode = mode;
			canvas.FillColor = source;
			canvas.FillCircle(centerX + size * 0.35f, centerY, size * 0.65f);

			// Draw references last so destination-sensitive modes cannot erase them.
			canvas.BlendMode = BlendMode.Normal;
			float previewSize = size * 0.45f;
			float previewY = dirtyRect.Y + 32;
			canvas.FillColor = destination;
			canvas.FillRectangle(centerX - size, previewY, previewSize, previewSize);
			canvas.FillColor = source;
			canvas.FillCircle(centerX + size * 0.65f, previewY + previewSize / 2, previewSize / 2);

			canvas.FontColor = Colors.Black;
			canvas.FontSize = 12;
			canvas.DrawString("Destination", centerX - size, previewY - 10,
				Microsoft.Maui.Graphics.HorizontalAlignment.Left);
			canvas.DrawString("Source", centerX + size * 0.65f, previewY - 10,
				Microsoft.Maui.Graphics.HorizontalAlignment.Center);
		}
		finally
		{
			canvas.RestoreState();
		}
	}
}
