using System;
using Google.Android.Material.Slider;

namespace Microsoft.Maui.Handlers;

public class SliderHandler2 : ViewHandler<ISlider, Slider>
{
    SliderListener? _listener;

    public static PropertyMapper<ISlider, SliderHandler2> Mapper =
            new(ViewMapper)
            {
                [nameof(ISlider.Value)] = MapValue,
                [nameof(ISlider.Minimum)] = MapMinimum,
                [nameof(ISlider.Maximum)] = MapMaximum,
                [nameof(ISlider.MinimumTrackColor)] = MapMinimumTrackColor,
                [nameof(ISlider.MaximumTrackColor)] = MapMaximumTrackColor,
                [nameof(ISlider.ThumbColor)] = MapThumbColor,
                [nameof(ISlider.ThumbImageSource)] = MapThumbImageSource,
            };

    public static CommandMapper<ISlider, SliderHandler2> CommandMapper =
            new(ViewCommandMapper);

    public SliderHandler2() : base(Mapper, CommandMapper)
    {
    }

    protected override Slider CreatePlatformView()
    {
        return new Slider(MauiMaterialContextThemeWrapper.Create(Context))
        {
            DuplicateParentStateEnabled = false,
        };
    }

    protected override void ConnectHandler(Slider platformView)
    {
        base.ConnectHandler(platformView);
        _listener = new SliderListener(this);
        platformView.AddOnChangeListener(_listener);
        platformView.AddOnSliderTouchListener(_listener);
    }

    protected override void DisconnectHandler(Slider platformView)
    {
        if (_listener is not null)
        {
            platformView.RemoveOnChangeListener(_listener);
            platformView.RemoveOnSliderTouchListener(_listener);
            _listener.Dispose();
            _listener = null;
        }
        base.DisconnectHandler(platformView);
    }

    public static void MapValue(SliderHandler2 handler, ISlider slider)
    {
        handler.PlatformView?.UpdateValue(slider);
    }

    public static void MapMinimum(SliderHandler2 handler, ISlider slider)
    {
        handler.PlatformView?.UpdateMinimum(slider);
    }

    public static void MapMaximum(SliderHandler2 handler, ISlider slider)
    {
        handler.PlatformView?.UpdateMaximum(slider);
    }

    public static void MapMinimumTrackColor(SliderHandler2 handler, ISlider slider)
    {
        handler.PlatformView?.UpdateMinimumTrackColor(slider);
    }

    public static void MapMaximumTrackColor(SliderHandler2 handler, ISlider slider)
    {
        handler.PlatformView?.UpdateMaximumTrackColor(slider);
    }

    public static void MapThumbColor(SliderHandler2 handler, ISlider slider)
    {
        handler.PlatformView?.UpdateThumbColor(slider);
    }

    public static void MapThumbImageSource(SliderHandler2 handler, ISlider slider)
    {
        var provider = handler.GetRequiredService<IImageSourceServiceProvider>();

        handler.PlatformView?.UpdateThumbImageSourceAsync(slider, provider)
            .FireAndForget(handler);
    }

    void OnValueChanged(Slider slider, float value)
    {
        if (VirtualView is null)
        {
            return;
        }

        if ((float)VirtualView.Value != value)
        {
            VirtualView.Value = value;
        }
    }

    // Uses IBaseOnChangeListener/IBaseOnSliderTouchListener (erased Object parameter) to avoid
    // ACW type erasure clash with Slider.IOnChangeListener/IOnSliderTouchListener.
    // Switch to the typed interfaces once https://github.com/dotnet/android-libraries/issues/1482
    // ships in a future Xamarin.Google.Android.Material release.
#pragma warning disable XAOBS001 // IBaseOnChangeListener/IBaseOnSliderTouchListener are marked as Google-internal
    class SliderListener : Java.Lang.Object, IBaseOnChangeListener, IBaseOnSliderTouchListener
    {
        readonly WeakReference<SliderHandler2> _handler;

        public SliderListener(SliderHandler2 handler)
        {
            _handler = new WeakReference<SliderHandler2>(handler);
        }

        public void OnValueChange(Java.Lang.Object slider, float value, bool fromUser)
        {
            if (fromUser && slider is Slider platformSlider && _handler.TryGetTarget(out var handler))
            {
                handler.OnValueChanged(platformSlider, value);
            }
        }

        public void OnStartTrackingTouch(Java.Lang.Object slider)
        {
            if (_handler.TryGetTarget(out var handler))
            {
                handler.VirtualView?.DragStarted();
            }
        }

        public void OnStopTrackingTouch(Java.Lang.Object slider)
        {
            if (_handler.TryGetTarget(out var handler))
            {
                handler.VirtualView?.DragCompleted();
            }
        }
    }
#pragma warning restore XAOBS001
}
