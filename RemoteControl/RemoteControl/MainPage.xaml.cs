using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RemoteControl
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_CMT_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CMTPage());
        }

        private async void Image_Support_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SupportPage());
        }

        private void Image_Home_Tapped(object sender, EventArgs e)
        {

        }
    }
}
