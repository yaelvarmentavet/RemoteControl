using RemoteControl.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RemoteControl.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            //InitializeComponent();
            InitMainPage();
        }

        void InitMainPage()
        {
            BindingContext = new MainViewModel();

            Label ltitle = new Label
            {
                Text = "APTX II – Welcome",
            };
            ltitle.SetDynamicResource(Label.StyleProperty, "LabelTitle");

            Button btn1 = new Button
            {
                Text = "CMT"
            };
            Resources.Where(r => r.Key == "ButtonLarge");
            btn1.SetDynamicResource(Button.StyleProperty, "ButtonLarge");
            btn1.SetBinding(Button.CommandProperty, "NextPageCMT");

            Button btn2 = new Button
            {
                Text = "Treatment"
            };
            btn2.SetDynamicResource(Button.StyleProperty, "ButtonLarge");
            btn2.SetBinding(Button.CommandProperty, "NextPageTreatment");

            Button btn3 = new Button
            {
                Text = "COW ID Management"
            };
            btn3.SetDynamicResource(Button.StyleProperty, "ButtonLarge");
            btn3.SetBinding(Button.CommandProperty, "NextPageCowId");
            
            Label lbl1 = new Label
            {
                Text = "AM ID : 987653",
            };
            lbl1.SetDynamicResource(Label.ScaleProperty, "LabelSmall");

            Label lbl2 = new Label
            {
                Text = "APT ID : 234876",
            };
            lbl2.SetDynamicResource(Label.ScaleProperty, "LabelSmall");

            Grid gridmid = new Grid()
            {
                RowDefinitions =
                {
                    new RowDefinition{ Height = new GridLength(3, GridUnitType.Star)},
                    new RowDefinition{ Height = new GridLength(6, GridUnitType.Star)},
                    new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)},
                    new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)},
                    new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)},
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
                }
            };
            gridmid.Children.Add(ltitle, 0, 0);
            Grid.SetColumnSpan(ltitle, gridmid.ColumnDefinitions.Count());
            gridmid.Children.Add(btn1, 0, 1);
            gridmid.Children.Add(btn2, 1, 1);
            gridmid.Children.Add(btn3, 2, 1);
            gridmid.Children.Add(lbl1, 0, 4);
            gridmid.Children.Add(lbl2, 0, 5);

            GridMain gridmain = new GridMain(gridmid, settings_tapped: "NextPageSettings");

            Content = gridmain.Grid;
        }

        //private async void Settings_Tapped(object sender, EventArgs e)
        //{
        //    await Navigation.PushAsync(new SettingsPage());
        //}
        //
        //private async void CMT_Tapped(object sender, EventArgs e)
        //{
        //    await Navigation.PushAsync(new CMTPage());
        //}
        //
        //private async void Treatment_Tapped(object sender, EventArgs e)
        //{
        //    await Navigation.PushAsync(new TreatmentPage());
        //}
        //
        //private async void COWIDManagement_Tapped(object sender, EventArgs e)
        //{
        //    await Navigation.PushAsync(new COWIDManagementPage());
        //}
    }
}
