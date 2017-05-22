using System;
using System.Linq;
using CoreGraphics;
using UIKit;

namespace SwipeUpScrollView
{
	public class SwipeUpScrollViewDelegate
	{
		private UIScrollView _scrollView;
		private NSLayoutConstraint[] _constraints;
		private SlidingContentViewController _slidingContentScrollViewController;
		private UIStackView _stackView;
		private HitTestView _hitTestView;

		private const int _navigationBarHeight = 44;

		internal bool TapToRaiseEnabled { get; set; }

		private nfloat _scrollViewHeight;
		public nfloat ScrollViewHeight
		{
			get
			{
				return _scrollViewHeight;
			}
			set
			{
				_scrollViewHeight = value;
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
				ScaleView();
			}
		}

		private float? _minScale;
		public float? MinScale
		{
			get
			{
				return _minScale;
			}
			set
			{
				_minScale = value;
				ScaleView();
			}
		}

		private float? _maxScale;
		public float? MaxScale
		{
			get
			{
				return _maxScale;
			}
			set
			{
				_maxScale = value;
				ScaleView();
			}
		}

		public bool _hasNavigationController
		{
			get
			{
				return _slidingContentScrollViewController.InNavigationController;
			}
		}

        private nfloat _scrollViewRaisingOffset
		{
			get
			{
				nfloat offset = 0;
				if (_slidingContentScrollViewController.SwipeUpScrollViewController.MainView != null)
				{
					offset = _slidingContentScrollViewController.SwipeUpScrollViewController.MainView.Frame.Size.Height;
					if (_slidingContentScrollViewController.SwipeUpScrollViewController.CoverStatusBar && _hasNavigationController)
					{
						offset += (_navigationBarHeight + UIApplication.SharedApplication.StatusBarFrame.Height);
					}
					else if (_slidingContentScrollViewController.SwipeUpScrollViewController.CoverNavigationBar && _hasNavigationController)
					{
						offset += _navigationBarHeight;
					}
					else if (!_hasNavigationController &&
					         !_slidingContentScrollViewController.SwipeUpScrollViewController.CoverStatusBar &&
					         !_slidingContentScrollViewController.SwipeUpScrollViewController.CoverNavigationBar)
					{
						offset -= (_navigationBarHeight + UIApplication.SharedApplication.StatusBarFrame.Height);
					}
					else if (!_hasNavigationController && _slidingContentScrollViewController.SwipeUpScrollViewController.CoverNavigationBar)
					{
						offset -= UIApplication.SharedApplication.StatusBarFrame.Height;
					}
				}
				return offset;
			}
		}

		private bool _isScrollViewRaised;
		public bool IsScrollViewRaised
		{
			get
			{
				return _isScrollViewRaised;
			}
			private set
			{
				_isScrollViewRaised = value;

				_scrollView.PagingEnabled = !value;

				if (_hitTestView != null)
				{	
					_hitTestView.IsSwipeUpScrollViewRaised = IsScrollViewRaised;
				}
			}
		}

		public SwipeUpScrollViewDelegate(SlidingContentViewController swipeUpScrollViewController, UIScrollView scrollView,
		                                 HitTestView hitTestView)
		{
			_slidingContentScrollViewController = swipeUpScrollViewController;
			_scrollView = scrollView;
			_scrollView.ShowsVerticalScrollIndicator = false;
			_scrollView.ShowsHorizontalScrollIndicator = false;
			_scrollView.ClipsToBounds = false;
			_scrollView.AlwaysBounceVertical = true;
			_scrollView.DirectionalLockEnabled = true;

			_hitTestView = hitTestView;

			_scrollView.Scrolled += (sender, e) => Scrolled();
			_scrollView.DecelerationEnded += (sender, e) => DecelerationEnded();

			var tapGestureRecogniser = new UITapGestureRecognizer();
            tapGestureRecogniser.AddTarget(ScrollViewTapped);
			_scrollView.AddGestureRecognizer(tapGestureRecogniser);
		}

		public void SetViews(UIView[] views)
		{
			_stackView = new UIStackView();
			_stackView.TranslatesAutoresizingMaskIntoConstraints = false;

			_stackView.Axis = UILayoutConstraintAxis.Vertical;
			_stackView.Alignment = UIStackViewAlignment.Top;
			_stackView.Distribution = UIStackViewDistribution.EqualSpacing;
			_stackView.Spacing = 0;

			foreach (var view in views)
			{
				_stackView.AddArrangedSubview(view);
				_stackView.AddConstraint(NSLayoutConstraint.Create(view, NSLayoutAttribute.Width, NSLayoutRelation.Equal, _stackView, NSLayoutAttribute.Width, 1, 0));
			}

			AddView(_stackView);
		}

        public void AddPlaceHolderViewIfNeeded()
        {
            var minContentHeight = _scrollViewRaisingOffset + ScrollViewHeight;
            if (_scrollView.ContentSize.Height < minContentHeight)
            {
                var placeHolderView = new UIView();
                placeHolderView.TranslatesAutoresizingMaskIntoConstraints = false;
                placeHolderView.AddConstraint(NSLayoutConstraint.Create(placeHolderView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, minContentHeight - _scrollView.ContentSize.Height));
                _stackView.AddArrangedSubview(placeHolderView);
                _stackView.AddConstraint(NSLayoutConstraint.Create(placeHolderView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, _stackView, NSLayoutAttribute.Width, 1, 0));
            }
        }

		private void AddView(UIStackView view)
		{
			if (_constraints != null)
			{
				_scrollView.RemoveConstraints(_constraints);
			}

			if (_scrollView.Subviews.Length > 0)
			{
				_scrollView.Subviews.FirstOrDefault().RemoveFromSuperview();
			}

			_scrollView.AddSubview(view);

			_constraints = new NSLayoutConstraint[]
			{
				NSLayoutConstraint.Create(view, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, _scrollView, NSLayoutAttribute.Leading, 1, 0),
				NSLayoutConstraint.Create(view, NSLayoutAttribute.Width, NSLayoutRelation.Equal, _scrollView, NSLayoutAttribute.Width, 1, 0),
				NSLayoutConstraint.Create(view, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, _scrollView, NSLayoutAttribute.Trailing, 1, 0),
				NSLayoutConstraint.Create(view, NSLayoutAttribute.Top, NSLayoutRelation.Equal, _scrollView, NSLayoutAttribute.Top, 1, 0),
				NSLayoutConstraint.Create(view, NSLayoutAttribute.Bottom, NSLayoutRelation.LessThanOrEqual, _scrollView, NSLayoutAttribute.Bottom, 1, 0)
			};

			_scrollView.AddConstraints(_constraints);

			ScaleView();


		}

		public void Scrolled()
		{
			if (IsScrollViewRaised && _scrollView.ContentOffset.Y <  0)
			{
                _scrollView.PagingEnabled = true;
			}

			ScaleView();
		}

		private void DecelerationEnded()
		{
            //-100 due to acceleration causing it to be not exactly 0
            IsScrollViewRaised = _scrollView.ContentOffset.Y >= -100;
		}

		private void RaiseScrollView()
		{
			IsScrollViewRaised = true;

			UIView.Animate(0.5f, () => {
				_scrollView.SetContentOffset(new CGPoint(0, 0), false);
			});
		}

		public void LowerScrollView()
		{
			IsScrollViewRaised = false;

			_scrollView.SetContentOffset(new CGPoint(0, -_scrollViewRaisingOffset), false);
			_scrollView.ContentInset = new UIEdgeInsets(_scrollViewRaisingOffset, 0, 0, 0);
		}
			
		private void ScrollViewTapped()
		{
			if (!IsScrollViewRaised && TapToRaiseEnabled)
			{
				RaiseScrollView();
			}
		}

		protected void ScaleView()
		{
			if (ScalingEnabled && MinScale.HasValue && MaxScale.HasValue)
			{
                float scale = (float) (1 - -_scrollView.ContentOffset.Y / _scrollViewRaisingOffset * (MaxScale.Value - MinScale.Value));

				if (scale > MaxScale.Value)
				{
					scale = MaxScale.Value;
				}
				else if (scale < MinScale.Value)
				{
					scale = MinScale.Value;
				}

				CGAffineTransform transform = CGAffineTransform.MakeIdentity();
				transform = CGAffineTransform.Scale(transform, scale, scale);
				_stackView.Transform = transform;
			}
		}
	}
}

