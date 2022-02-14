using RemoteControl.ViewModels;
using RemoteControl.Views;
using System;
using System.Linq;
using System.Threading;
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

            DependencyService.Get<IRemoteControlUsbDevice>().Event(async (sender, eventArgs) =>
            {
                if ((BindingContext != null) &&
                    (BindingContext as CowIdViewModel)?.IsPageOpened == true)
                    return;
               
                BindingContext = new CowIdViewModel()
                {
                    Id = DependencyService.Get<IRemoteControlUsbDevice>().GetId(),
                };
                await Current.MainPage.Navigation.PushAsync(new CowIdPage());
            });

            //DependencyService.Get<IRemoteControlUsbDevice>().Event(async (sender, eventArgs) =>
            //{
            //    await Current.MainPage.Navigation.PushAsync(new CowIdPage());
            //});
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
