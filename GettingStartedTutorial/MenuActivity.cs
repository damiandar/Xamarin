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

namespace GettingStartedTutorial
{
    [Activity(Label = "MenuActivity")]
    public class MenuActivity : Activity
    {
        ImageButton imageButton1 = null;
        ImageButton imageButton2 = null;
        ImageButton imageButton3 = null;
        ImageButton imageButton4 = null;
        ImageButton imageButton5 = null;
        ImageButton imageButton6 = null;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Menu);
            imageButton1 = FindViewById<ImageButton>(Resource.Id.imageButton1);
            imageButton2 = FindViewById<ImageButton>(Resource.Id.imageButton2);
            imageButton3 = FindViewById<ImageButton>(Resource.Id.imageButton3);
            imageButton4 = FindViewById<ImageButton>(Resource.Id.imageButton4);
            imageButton5 = FindViewById<ImageButton>(Resource.Id.imageButton5);
            imageButton6 = FindViewById<ImageButton>(Resource.Id.imageButton6);
            // Create your application here
            imageButton1.Click += ImageButton1_Click;
            imageButton2.Click += ImageButton2_Click;
            imageButton3.Click += ImageButton3_Click;
            imageButton4.Click += ImageButton4_Click;
            imageButton5.Click += ImageButton5_Click;
            imageButton6.Click += ImageButton6_Click;
        }

        private void ImageButton6_Click(object sender, EventArgs e)
        {
           
        }

        private void ImageButton5_Click(object sender, EventArgs e)
        { 
        }

        private void ImageButton4_Click(object sender, EventArgs e)
        { 
        }

        private void ImageButton3_Click(object sender, EventArgs e)
        { 
        }

        private void ImageButton2_Click(object sender, EventArgs e)
        { 
        }

        private void ImageButton1_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }
    }
}