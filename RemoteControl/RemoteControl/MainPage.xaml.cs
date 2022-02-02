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
            //App.Current.Resources.
        }

        private async void CMT_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CMTPage());
        }

        private async void Support_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SupportPage());
        }

        private void Home_Tapped(object sender, EventArgs e)
        {

        }

        private async void Treatment_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TreatmentPage());
        }

        private async void COWIDManagement_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new COWIDManagementPage());
        }
    }
}
