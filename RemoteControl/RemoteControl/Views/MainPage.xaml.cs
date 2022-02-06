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
            Init();
        }

        void Init()
        {
            BindingContext = new MainPageViewModel();

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
            btn1.SetBinding(Button.CommandProperty, "NextPage1");

            Button btn2 = new Button
            {
                Text = "Treatment"
            };
            btn2.SetDynamicResource(Button.StyleProperty, "ButtonLarge");
            btn2.SetBinding(Button.CommandProperty, "NextPage2");

            Button btn3 = new Button
            {
                Text = "COW ID Management"
            };
            btn3.SetDynamicResource(Button.StyleProperty, "ButtonLarge");
            btn3.SetBinding(Button.CommandProperty, "NextPage3");

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

            Frame framemid = new Frame();
            framemid.SetDynamicResource(Frame.StyleProperty, "FrameLight");

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

            Frame framemain = new Frame();
            framemain.SetDynamicResource(Frame.StyleProperty, "FrameDark");
            
            Grid gridmain = new Grid()
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
            gridmain.Children.Add(framemain, 0, 0);
            Grid.SetRowSpan(framemain, 3);
            GridTop gridtop = new GridTop(settings_tapped: Settings_Tapped);
            gridmain.Children.Add(gridtop.Frame, 0, 0);
            gridmain.Children.Add(framemid, 0, 1);
            GridBottom gridbottom = new GridBottom();
            gridmain.Children.Add(gridbottom.Frame, 0, 2);
            gridmain.Children.Add(gridtop.Grid, 0, 0);
            gridmain.Children.Add(gridmid, 0, 1);
            gridmain.Children.Add(gridbottom.Grid, 0, 2);

            Content = gridmain;
        }

        private async void Settings_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }

        private async void CMT_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CMTPage());
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
