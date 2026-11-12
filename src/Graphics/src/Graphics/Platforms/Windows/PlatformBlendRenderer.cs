using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using WSize = global::Windows.Foundation.Size;
#if NETFX_CORE
using WColors = global::Windows.UI.Colors;
#else
using WColors = global::Microsoft.UI.Colors;
#endif

#if MAUI_GRAPHICS_WIN2D
namespace Microsoft.Maui.Graphics.Win2D
#else
namespace Microsoft.Maui.Graphics.Platform
#endif
{
	internal sealed class PlatformBlendRenderer : IDisposable
	{
		readonly CanvasDrawingSession _target;
		readonly WSize _size;
		readonly List<CanvasActiveLayer> _layers = new List<CanvasActiveLayer>();
		readonly List<ICanvasImage> _effects = new List<ICanvasImage>();
		CanvasCommandList _recording;
		CanvasDrawingSession _recordingSession;
		CanvasRenderTarget _destination;
		CanvasRenderTarget _source;
		CanvasRenderTarget _output;
		CanvasRenderTarget _coverage;
		CanvasSolidColorBrush _coverageBrush;

		internal PlatformBlendRenderer(CanvasDrawingSession target, WSize size)
		{
			_target = target;
			_size = size;
			_recording = new CanvasCommandList(target);
			_recordingSession = _recording.CreateDrawingSession();
		}

		internal void Draw(Action<CanvasDrawingSession> draw, Action<CanvasDrawingSession, ICanvasBrush> drawCoverage, PlatformCanvasState state)
		{
			if (_destination is null && state.BlendMode == BlendMode.Normal && !state.IsShadowed && !state.IsBlurred)
			{
				draw(_target);
				DrawClipped(_recordingSession, draw, state);
				return;
			}

			// Rasterize effects immediately: their input bitmap is reused by later draws.
			EnsureDestination();
			if (state.BlendMode == BlendMode.Normal)
			{
				using var session = _destination.CreateDrawingSession();
				DrawClipped(session, draw, state);
				return;
			}

			_source ??= CreateTarget();
			Render(_source, draw, state);

			try
			{
				ICanvasImage result;
				if (GetBlendEffect(state.BlendMode) is BlendEffectMode blend)
				{
					result = Track(new BlendEffect { Background = _destination, Foreground = _source, Mode = blend });
				}
				else
				{
					bool needsCoverage = NeedsCoverage(state.BlendMode);
					if (needsCoverage)
					{
						_coverage ??= CreateTarget();
						_coverageBrush ??= new CanvasSolidColorBrush(_target, WColors.White);
						Render(_coverage, session => drawCoverage(session, _coverageBrush), state);
					}

					result = state.BlendMode switch
					{
						BlendMode.Clear => Composite(CanvasComposite.DestinationOut, _destination, _coverage),
						BlendMode.Copy => _source,
						BlendMode.SourceIn => Composite(CanvasComposite.SourceIn, _destination, _source),
						BlendMode.SourceOut => Composite(CanvasComposite.SourceOut, _destination, _source),
						BlendMode.SourceAtop => Composite(CanvasComposite.SourceAtop, _destination, _source),
						BlendMode.DestinationOver => Composite(CanvasComposite.DestinationOver, _destination, _source),
						BlendMode.DestinationIn => Composite(CanvasComposite.DestinationIn, _destination, _source),
						BlendMode.DestinationOut => Composite(CanvasComposite.DestinationOut, _destination, _source),
						BlendMode.DestinationAtop => Composite(CanvasComposite.DestinationAtop, _destination, _source),
						BlendMode.Xor => Composite(CanvasComposite.Xor, _destination, _source),
						BlendMode.PlusLighter => Composite(CanvasComposite.Add, _destination, _source),
						BlendMode.PlusDarker => PlusDarker(),
						_ => throw new ArgumentOutOfRangeException(nameof(state.BlendMode), state.BlendMode, null)
					};

					if (needsCoverage && state.BlendMode != BlendMode.Clear)
					{
						// Primitive drawing must leave pixels outside the geometry unchanged.
						var outside = Composite(CanvasComposite.DestinationOut, _destination, _coverage);
						result = Composite(CanvasComposite.Add, outside, result);
					}
				}

				_output ??= CreateTarget();
				using (var session = _output.CreateDrawingSession())
				{
					session.Clear(WColors.Transparent);
					session.DrawImage(result);
				}
				(_destination, _output) = (_output, _destination);
			}
			finally
			{
				foreach (var effect in _effects)
				{
					effect.Dispose();
				}
				_effects.Clear();
			}
		}

		static bool NeedsCoverage(BlendMode mode) =>
			mode == BlendMode.Clear || mode == BlendMode.Copy ||
			mode == BlendMode.SourceIn || mode == BlendMode.SourceOut ||
			mode == BlendMode.DestinationIn || mode == BlendMode.DestinationAtop ||
			mode == BlendMode.PlusDarker;

		ICanvasImage PlusDarker()
		{
			var destinationInside = Composite(CanvasComposite.SourceIn, _coverage, _destination);
			var sum = Arithmetic(_source, destinationInside, 1, 1, false);
			var excessAlpha = Arithmetic(AlphaImage(sum), AlphaImage(_coverage), 1, -1, true);
			// Subtract max(0, As + Ad - 1) from premultiplied RGBA, not min(S, D).
			return Arithmetic(sum, excessAlpha, 1, -1, true);
		}

		ColorMatrixEffect AlphaImage(ICanvasImage source) => Track(new ColorMatrixEffect
		{
			Source = source,
			AlphaMode = CanvasAlphaMode.Premultiplied,
			ColorMatrix = new Matrix5x4 { M41 = 1, M42 = 1, M43 = 1, M44 = 1 },
			BufferPrecision = CanvasBufferPrecision.Precision32Float
		});

		ArithmeticCompositeEffect Arithmetic(ICanvasImage first, ICanvasImage second, float firstAmount, float secondAmount, bool clamp) =>
			Track(new ArithmeticCompositeEffect
			{
				Source1 = first,
				Source2 = second,
				MultiplyAmount = 0,
				Source1Amount = firstAmount,
				Source2Amount = secondAmount,
				ClampOutput = clamp,
				BufferPrecision = CanvasBufferPrecision.Precision32Float
			});

		CompositeEffect Composite(CanvasComposite mode, ICanvasImage background, ICanvasImage foreground) =>
			Track(new CompositeEffect { Mode = mode, Sources = { background, foreground } });

		T Track<T>(T effect) where T : ICanvasImage
		{
			_effects.Add(effect);
			return effect;
		}

		static BlendEffectMode? GetBlendEffect(BlendMode mode) => mode switch
		{
			BlendMode.Multiply => BlendEffectMode.Multiply,
			BlendMode.Screen => BlendEffectMode.Screen,
			BlendMode.Overlay => BlendEffectMode.Overlay,
			BlendMode.Darken => BlendEffectMode.Darken,
			BlendMode.Lighten => BlendEffectMode.Lighten,
			BlendMode.ColorDodge => BlendEffectMode.ColorDodge,
			BlendMode.ColorBurn => BlendEffectMode.ColorBurn,
			BlendMode.SoftLight => BlendEffectMode.SoftLight,
			BlendMode.HardLight => BlendEffectMode.HardLight,
			BlendMode.Difference => BlendEffectMode.Difference,
			BlendMode.Exclusion => BlendEffectMode.Exclusion,
			BlendMode.Hue => BlendEffectMode.Hue,
			BlendMode.Saturation => BlendEffectMode.Saturation,
			BlendMode.Color => BlendEffectMode.Color,
			BlendMode.Luminosity => BlendEffectMode.Luminosity,
			_ => null
		};

		CanvasRenderTarget CreateTarget()
		{
			if (_size.Width <= 0 || _size.Height <= 0 || double.IsNaN(_size.Width) || double.IsNaN(_size.Height) ||
				double.IsInfinity(_size.Width) || double.IsInfinity(_size.Height))
			{
				throw new InvalidOperationException("Set CanvasSize to a finite, positive size before drawing with blend modes.");
			}

			return new CanvasRenderTarget(_target, _size);
		}

		void EnsureDestination()
		{
			if (_destination is not null)
			{
				return;
			}

			_recordingSession.Dispose();
			_recordingSession = null;
			_destination = CreateTarget();
			using (var session = _destination.CreateDrawingSession())
			{
				session.Clear(WColors.Transparent);
				session.DrawImage(_recording);
			}
			_recording.Dispose();
			_recording = null;
		}

		void Render(CanvasRenderTarget target, Action<CanvasDrawingSession> draw, PlatformCanvasState state)
		{
			using var session = target.CreateDrawingSession();
			session.Clear(WColors.Transparent);
			DrawClipped(session, draw, state);
		}

		void DrawClipped(CanvasDrawingSession session, Action<CanvasDrawingSession> draw, PlatformCanvasState state)
		{
			session.Transform = _target.Transform;
			session.Antialiasing = _target.Antialiasing;
			session.TextAntialiasing = _target.TextAntialiasing;
			session.TextRenderingParameters = _target.TextRenderingParameters;
			try
			{
				state.ApplyClipLayers(session, _layers);
				draw(session);
			}
			finally
			{
				for (int i = _layers.Count - 1; i >= 0; i--)
				{
					_layers[i].Dispose();
				}
				_layers.Clear();
			}
		}

		public void Dispose()
		{
			try
			{
				if (_destination is not null)
				{
					var transform = _target.Transform;
					var blend = _target.Blend;
					try
					{
						_target.Transform = Matrix3x2.Identity;
						_target.Blend = CanvasBlend.Copy;
						_target.DrawImage(_destination);
					}
					finally
					{
						_target.Transform = transform;
						_target.Blend = blend;
					}
				}
			}
			finally
			{
				_recordingSession?.Dispose();
				_recording?.Dispose();
				_destination?.Dispose();
				_source?.Dispose();
				_output?.Dispose();
				_coverage?.Dispose();
				_coverageBrush?.Dispose();
			}
		}
	}
}
