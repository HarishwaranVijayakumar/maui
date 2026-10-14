#nullable disable
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WASDKListViewSelectionMode = Microsoft.UI.Xaml.Controls.ListViewSelectionMode;
using WASDKSelectionChangedEventArgs = Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs;

namespace Microsoft.Maui.Controls.Handlers.Items
{
	public partial class SelectableItemsViewHandler<TItemsView> : StructuredItemsViewHandler<TItemsView> where TItemsView : SelectableItemsView
	{
		bool _ignorePlatformSelectionChange;
		bool _ignoreVirtualSelectionChange;
		long _selectionModePropertyChangedToken;

		protected override void ConnectHandler(ListViewBase platformView)
		{
			base.ConnectHandler(platformView);

			if (ItemsView != null)
			{
				ItemsView.SelectionChanged += VirtualSelectionChanged;
			}

			var newListViewBase = ListViewBase;

			if (newListViewBase != null)
			{
				// WinUI bindings to managed properties require generated custom-property support under NativeAOT.
				UpdateValue(nameof(SelectableItemsView.SelectionMode));
				_selectionModePropertyChangedToken = newListViewBase.RegisterPropertyChangedCallback(
					ListViewBase.SelectionModeProperty, PlatformSelectionModeChanged);
				newListViewBase.SelectionChanged += PlatformSelectionChanged;
			}

			UpdatePlatformSelection();
		}

		protected override void DisconnectHandler(ListViewBase platformView)
		{
			var oldListViewBase = platformView;

			if (oldListViewBase != null)
			{
				oldListViewBase.SelectionChanged -= PlatformSelectionChanged;
				oldListViewBase.UnregisterPropertyChangedCallback(
					ListViewBase.SelectionModeProperty, _selectionModePropertyChangedToken);
				oldListViewBase.ClearValue(ListViewBase.SelectionModeProperty);
			}

			if (ItemsView != null)
			{
				ItemsView.SelectionChanged -= VirtualSelectionChanged;
			}

			base.DisconnectHandler(platformView);
		}

		public static void MapSelectedItem(SelectableItemsViewHandler<TItemsView> handler, SelectableItemsView itemsView)
		{
		}

		public static void MapSelectedItems(SelectableItemsViewHandler<TItemsView> handler, SelectableItemsView itemsView)
		{
		}

		public static void MapSelectionMode(SelectableItemsViewHandler<TItemsView> handler, SelectableItemsView itemsView)
		{
			if (((IElementHandler)handler).VirtualView is null ||
				((IElementHandler)handler).PlatformView is not ListViewBase platformView)
			{
				return;
			}

			var previousIgnorePlatformSelectionChange = handler._ignorePlatformSelectionChange;
			handler._ignorePlatformSelectionChange = true;

			try
			{
				// Changing the native mode can clear its selection; do not write that transient state back to MAUI.
				platformView.SelectionMode = itemsView.SelectionMode switch
				{
					SelectionMode.Single => WASDKListViewSelectionMode.Single,
					SelectionMode.Multiple => WASDKListViewSelectionMode.Multiple,
					_ => WASDKListViewSelectionMode.None
				};

				handler.UpdatePlatformSelection();
			}
			finally
			{
				handler._ignorePlatformSelectionChange = previousIgnorePlatformSelectionChange;
			}
		}

		void PlatformSelectionModeChanged(DependencyObject sender, DependencyProperty property)
		{
			if (((IElementHandler)this).VirtualView is not SelectableItemsView itemsView)
			{
				return;
			}

			var selectionMode = ((ListViewBase)sender).SelectionMode switch
			{
				WASDKListViewSelectionMode.Single => SelectionMode.Single,
				WASDKListViewSelectionMode.Multiple => SelectionMode.Multiple,
				_ => SelectionMode.None
			};

			if (itemsView.SelectionMode != selectionMode)
			{
				itemsView.SelectionMode = selectionMode;
			}
		}

		void UpdatePlatformSelection()
		{
			_ignorePlatformSelectionChange = true;

			switch (ListViewBase.SelectionMode)
			{
				case WASDKListViewSelectionMode.None:
					break;
				case WASDKListViewSelectionMode.Single:
					if (ItemsView != null)
					{
						if (ItemsView.SelectedItem == null)
						{
							ListViewBase.SelectedItem = null;
						}
						else
						{
							ListViewBase.SelectedItem =
								ListViewBase.Items.FirstOrDefault(item =>
								{
									if (item is ItemTemplateContext itemPair)
									{
										return object.Equals(itemPair.Item, ItemsView.SelectedItem);
									}
									else
									{
										return object.Equals(item, ItemsView.SelectedItem);
									}
								});
						}
					}

					break;
				case WASDKListViewSelectionMode.Multiple:
					ListViewBase.SelectedItems.Clear();
					foreach (var nativeItem in ListViewBase.Items)
					{
						if (nativeItem is ItemTemplateContext itemPair && ItemsView.SelectedItems.Contains(itemPair.Item))
						{
							ListViewBase.SelectedItems.Add(nativeItem);
						}
						else if (ItemsView.SelectedItems.Contains(nativeItem))
						{
							ListViewBase.SelectedItems.Add(nativeItem);
						}
					}
					break;
				case WASDKListViewSelectionMode.Extended:
					break;
				default:
					break;
			}

			UpdateItemContentControlSelection();
			_ignorePlatformSelectionChange = false;
		}

		void VirtualSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// When the selection changes within the SelectionChanged event, the new selection isn't immediately reflected in the view.
			// After the virtual selection is correctly updated, the flag is reset to enable future updates
			if (_ignoreVirtualSelectionChange)
			{
				_ignoreVirtualSelectionChange = false;
				return;
			}
			UpdatePlatformSelection();
		}

		void PlatformSelectionChanged(object sender, WASDKSelectionChangedEventArgs args)
		{
			UpdateVirtualSelection();
		}

		void UpdateVirtualSelection()
		{
			if (_ignorePlatformSelectionChange || ItemsView == null)
			{
				return;
			}

			switch (ListViewBase.SelectionMode)
			{
				case WASDKListViewSelectionMode.None:
					break;
				case WASDKListViewSelectionMode.Single:
					UpdateVirtualSingleSelection();
					break;
				case WASDKListViewSelectionMode.Multiple:
					UpdateVirtualMultipleSelection();
					break;
				default:
					break;
			}

			UpdateItemContentControlSelection();
		}

		void UpdateVirtualSingleSelection()
		{
			var selectedItem = ListViewBase.SelectedItem is ItemTemplateContext itemPair
				? itemPair.Item
				: ListViewBase.SelectedItem;

			if (ItemsView != null)
			{
				_ignoreVirtualSelectionChange = true;
				ItemsView.SelectedItem = selectedItem;

				_ignoreVirtualSelectionChange = false;
			}
		}

		void UpdateVirtualMultipleSelection()
		{
			ItemsView.SelectionChanged -= VirtualSelectionChanged;

			var selection = new List<object>();
			for (int n = 0; n < ListViewBase.SelectedItems.Count; n++)
			{
				var item = ListViewBase.SelectedItems[n];
				selection.Add(item is ItemTemplateContext itc ? itc.Item : item);
			}

			ItemsView.UpdateSelectedItems(selection);

			ItemsView.SelectionChanged += VirtualSelectionChanged;
		}

		void UpdateItemContentControlSelection()
		{
			var formsItemContentControls = ListViewBase.GetChildren<ItemContentControl>();

			foreach (var formsItemContentControl in formsItemContentControls)
			{
				bool isSelected = object.Equals(ItemsView.SelectedItem, formsItemContentControl.FormsDataContext) || ItemsView.SelectedItems.Contains(formsItemContentControl.FormsDataContext);
				formsItemContentControl.UpdateIsSelected(isSelected);
			}
		}

		protected override void UpdateItemsLayout()
		{
			_ignorePlatformSelectionChange = true;

			base.UpdateItemsLayout();
			_ignorePlatformSelectionChange = false;
		}

		protected override void UpdateItemsSource()
		{
			_ignorePlatformSelectionChange = true;

			base.UpdateItemsSource();
			UpdatePlatformSelection();

			_ignorePlatformSelectionChange = false;
		}

	}
}
