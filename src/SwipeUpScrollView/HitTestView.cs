using System;
using UIKit;

namespace SwipeUpScrollView
{
	public class HitTestView : UIView
	{
		public bool IsSwipeUpScrollViewRaised { get; set; }

		public UIScrollView SwipeUpScrollView { get; set; }

        private UIWindow _contentWindow { get { return UIApplication.SharedApplication.Delegate.GetWindow(); } }

		public HitTestView(IntPtr handle) : base(handle)
		{
			//Need this constructor for Interface Builder to be able to reference this
		}

		public HitTestView()
		{
		}

		public override UIView HitTest(CoreGraphics.CGPoint point, UIEvent uievent)
		{
			if (PointInside(point, uievent))
			{
				UIView view = base.HitTest(point, uievent);
				if (view == this && IsSwipeUpScrollViewRaised)
				{
					view = SwipeUpScrollView.Subviews[0].HitTest(point, uievent);
				}
				else if (view == this)
				{
                    view = _contentWindow.HitTest(point, uievent);
				}
				return view;
			}
			else
			{
				return null;
			}
		}
	}
}
