using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Widget;
using Xamarin.Contacts;
using Android.Content;

namespace DrunkDial
{
	[Activity(Label = "DrunkDial", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button>(Resource.Id.myButton);
			
			button.Click += delegate
			{
				Dial();
			};
		}

		private void Dial()
		{
			var book = new Xamarin.Contacts.AddressBook(this);
			book.RequestPermission().ContinueWith(t =>
				{
					if (!t.Result)
					{
						Console.WriteLine("Permission denied by user or manifest");
						return;
					}
					var validContacts = book.Where(a => a.Phones.Any(b => b.Number.Any())).ToList();
					var totalValidContacts = validContacts.Count;
					if (totalValidContacts < 1)
					{
						var alert = new AlertDialog.Builder(this);
						alert.SetTitle("No valid Contacts Found");
						alert.SetMessage("No valid Contacts Found");
					}
					var rnd = new Random();
					Contact contact = null;
					while (contact == null)
					{
						contact = validContacts.Skip(rnd.Next(0, totalValidContacts)).FirstOrDefault();
					}
					var urlNumber = Android.Net.Uri.Parse("tel:" + contact.Phones.First().Number);
					var intent = new Intent(Intent.ActionCall);
					intent.SetData(urlNumber);
					this.StartActivity(intent);
				}, TaskScheduler.FromCurrentSynchronizationContext());
		}
	}
}


