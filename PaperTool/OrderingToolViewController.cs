// This file has been autogenerated from parsing an Objective-C header file added in Xcode.

using System;
using System.Threading;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MessageUI;
using Casascius.Bitcoin;

namespace PaperTool
{
	public partial class OrderingToolViewController : UIViewController
	{

		MFMailComposeViewController _mail;


		public OrderingToolViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			btnGenerate.TouchUpInside += OnClickButtonGenerate;
			stepCodeCount.ValueChanged += delegate(object sender, EventArgs e) {
				// allowable values: 1,2,3,4,5,10,20,50,60,70,80,90,100,120,140,160,180,200 etc
				if (stepCodeCount.Value==6) {
					stepCodeCount.Value=10;
				} else if (stepCodeCount.Value==9) {
					stepCodeCount.Value=5;
				} else if (stepCodeCount.Value > 10 && stepCodeCount.Value < 100) {
					if (stepCodeCount.Value % 10 == 1) {
						stepCodeCount.Value += 9;
					} else if (stepCodeCount.Value % 10 == 9) {
						stepCodeCount.Value -= 9;
					}

				} else if (stepCodeCount.Value > 100) {
					if (stepCodeCount.Value % 10 == 1) {
						stepCodeCount.Value += 19;
					} else if (stepCodeCount.Value % 10 == 9) {
						stepCodeCount.Value -= 19;
					}

				}

				if (stepCodeCount.Value == 1) {
					lblCodeCount.Text = "A total of 1 code...";
				} else {
					lblCodeCount.Text = "A total of " + stepCodeCount.Value + " codes...";
				}
			};
		}
	

		bool userDidCancel=false;
		bool workerRunning=false;

		string thePassphrase;
		List<string> generatedCodes;
		int leftToDo = 0;
		int lotNumber = 0;


		void OnClickButtonGenerate (object sender, EventArgs e)
		{
			if (workerRunning) {
				btnGenerate.SetTitle("Stopping...", UIControlState.Normal);
				userDidCancel=true;
				return;
			}

			if ((Application.currentPassphrase ?? "") == "") {
				UIAlertView alert = new UIAlertView ("No passphrase set",
				                                     "You need to set a passphrase before generating Intermediate Codes. " +
					"The passphrase will be needed later to decrypt your vouchers.",			                                     
			                                     null, "OK", null);
			
				alert.Show ();
				return;
			}
		
			userDidCancel=false;
			generatedCodes = new List<string>();
			btnGenerate.SetTitle ("Cancel (generating...)", UIControlState.Normal);
			actIndicator.StartAnimating();
			Thread t = new Thread(new ThreadStart(ThreadProc));
			workerRunning = true;
			leftToDo = (int)stepCodeCount.Value;
			thePassphrase = Application.currentPassphrase;
			t.Start ();
			

		}

		void ThreadProc ()
		{

			try {
				Bip38Intermediate mostrecentcode = null;

				while (leftToDo-- > 0) {
					if (userDidCancel) {

						return;

					}
					if (mostrecentcode==null) {
						mostrecentcode = new Bip38Intermediate (thePassphrase, Bip38Intermediate.Interpretation.Passphrase, 1);
					} else {
						mostrecentcode = new Bip38Intermediate (mostrecentcode);
					}				
					generatedCodes.Add (mostrecentcode.Code);
					InvokeOnMainThread (delegate {
						if (IsViewLoaded) {
							btnGenerate.SetTitle ("Cancel (done " + generatedCodes.Count + ")", UIControlState.Normal);
						}
					});


				}

				lotNumber = mostrecentcode.LotNumber;

				if (userDidCancel == false) {
					InvokeOnMainThread (delegate {
						if (IsViewLoaded) {
							if (MFMailComposeViewController.CanSendMail) {

								_mail = new MFMailComposeViewController ();

								_mail.SetSubject("Codes");

								string msgstart = "Below find the code you'll need for generating my passphrase-protected cryptocurrency materials.  " +
									"This code was derived from the passphrase I have selected.  I understand that I will need to remember my original passphrase to decrypt the key.\r\n\r\n" + 
									"Lot number: " + mostrecentcode.LotNumber + "\r\n";

								if (generatedCodes.Count != 1) {
									msgstart = "Below find the codes you'll need for generating my passphrase-protected cryptocurrency materials.\r\n\r\n" +
									"Each code was derived from the passphrase I have selected.  I understand that I will need to remember my original passphrase to decrypt my keys.\r\n\r\n" +
											"I am expecting my materials will show a lot number of " + mostrecentcode.LotNumber + " and sequence numbers 1 thru " + mostrecentcode.SequenceNumber + ".\r\n\r\n";
								}


								string msgBody = msgstart + 
									
									String.Join ("\r\n", generatedCodes);

								Console.WriteLine (msgBody);
								_mail.SetMessageBody (msgBody,
							                      
							                      false);
					
								_mail.Finished += HandleMailFinished;

							
							
								this.PresentModalViewController (_mail, true);

							
							
							} else {

								using (var alert = new UIAlertView()) {
									alert.Message = "The tool could not launch Mail";
									alert.Title = "Mail not available";
									alert.AddButton ("OK");
									alert.Show ();
								}

							}
						}
					});
				
				}
			} finally {
				InvokeOnMainThread (delegate {
					if (IsViewLoaded) {
						workerRunning = false;
						userDidCancel = false;

						btnGenerate.SetTitle("Generate and E-mail Codes", UIControlState.Normal);
						actIndicator.StopAnimating();
					}
				});
			}
		

		}

		void HandleMailFinished (object sender, MFComposeResultEventArgs e)
		{

			if (e.Result == MFMailComposeResult.Sent) {

					UIAlertView alert = new UIAlertView ("E-mail Sent", "If you are ordering printed materials, the lot number " + lotNumber + " will be printed on them.  Use this number to ensure you received the correct order.",
					                                     
					                                     null, "OK", null);

					alert.Show ();

					
					
						// you should handle other values that could be returned
						
						// in e.Result and also in e.Error
						
			}

			e.Controller.DismissModalViewControllerAnimated (true);



			
		}
	}
}
