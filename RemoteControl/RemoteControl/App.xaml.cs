using Autofac;
using CommonServiceLocator;
using RemoteControl.Models;
using RemoteControl.Views;
using System.Linq;
using Xamarin.Forms;

namespace RemoteControl
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            
            DataModel = ServiceLocator.Current.GetInstance<DataModel>();

            var color = Resources.Where(r => r.Key == "BackgroundDark").FirstOrDefault().Value;
            MainPage = new NavigationPage(new MainPage())
            {
                BarBackgroundColor = color == null ? Color.Default : (Color)color,
            };
            
        }

        public static DataModel DataModel;

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
