using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GettingStartedTutorial
{
    [Activity(Label = "SplashScreen",MainLauncher = true)]
    public class SplashScreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
         
            base.OnCreate(savedInstanceState);
            ActionBar.Hide();
            // Create your application here
            SetContentView(Resource.Layout.SplashScreen);
            // Create your application here
            Task.Run(() => {
                Thread.Sleep(3000); // Simulate a long loading process on app startup.
                RunOnUiThread(() => {
                    StartActivity(typeof(MenuActivity));
                });
            });
        
    }
    }
}