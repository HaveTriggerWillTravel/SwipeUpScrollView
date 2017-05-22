using System;
using UIKit;

namespace SwipeUpScrollView
{
	public class SlidingContentViewController: UIViewController
	{
        public SwipeUpScrollViewDelegate SwipeUpScrollViewDelegate;

		public UIScrollView SwipeUpScrollView { get; set; }

		private HitTestView _hitTestView;

		public SwipeUpScrollViewController SwipeUpScrollViewController { get; set; }

		private NSLayoutConstraint _hitTestBottomLayoutConstraint;

		public bool InNavigationController
		{
			get
			{
				return SwipeUpScrollViewController.HasNavigationController;
			}
		}
		
		private nfloat? _swipeUpScrollViewHeight;
		public nfloat? SwipeUpScrollViewHeight
		{
			get
			{
				return _swipeUpScrollViewHeight;
			}
			set
			{
				_swipeUpScrollViewHeight = value;
				UpdateSwipeUpScrollViewHeight();
			}
		}

		private float _minScale;
		public float MinScale
		{
			get
			{
				return _minScale;
			}
			set
			{
				_minScale = value;
				ScalingEnabled = true;
				UpdateMinScale();
			}
		}

		private float _maxScale;
		public float MaxScale
		{
			get
			{
				return _maxScale;
			}
			set
			{
				_maxScale = value;
				ScalingEnabled = true;
				UpdateMaxScale();
			}
		}

		private UIView[] _swipeUpViewList;
		public UIView[] SwipeUpViewList
		{
			get
			{
				return _swipeUpViewList;
			}
			set
			{
				_swipeUpViewList = value;
				UpdateSwipeUpView();
			}
		}

		private bool _tapToRaiseEnabled;
		public bool TapToRaiseEnabled
		{
			get
			{
				return _tapToRaiseEnabled;
			}
			set
			{
				_tapToRaiseEnabled = value;
				UpdateTapToRaise();
			}
		}

		private bool _scalingEnabled;
		public bool ScalingEnabled
		{
			get
			{
				return _scalingEnabled;
			}
			set
			{
				_scalingEnabled = value;
				UpdateScaling();
			}
		}

		public SlidingContentViewController(SwipeUpScrollViewController swipeUpScrollViewController) : base()
		{
			SwipeUpScrollViewController = swipeUpScrollViewController;
			InitDefaults();
		}

		private void InitDefaults()
		{
			//Default to extra functionality not enabled
			TapToRaiseEnabled = false;
			ScalingEnabled = false;
			_minScale = 0.9f; //default to 90%
			_maxScale = 1;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			AutomaticallyAdjustsScrollViewInsets = false;

			EdgesForExtendedLayout = UIRectEdge.None;

			SwipeUpScrollView = new UIScrollView();
			SwipeUpScrollView.TranslatesAutoresizingMaskIntoConstraints = false;
			AutomaticallyAdjustsScrollViewInsets = false;

			var constraints = new NSLayoutConstraint[] 
			{ 
				NSLayoutConstraint.Create(SwipeUpScrollView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0),
				NSLayoutConstraint.Create(SwipeUpScrollView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
				NSLayoutConstraint.Create(SwipeUpScrollView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1, 0),
				NSLayoutConstraint.Create(SwipeUpScrollView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, 0)
			};

			View.AddSubview(SwipeUpScrollView);

			View.AddConstraints(constraints);

			_hitTestView = new HitTestView();
			_hitTestView.TranslatesAutoresizingMaskIntoConstraints = false;

			_hitTestBottomLayoutConstraint = NSLayoutConstraint.Create(_hitTestView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, 0);

			var hitTestViewConstraints = new NSLayoutConstraint[]
			{
				_hitTestBottomLayoutConstraint,
				NSLayoutConstraint.Create(_hitTestView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
				NSLayoutConstraint.Create(_hitTestView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1, 0),
				NSLayoutConstraint.Create(_hitTestView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0)
			};

			View.AddSubview(_hitTestView);

			View.AddConstraints(hitTestViewConstraints);

			SwipeUpScrollViewDelegate = new SwipeUpScrollViewDelegate(this, SwipeUpScrollView, _hitTestView);

			_hitTestView.SwipeUpScrollView = SwipeUpScrollView;

			//Initialise fields in delegate now that the view has loaded
			UpdateTapToRaise();
			UpdateSwipeUpView();
			UpdateSwipeUpScrollViewHeight();
			UpdateScaling();
		}

		public void LowerScrollView()
		{
			SwipeUpScrollViewDelegate.LowerScrollView();
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			//Have to wait until after ViewDidAppear to scale view otherwise height is 0
			UpdateMinScale();
			UpdateMaxScale();
		}

		private void UpdateTapToRaise()
		{
			if (SwipeUpScrollViewDelegate != null)
			{
				SwipeUpScrollViewDelegate.TapToRaiseEnabled = TapToRaiseEnabled;
			}
		}

		private void UpdateSwipeUpScrollViewHeight()
		{
			if (SwipeUpScrollViewHeight.HasValue)
			{
				SwipeUpScrollViewDelegate.ScrollViewHeight = SwipeUpScrollViewHeight.Value;
				_hitTestBottomLayoutConstraint.Constant = -SwipeUpScrollViewHeight.Value;
			}
		}

		private void UpdateSwipeUpView()
		{
			if (SwipeUpViewList != null && SwipeUpScrollViewDelegate != null)
			{
				SwipeUpScrollViewDelegate.SetViews(SwipeUpViewList);
			}
		}

		private void UpdateScaling()
		{
			if (SwipeUpScrollViewDelegate != null)
			{
				SwipeUpScrollViewDelegate.ScalingEnabled = ScalingEnabled;
			}
		}

		private void UpdateMinScale()
		{
			if (SwipeUpScrollViewDelegate != null)
			{
				SwipeUpScrollViewDelegate.MinScale = MinScale;
			}
		}

		private void UpdateMaxScale()
		{
			if (SwipeUpScrollViewDelegate != null)
			{
				SwipeUpScrollViewDelegate.MaxScale = MaxScale;
			}
		}
	}
}

