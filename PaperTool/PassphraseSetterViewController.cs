// This file has been autogenerated from parsing an Objective-C header file added in Xcode.

using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace PaperTool
{
	public partial class PassphraseSetterViewController : UIViewController
	{
		public PassphraseSetterViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			txtPassphrase.ClearButtonMode = UITextFieldViewMode.Always;
			txtPassphrase.ShouldReturn = delegate {
				txtPassphrase.ResignFirstResponder();
				return false;
			};

			txtPassphrase.EditingChanged += delegate(object sender, EventArgs e) {

				Application.currentPassphrase = txtPassphrase.Text.Trim ();
			};

			switchShow.ValueChanged += delegate(object sender, EventArgs e) {
				Console.WriteLine ("switch " + (switchShow.On ? "on" : "off"));
				txtPassphrase.SecureTextEntry = !(switchShow.On);
			};

			this.View.BackgroundColor = UIColor.FromPatternImage(UIImage.FromFile ("Images/bigvires.png"));
		}
	}
}