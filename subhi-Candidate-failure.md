# PR #35543 — Test Failure Analysis: `TabbedPageGradientBrushSubscriberRemovedAfterModalPop` on Windows

## PR Overview

**Title:** [Android & iOS] TabbedPage leaks with shared GradientBrush  
**PR:** https://github.com/dotnet/maui/pull/35543  
**Issue Fixed:** https://github.com/dotnet/maui/issues/35469  
**Status:** Merged into `inflight/current`

The PR fixes a memory leak where `TabbedPage` (and `Toolbar` on Android) failed to unsubscribe from `GradientBrush.InvalidateGradientBrushRequested` when the page was removed. Changes were made to:

- `TabbedRenderer.cs` → iOS `Dispose()`
- `TabbedPageManager.cs` → Android `SetElement()`
- `Toolbar.Android.cs` → Android `OnHandlerChanging()`

---

## Failing Test

**Test:** `TabbedPageGradientBrushSubscriberRemovedAfterModalPop`  
**Platform:** Windows  
**Failure point:**

```csharp
Assert.Single(GetGradientBrushInvocationList(sharedBrush)); // list is EMPTY
```

This assertion fires **while the TabbedPage is still live** (after `PushModalAsync`, before `PopModalAsync`).

---

## Root Cause

The Windows `TabbedPage.MapBarBackground` implementation **never subscribed** to `GradientBrush.InvalidateGradientBrushRequested`:

```csharp
// Original — TabbedPage.Windows.cs
internal static void MapBarBackground(ITabbedViewHandler handler, TabbedPage view)
{
    // Just applies the value — NO += subscription
    view._navigationView?.UpdateTopNavAreaBackground(view.BarBackground ?? view.BarBackgroundColor?.AsPaint());
}
```

Since no subscription (`+=`) was ever made, the invocation list is always empty on Windows. The test's `Assert.Single` check expects exactly 1 subscriber while the page is live — it fails immediately with an empty list.

---

## Why iOS & Android Had the Wiring But Windows Did Not

| | iOS / Android | Windows |
|---|---|---|
| Architecture | Old compatibility renderer | New handler pattern |
| Painting | Manual native painting (UITabBar / BottomNavigationView) | WinUI NavigationView + XAML brushes |
| Why subscribed | Must know when brush changes to **manually repaint** native view | WinUI handles repaints — subscription was skipped |
| Result | Subscription exists → PR's `-=` fix was needed | No subscription → `Assert.Single` fails |

iOS/Android needed the subscription for **functional correctness** (manual repaint on brush change). Windows skipped it assuming WinUI's brush system handled updates natively, so the leak-fix PR never addressed Windows.

---

## Why the Other Two Tests Pass on Windows

The other tests added by the PR only use `Assert.Empty` after disconnect/GC:

```csharp
Assert.Empty(brushInvocationList); // only assertion on the brush
```

Since Windows **never subscribed**, the invocation list is always `[]`. `Assert.Empty` is trivially true — the tests pass, but for the **wrong reason** (vacuously correct, not because the fix worked).

| Test | Assertions | Windows Result | Reason |
|------|-----------|---------------|--------|
| `TabbedPageDoesNotLeakGradientBrushSubscriberOnDisconnect` | `Assert.Empty` after GC | ✅ Passes | Never subscribed → always empty |
| `TabbedPageDoesNotLeakGradientBrushSubscriberWhenSetViaStyle` | `Assert.Empty` after GC | ✅ Passes | Never subscribed → always empty |
| `TabbedPageGradientBrushSubscriberRemovedAfterModalPop` | `Assert.Single` while live + `Assert.Empty` after pop | ❌ Fails at `Assert.Single` | Never subscribed → list is empty |

---

## About `GetGradientBrushInvocationList`

```csharp
static System.Delegate[] GetGradientBrushInvocationList(GradientBrush brush)
{
    var field = typeof(GradientBrush).GetField(
        "InvalidateGradientBrushRequested",
        BindingFlags.Instance | BindingFlags.NonPublic);

    if (field is null)
        return [];

    return (field.GetValue(brush) as MulticastDelegate)?.GetInvocationList() ?? [];
}
```

`InvalidateGradientBrushRequested` is a **field-like event** in C#:
```csharp
public event EventHandler InvalidateGradientBrushRequested;
```
The compiler generates a **private backing field** with the same name. Since the event only exposes `add`/`remove` publicly, reflection with `BindingFlags.NonPublic` is the only way to inspect the invocation list (count active subscribers) from outside the class.

Each element in `GetInvocationList()` = one subscriber. The tests use this to verify:
- `Assert.Single` → exactly 1 subscriber (renderer/manager is subscribed while page is live)
- `Assert.Empty` → 0 subscribers (unsubscribed after pop = no leak)

---

## Fix Applied (Local, Not Committed)

**File:** `src/Controls/src/Core/TabbedPage/TabbedPage.Windows.cs`

### Changes

1. **Added fields** to track subscription state:
```csharp
Brush? _currentBarBackground;
bool _barBackgroundSubscribed;
```

2. **Added `OnBarBackgroundChanged`** — re-applies background when brush content changes:
```csharp
void OnBarBackgroundChanged(object? sender, EventArgs e)
{
    _navigationView?.UpdateTopNavAreaBackground(_currentBarBackground ?? BarBackgroundColor?.AsPaint());
}
```

3. **Updated `MapBarBackground`** — subscribe to new brush, unsubscribe from old:
```csharp
internal static void MapBarBackground(ITabbedViewHandler handler, TabbedPage view)
{
    if (view._currentBarBackground is GradientBrush oldGradientBrush && view._barBackgroundSubscribed)
    {
        oldGradientBrush.InvalidateGradientBrushRequested -= view.OnBarBackgroundChanged;
        view._barBackgroundSubscribed = false;
    }

    view._currentBarBackground = view.BarBackground;

    if (view._currentBarBackground is GradientBrush newGradientBrush)
    {
        newGradientBrush.InvalidateGradientBrushRequested += view.OnBarBackgroundChanged;
        view._barBackgroundSubscribed = true;
    }

    view._navigationView?.UpdateTopNavAreaBackground(view.BarBackground ?? view.BarBackgroundColor?.AsPaint());
}
```

4. **Updated `OnTabbedPageDisappearing`** — unsubscribes on modal pop (key fix):
```csharp
if (_currentBarBackground is GradientBrush gradientBrush && _barBackgroundSubscribed)
{
    gradientBrush.InvalidateGradientBrushRequested -= OnBarBackgroundChanged;
    _barBackgroundSubscribed = false;
}
```

5. **Updated `OnTabbedPageAppearing`** — re-subscribes if page reappears:
```csharp
if (_currentBarBackground is GradientBrush gradientBrush && !_barBackgroundSubscribed)
{
    gradientBrush.InvalidateGradientBrushRequested += OnBarBackgroundChanged;
    _barBackgroundSubscribed = true;
}
```

6. **Updated `OnHandlerDisconnected`** — final safety-net cleanup:
```csharp
if (_currentBarBackground is GradientBrush currentGradientBrush && _barBackgroundSubscribed)
{
    currentGradientBrush.InvalidateGradientBrushRequested -= OnBarBackgroundChanged;
    _barBackgroundSubscribed = false;
}
_currentBarBackground = null;
```

### Test Flow After Fix

| Step | Action | State |
|------|--------|-------|
| 1 | `PushModalAsync` → `MapBarBackground` called | `_barBackgroundSubscribed = true`, count = 1 |
| 2 | `Assert.Single(...)` | ✅ Passes |
| 3 | `PopModalAsync` → `Disappearing` fires → `OnTabbedPageDisappearing` | `_barBackgroundSubscribed = false`, count = 0 |
| 4 | `Assert.Empty(...)` | ✅ Passes |
