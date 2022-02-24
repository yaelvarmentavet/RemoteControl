﻿using RemoteControl.ViewModels;
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
            
            Label lbltitle = new Label
            {
                Text = "APTX II – Welcome",
            };
            lbltitle.SetDynamicResource(StyleProperty, "LabelTitle");

            Button btncmt = new Button
            {
                Text = "CMT"
            };
            Resources.Where(r => r.Key == "ButtonLarge");
            btncmt.SetDynamicResource(StyleProperty, "ButtonLarge");
            btncmt.SetBinding(Button.CommandProperty, "NextPageCMT");

            Button btntreat = new Button
            {
                Text = "Treatment"
            };
            btntreat.SetDynamicResource(StyleProperty, "ButtonLarge");
            btntreat.SetBinding(Button.CommandProperty, "NextPageTreatment");

            Button btncowid = new Button
            {
                Text = "COW ID Management"
            };
            btncowid.SetDynamicResource(StyleProperty, "ButtonLarge");
            btncowid.SetBinding(Button.CommandProperty, "NextPageCowId");

            Label lblamid = new Label
            {
                Text = "AM ID :",
                HorizontalTextAlignment = TextAlignment.End,
            };
            lblamid.SetDynamicResource(StyleProperty, "LabelSmall");

            Label lblamval = new Label();
            lblamval.SetDynamicResource(StyleProperty, "LabelSmall");
            lblamval.SetBinding(Label.TextProperty, "SNum");
            //lblamval.BindingContext = BindingContext;

            Label lblaptid = new Label
            {
                Text = "APT ID :",
                HorizontalTextAlignment = TextAlignment.End,
            };
            lblaptid.SetDynamicResource(StyleProperty, "LabelSmall");

            Label lblaptval = new Label();
            lblaptval.SetDynamicResource(StyleProperty, "LabelSmall");
            lblaptval.SetBinding(Label.TextProperty, "AptId");
            //lblaptval.BindingContext = BindingContext;

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
            gridmid.Children.Add(lbltitle, 0, 0);
            Grid.SetColumnSpan(lbltitle, gridmid.ColumnDefinitions.Count());
            gridmid.Children.Add(btncmt, 0, 1);
            gridmid.Children.Add(btntreat, 1, 1);
            gridmid.Children.Add(btncowid, 2, 1);
            gridmid.Children.Add(lblamid, 0, 4);
            gridmid.Children.Add(lblamval, 1, 4);
            gridmid.Children.Add(lblaptid, 0, 5);
            gridmid.Children.Add(lblaptval, 1, 5);
            Grid.SetColumnSpan(lblaptval, gridmid.ColumnDefinitions.Count() - 1);

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
