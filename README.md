# SwipeUpScrollView
Xamarin.iOS library for sliding content up from the bottom of the screen

To use, set the main view which is a single UIView and the scrollable content which is an array of UIVIews, i.e.

var swipeUpScrollViewController = new SwipeUpScrollViewController();
swipeUpScrollViewController.SwipeUpViewList = new UIView[] { view1, view2, view3, view4 etc.. };
swipeUpScrollViewController.MainView = mainView;

The sliding content can be a single view, it can also be smaller than the main view e.g.

swipeUpScrollViewController.SwipeUpViewList = new UIView[] { view1 };

Examples for how to configure the view:

Tap it when lowered to raise it up:

swipeUpScrollViewController.TapToRaiseEnabled = true;

Scale the views to 80% when lowered and scale to 100% as it slides up:

swipeUpScrollViewController.MinScale = 0.8f;
swipeUpScrollViewController.MaxScale = 1.0f;

Set the height of the sliding view when it is lowered:

swipeUpScrollViewController.SwipeUpScrollViewHeight = 100;

Choose how much of the view below to cover when raised:

Stop below the UINavigationBar:

swipeUpScrollViewController.CoverNavigationBar = false;
swipeUpScrollViewController.CoverStatusBar = false;

Cover UINavigationBar but stop below the StatusBar (default library behaviour):

swipeUpScrollViewController.CoverStatusBar = false;
swipeUpScrollViewController.CoverNaviationBar = true;

Cover entire screen including UINavigationBar and StatusBar:

swipeUpScrollViewController.CoverStatusBar = true;
