using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace System.MacOS.AppKit
{
	[NativeClass("NSSegmentedCell", "AppKit")]
	public class SegmentedCell : ActionCell
	{
		#region SegmentCollection Class
		
		public sealed class SegmentCollection : IList<Segment>
		{
			private SegmentedCell cell;
			
			internal SegmentCollection(SegmentedCell cell) { this.cell = cell; }
			
			public Segment this[int index]
			{
				get
				{
					lock (cell.segmentList)
						return cell.segmentList[index];
				}
				set
				{
					lock (cell.segmentList)
					{
						var oldSegment = cell.segmentList[index];
						
						if (value != oldSegment)
						{
							if (value != null) value.Cell = cell;
							
							cell.segmentList[index] = value;
							
							if (oldSegment != null) oldSegment.Cell = null;
						}
					}
				}
			}
			
			public int Count
			{
				get
				{
					lock (cell.segmentList) // This lock is probably not necessary, but just to stay consistent…
						return cell.segmentList.Count;
				}
			}
			
			public bool IsReadOnly { get { return false; } }
			
			public void Add(Segment item)
			{
				lock (cell.segmentList)
				{
					item.Cell = cell;
					if (cell.segmentList.Count == 0) item.Selected = true;
					cell.segmentList.Add(item);
					cell.SegmentAdded();
				}
			}
			
			public void Insert(int index, Segment item)
			{
				lock (cell.segmentList)
				{
					item.Cell = cell;
					if (cell.segmentList.Count == 0) item.Selected = true;
					cell.segmentList.Insert(index, item);
					cell.SegmentInserted(index);
				}
			}
			
			public bool Contains(Segment item)
			{
				lock (cell.segmentList)
					return cell.segmentList.Contains(item);
			}
			
			public int IndexOf(Segment item)
			{
				lock (cell.segmentList)
					return cell.segmentList.IndexOf(item);
			}
			
			public bool Remove(Segment item)
			{
				lock (cell.segmentList)
				{
					int index = cell.segmentList.IndexOf(item);
					
					if (index >= 0)
					{
						cell.segmentList.RemoveAt(index);
						return true;
					}
					else
						return false;
				}
			}
			
			public void RemoveAt(int index)
			{
				lock (cell.segmentList)
				{
					var segment = cell.segmentList[index];
					
					cell.segmentList.RemoveAt(index);
					segment.Cell = null;
					cell.SegmentRemoved(index);
				}
			}
			
			public void Clear()
			{
				lock (cell.segmentList)
				{
					foreach (var segment in cell.segmentList)
						segment.Cell = null;
					cell.segmentList.Clear();
					cell.SegmentsRemoved();
				}
			}
			
			public void CopyTo(Segment[] array, int arrayIndex)
			{
				lock (cell.segmentList)
					cell.segmentList.CopyTo(array, arrayIndex);
			}
			
			public IEnumerator<Segment> GetEnumerator()
			{
				Segment[] segments;
				
				lock (cell.segmentList)
					segments = cell.segmentList.ToArray();
				
				foreach (var segment in segments)
					yield return segment;
			}
			
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
		}
		
		#endregion
		
		#region Method Selector Ids
		
		internal static new class Selectors
		{
			private static class setTrackingMode { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setTrackingMode:"); }
			private static class setSegmentStyle { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setSegmentStyle:"); }
			private static class setSegmentCount { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setSegmentCount:"); }
			private static class selectedSegment { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("selectedSegment"); }
			private static class setSelectedSegment { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setSelectedSegment:"); }
			private static class setLabelForSegment { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setLabel:forSegment:"); }
			private static class setToolTipForSegment { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setToolTip:forSegment:"); }
			private static class setImageForSegment { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setImage:forSegment:"); }
			private static class setImageScalingForSegment { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setImageScaling:forSegment:"); }
			private static class setMenuForSegment { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setMenu:forSegment:"); }
			private static class setSelectedForSegment { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setSelected:forSegment:"); }
			private static class setEnabledForSegment { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setEnabled:forSegment:"); }
			private static class setWidthForSegment { public static readonly IntPtr SelectorHandle = ObjectiveC.GetSelector("setWidth:forSegment:"); }
			
			public static IntPtr SetTrackingMode { get { return setTrackingMode.SelectorHandle; } }
			public static IntPtr SetSegmentStyle { get { return setSegmentStyle.SelectorHandle; } }
			public static IntPtr SetSegmentCount { get { return setSegmentCount.SelectorHandle; } }
			public static IntPtr SetLabelForSegment { get { return setLabelForSegment.SelectorHandle; } }
			public static IntPtr SetToolTipForSegment { get { return setToolTipForSegment.SelectorHandle; } }
			public static IntPtr SetImageForSegment { get { return setImageForSegment.SelectorHandle; } }
			public static IntPtr SetImageScalingForSegment { get { return setImageScalingForSegment.SelectorHandle; } }
			public static IntPtr SetMenuForSegment { get { return setMenuForSegment.SelectorHandle; } }
			public static IntPtr SetSelectedForSegment { get { return setSelectedForSegment.SelectorHandle; } }
			public static IntPtr SetEnabledForSegment { get { return setEnabledForSegment.SelectorHandle; } }
			public static IntPtr SetWidthForSegment { get { return setWidthForSegment.SelectorHandle; } }
		}
		
		#endregion
		
		#region NSSegmentedControl Interop
		
		// Most of these selectors do nothing, this way, no one can tweak the control behind our back
		
		[SelectorStub("setSegmentCount:")]
		private static void SetSegmentCount(IntPtr self, IntPtr _cmd, IntPtr count) { }
		
		[SelectorStub("setLabel:forSegment:")]
		[SelectorStub("setToolTip:forSegment:")]
		[SelectorStub("setImage:forSegment:")]
		[SelectorStub("setImageScaling:forSegment:")]
		[SelectorStub("setMenu:forSegment:")]
		[SelectorStub("setSelected:forSegment:")]
		[SelectorStub("setEnabled:forSegment:")]
		[SelectorStub("setWidth:forSegment:")]
		private static void SetPropertyForSegment(IntPtr self, IntPtr _cmd, IntPtr label, IntPtr segment) { }
		
		[SelectorStub("setSelectedSegment:")]
		private static void SetSelectedSegment(IntPtr self, IntPtr _cmd, IntPtr segment)
		{
			Debug.WriteLine("Selected Segment: " + segment);
		}
		
		#endregion
		
		private List<Segment> segmentList;
		private SegmentCollection segmentCollection;
		private SegmentStyle style;
		private ItemSelectionMode selectionMode;
		
		public SegmentedCell()
		{
			segmentList = new List<Segment>();
			segmentCollection = new SegmentCollection(this);
		}
		
		public SegmentCollection Segments { get { return segmentCollection; } }
		
		public SegmentStyle Style
		{
			get { return style; }
			set
			{
				if (!Enum.IsDefined(typeof(SegmentStyle), value))
					throw new ArgumentOutOfRangeException("value");
				
				if (value != style)
				{
					style = value;
					
					if (Created)
						SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetSegmentStyle, checked((IntPtr)style));
				}
			}
		}
		
		public ItemSelectionMode SelectionMode
		{
			get { return selectionMode; }
			set
			{
				if (!Enum.IsDefined(typeof(ItemSelectionMode), value))
					throw new ArgumentOutOfRangeException("value");
				
				if (value != selectionMode)
				{
					selectionMode = value;
					
					if (Created)
						SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetTrackingMode, checked((IntPtr)selectionMode));
				}
			}
		}
		
		internal override void OnCreated()
		{
			base.OnCreated();
			SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetTrackingMode, checked((IntPtr)selectionMode));
			SafeNativeMethods.objc_msgSend(NativePointer, Selectors.SetSegmentStyle, checked((IntPtr)style));
			UpdateSegments(0);
		}
		
		private void HandleSegmentPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!Created) return;
			
			var segment = sender as Segment;
			
			lock (segmentList)
			{
				int index = segmentList.IndexOf(segment);
				
				if (index < 0) return; // This condition should never be true in reality.
				
				var i = checked((IntPtr)segmentList.IndexOf(segment));
				
				// Don't handle the Selected property here
				switch (e.PropertyName)
				{
					case "Label":
						UpdateLabel(i, segment);
						break;
					case "ToolTip":
						UpdateToolTip(i, segment);
						break;
					case "Image":
						UpdateImage(i, segment);
						break;
					case "Menu":
						UpdateMenu(i, segment);
						break;
					case "Enabled":
						UpdateEnabled(i, segment);
						break;
				}
			}
		}
		
		internal void HandleSegmentSelectedChanged(Segment segment)
		{
			if (!Created) return;
			
			UpdateSelected(checked((IntPtr)segmentList.IndexOf(segment)), segment);
		}
		
		private void SegmentAdded() { SegmentInserted(segmentList.Count - 1); }
		
		private void SegmentInserted(int index) { if (Created) UpdateSegments(index); }
		
		private void SegmentRemoved(int index) { if (Created) UpdateSegments(index); }
		
		private void SegmentsRemoved() { if (Created) UpdateSegmentCount(); } 
		
		private void UpdateSegments(int index)
		{
			UpdateSegmentCount();
			
			for (int i = index; i < segmentList.Count; i++)
				UpdateSegment(i);
		}
		
		private void UpdateSegmentCount()
		{
			SafeNativeMethods.objc_msgSendSuper(ref super, Selectors.SetSegmentCount, checked((IntPtr)segmentList.Count));
		}
		
		private void UpdateSegment(int index)
		{
			var segment = segmentList[index];
			var i = checked((IntPtr)index);
			
			UpdateLabel(i, segment);
			UpdateToolTip(i, segment);
			UpdateImage(i, segment);
			UpdateMenu(i, segment);
			UpdateEnabled(i, segment);
			UpdateSelected(i, segment);
		}
		
		private void UpdateLabel(IntPtr index, Segment segment) { SafeNativeMethods.objc_msgSendSuper_set_String(ref super, Selectors.SetLabelForSegment, segment.Label, index); }
		
		private void UpdateToolTip(IntPtr index, Segment segment) { SafeNativeMethods.objc_msgSendSuper_set_String(ref super, Selectors.SetToolTipForSegment, segment.ToolTip, index); }
		
		private void UpdateImage(IntPtr index, Segment segment) { SafeNativeMethods.objc_msgSendSuper(ref super, Selectors.SetImageForSegment, segment.Image != null ? segment.Image.NativePointer : IntPtr.Zero, index); }
		
		private void UpdateMenu(IntPtr index, Segment segment) { SafeNativeMethods.objc_msgSendSuper(ref super, Selectors.SetMenuForSegment, segment.Menu != null ? segment.Menu.NativePointer : IntPtr.Zero, index); }
		
		private void UpdateEnabled(IntPtr index, Segment segment) { SafeNativeMethods.objc_msgSendSuper_set_Boolean(ref super, Selectors.SetEnabledForSegment, segment.Enabled, index); }
		
		private void UpdateSelected(IntPtr index, Segment segment) { SafeNativeMethods.objc_msgSendSuper_set_Boolean(ref super, Selectors.SetSelectedForSegment, segment.Selected, index); }
		
		protected override void OnAction(EventArgs e)
		{
			base.OnAction(e);
		}
		
		public override object Clone()
		{
			var clone = base.Clone() as SegmentedCell;
			
			clone.segmentList = new List<Segment>(segmentList.Count);
			clone.segmentCollection = new SegmentCollection(clone);
			
			foreach (var segment in segmentList)
				clone.segmentList.Add(segment.Clone() as Segment);
			
			return clone;
		}
	}
}
