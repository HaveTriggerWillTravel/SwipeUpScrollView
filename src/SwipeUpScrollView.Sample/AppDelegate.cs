using Foundation;
using UIKit;
using CoreGraphics;

namespace SwipeUpScrollView.Sample
{
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
            Window = new UIWindow(UIScreen.MainScreen.Bounds);
			Window.BackgroundColor = UIColor.White;

			var view1 = new UIView() { BackgroundColor = UIColor.LightGray };
			view1.AddConstraint(NSLayoutConstraint.Create(view1, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, 200));
            var view2 = new UIView() { BackgroundColor = UIColor.White };
			view2.AddConstraint(NSLayoutConstraint.Create(view2, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, 200));
			var view3 = new UIView() { BackgroundColor = UIColor.LightGray };
			view3.AddConstraint(NSLayoutConstraint.Create(view3, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, 200));
            var view4 = new UIView() { BackgroundColor = UIColor.White };
			view4.AddConstraint(NSLayoutConstraint.Create(view4, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, 200));
            var view5 = new UIView() { BackgroundColor = UIColor.LightGray };
            view5.AddConstraint(NSLayoutConstraint.Create(view5, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, 200));

            var viewController = new SwipeUpScrollViewController();
            viewController.MainView = new UILabel { Text = "This is the main view.", TextAlignment = UITextAlignment.Center};
            viewController.SwipeUpViewList = new UIView[] { view1, view2, view3, view4, view5 };

			viewController.TapToRaiseEnabled = true;
            viewController.MinScale = 0.9f;
			viewController.SwipeUpScrollViewHeight = 100;
			viewController.CoverStatusBar = false;
			viewController.CoverNavigationBar = true;

			var navigationController = new UINavigationController(viewController);
			var barButtonItem = new UIBarButtonItem();
			barButtonItem.Title = "Test";
			navigationController.NavigationItem.LeftBarButtonItem = barButtonItem;
			viewController.Title = "SwipeUpScrollView";
			Window.RootViewController = navigationController;

            viewController.ScrollView.Scrolled += (sender, e) => 
            {
              viewController.MainView.Alpha = -viewController.ScrollView.ContentOffset.Y / viewController.MainView.Frame.Height;
            };

			Window.MakeKeyAndVisible();

			return true;
		}
	}
}

