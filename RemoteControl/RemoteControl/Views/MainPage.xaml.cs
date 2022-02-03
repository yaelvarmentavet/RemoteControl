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
            //InitializeComponent();
            //App.Current.Resources.
            BindingContext = new MainPageViewModel();

            Image img = new Image
            { 
            Source = RemoteControl.re
            };
            img.SetDynamicResource(Image.StyleProperty, "IconFrame");
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += Home_Tapped;
            img.GestureRecognizers.Add(tap);

            Grid gridtop = new Grid()
            {
                RowDefinitions =
                {
                    new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)},
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
                }
            };
            gridtop.Children.Add(img);

            Button cmt = new Button
            {
                Text = "CMT"
            };
            cmt.SetDynamicResource(Button.StyleProperty, "ButtonLarge");
            cmt.SetBinding(Button.CommandProperty, "NextPage1");

            Button treat = new Button
            {
                Text = "Treatment"
            };
            treat.SetDynamicResource(Button.StyleProperty, "ButtonLarge");
            treat.SetBinding(Button.CommandProperty, "NextPage2");

            Button cow = new Button
            {
                Text = "COW ID Management"
            };
            cow.SetDynamicResource(Button.StyleProperty, "ButtonLarge");
            cow.SetBinding(Button.CommandProperty, "NextPage3");

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
            gridmid.Children.Add(cmt);
            gridmid.Children.Add(treat, 1, 0);
            gridmid.Children.Add(cow, 2, 0);

            Grid grid = new Grid()
            {
                RowDefinitions =
                {
                    new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)},
                    new RowDefinition{ Height = new GridLength(5, GridUnitType.Star)},
                    new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)},
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
                }
            };
            grid.Children.Add(gridtop, 0, 0);
            grid.Children.Add(gridmid, 1, 0);

            Content = grid;
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
