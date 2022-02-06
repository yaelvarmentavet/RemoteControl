using RemoteControl.Views;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RemoteControl
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var color = Resources.Where(r => r.Key == "BackgroundDark").FirstOrDefault().Value;
            MainPage = new NavigationPage(new MainPage())
            {
                BarBackgroundColor = color == null ? Color.Default : (Color)color,
            };
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
