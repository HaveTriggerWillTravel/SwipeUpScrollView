﻿using System;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace SwipeUpScrollView
{
    [Register("SwipeUpScrollViewController")]
    public class SwipeUpScrollViewController : UIViewController
    {
        protected NSLayoutConstraint[] _contentViewConstraints;
        protected NSLayoutConstraint _mainViewBottomConstraint;

        protected UIWindow _slidingUpWindow;

        protected SlidingContentViewController _slidingContentViewController;

        private bool _hasFirstLoaded;

        public UIScrollView ScrollView
        {
            get
            {
                return _slidingContentViewController.SwipeUpScrollView;
            }
        }

        protected bool _coverStatusBar;
        public bool CoverStatusBar
        {
            get
            {
                return _coverStatusBar;
            }
            set
            {
                _coverStatusBar = value;
                UpdateWindowBounds();
            }
        }

        protected bool _coverNavigationBar;
        public bool CoverNavigationBar
        {
            get
            {
                return _coverNavigationBar;
            }
            set
            {
                _coverNavigationBar = value;
                UpdateWindowBounds();
            }
        }

        protected nfloat? _swipeUpScrollViewHeight;
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

        protected UIView _mainView;
        public UIView MainView
        {
            get
            {
                return _mainView;
            }
            set
            {
                _mainView = value;
                UpdateMainView();
            }
        }

        public UIView[] SwipeUpViewList
        {
            get
            {
                return _slidingContentViewController.SwipeUpViewList;
            }
            set
            {
                if (_slidingContentViewController != null)
                {
                    _slidingContentViewController.SwipeUpViewList = value;
                }
            }
        }

        public bool TapToRaiseEnabled
        {
            get
            {
                return _slidingContentViewController.TapToRaiseEnabled;
            }
            set
            {
                if (_slidingContentViewController != null)
                {
                    _slidingContentViewController.TapToRaiseEnabled = value;
                }
            }
        }

        public float MinScale
        {
            get
            {
                return _slidingContentViewController.MinScale;
            }
            set
            {
                if (_slidingContentViewController != null)
                {
                    _slidingContentViewController.MinScale = value;
                }
            }
        }

        public float MaxScale
        {
            get
            {
                return _slidingContentViewController.MaxScale;
            }
            set
            {
                if (_slidingContentViewController != null)
                {
                    _slidingContentViewController.MaxScale = value;
                }
            }
        }

        public SwipeUpScrollViewController() : base()
        {
            _slidingUpWindow = new UIWindow(UIScreen.MainScreen.Bounds);
            _slidingUpWindow.ClipsToBounds = true;
            _slidingUpWindow.BackgroundColor = UIColor.Clear;
            _slidingUpWindow.WindowLevel = UIWindowLevel.Alert;
            _slidingUpWindow.Hidden = false;

            CoverStatusBar = false;
            CoverNavigationBar = true;
            _hasFirstLoaded = false;
            SwipeUpScrollViewHeight = 0;

            _slidingContentViewController = new SlidingContentViewController(this);
            _slidingUpWindow.RootViewController = _slidingContentViewController;
        } 

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			AutomaticallyAdjustsScrollViewInsets = false;
			EdgesForExtendedLayout = UIRectEdge.None;
		}

		protected void UpdateMainView()
		{
			if (MainView != null)
			{
				MainView.TranslatesAutoresizingMaskIntoConstraints = false;

				if (_contentViewConstraints != null)
				{
					View.RemoveConstraints(_contentViewConstraints);
				}

				if (View.Subviews.Length > 0)
				{
					View.Subviews.FirstOrDefault().RemoveFromSuperview();
				}

				View.AddSubview(MainView);

				_mainViewBottomConstraint = NSLayoutConstraint.Create(MainView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, 0);

				_contentViewConstraints = new NSLayoutConstraint[]
				{
					NSLayoutConstraint.Create(MainView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
					NSLayoutConstraint.Create(MainView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1, 0),
					NSLayoutConstraint.Create(MainView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0),
					_mainViewBottomConstraint
				};

				View.AddConstraints(_contentViewConstraints);
			}

            UpdateSwipeUpScrollViewHeight();
		}

		protected void UpdateSwipeUpScrollViewHeight()
		{
            if (SwipeUpScrollViewHeight.HasValue && _slidingContentViewController != null && _mainViewBottomConstraint != null)
			{
				_slidingContentViewController.SwipeUpScrollViewHeight = SwipeUpScrollViewHeight.Value;
				_mainViewBottomConstraint.Constant = -SwipeUpScrollViewHeight.Value;
			} 
		}

		protected void UpdateWindowBounds()
		{
			int topMargin = 0;

			if (CoverStatusBar)
			{
				//Do nothing, cover entire screen
			}
			else if (CoverNavigationBar)
			{
				topMargin += (int) UIApplication.SharedApplication.StatusBarFrame.Height;
			}
			else
			{
				topMargin += (int) (UIApplication.SharedApplication.StatusBarFrame.Height + 44); //Hardcoded to 44 for navigation bar
			}

			_slidingUpWindow.Frame = new CGRect(0, 
			                                     topMargin,
			                                     UIScreen.MainScreen.Bounds.Width,
			                                     UIScreen.MainScreen.Bounds.Height - topMargin);
		}

		public bool HasNavigationController
		{
			get
			{
				return NavigationController != null && !NavigationController.NavigationBarHidden;
			}
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();

			if (!_hasFirstLoaded)
			{
				_slidingContentViewController.LowerScrollView();
                _slidingContentViewController.SwipeUpScrollViewDelegate.AddPlaceHolderViewIfNeeded();
				_hasFirstLoaded = true;
			}
		}
	}
}

