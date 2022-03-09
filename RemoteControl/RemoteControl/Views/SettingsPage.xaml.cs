using RemoteControl.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RemoteControl.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            //Init();
        }
         
        //void Init()
        //{
        //    BindingContext = new SettingsViewModel();

        //    Label ltitle = new Label
        //    {
        //        Text = "Technician Mode",
        //    };
        //    ltitle.SetDynamicResource(Label.StyleProperty, "LabelTitle");

        //    Button btn1 = new Button
        //    {
        //        Text = "Battery V: XX[V] I: XX[A]"
        //    };
        //    Resources.Where(r => r.Key == "ButtonLarge");
        //    btn1.SetDynamicResource(Button.StyleProperty, "ButtonLarge");
        //    //btn1.SetBinding(Button.CommandProperty, "");

        //    Button btn2 = new Button
        //    {
        //        Text = "Valid touch XXX[%]"
        //    };
        //    btn2.SetDynamicResource(Button.StyleProperty, "ButtonLarge");
        //    //btn2.SetBinding(Button.CommandProperty, "");

        //    Button btn3 = new Button
        //    {
        //        Text = "Pressure XX[Bar]"
        //    };
        //    btn3.SetDynamicResource(Button.StyleProperty, "ButtonLarge");
        //    //btn3.SetBinding(Button.CommandProperty, "");

        //    Button btn4 = new Button
        //    {
        //        Text = "KTU Speed XX.X[m / sec]"
        //    };
        //    btn4.SetDynamicResource(Button.StyleProperty, "ButtonLarge");
        //    //btn4.SetBinding(Button.CommandProperty, "");

        //    Grid gridmid = new Grid()
        //    {
        //        RowDefinitions =
        //        {
        //            new RowDefinition{ Height = new GridLength(3, GridUnitType.Star)},
        //            new RowDefinition{ Height = new GridLength(6, GridUnitType.Star)},
        //            new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)},
        //            new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)},
        //            new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)},
        //        },
        //        ColumnDefinitions =
        //        {
        //            new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
        //            new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
        //            new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
        //            new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
        //        }
        //    };
        //    gridmid.Children.Add(ltitle, 0, 0);
        //    Grid.SetColumnSpan(ltitle, gridmid.ColumnDefinitions.Count());
        //    gridmid.Children.Add(btn1, 0, 1);
        //    gridmid.Children.Add(btn2, 1, 1);
        //    gridmid.Children.Add(btn3, 2, 1);
        //    gridmid.Children.Add(btn4, 3, 1);

        //    GridMain gridmain = new GridMain(gridmid, "NextPageHome", null);

        //    Content = gridmain.Grid;
        //}

        //private async void Home_Tapped(object sender, EventArgs e)
        //{
        //    await Navigation.PopToRootAsync();
        //}
    }
}