using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace RemoteControl.Views
{
    class GridBottom
    {
        public Frame Frame;
        public Grid Grid;

        public GridBottom()
        {
            Image imgpressure = new Image();
            imgpressure.SetDynamicResource(Image.StyleProperty, "IconFrame");
            //img.SetBinding(Image.SourceProperty, "RemoteControl.Icons.home.png");
            imgpressure.Source = ImageSource.FromResource("RemoteControl.Icons.pr1h.bmp");

            Image imgpulse = new Image();
            imgpulse.SetDynamicResource(Image.StyleProperty, "IconFrame");
            //img.SetBinding(Image.SourceProperty, "RemoteControl.Icons.home.png");
            imgpulse.Source = ImageSource.FromResource("RemoteControl.Icons.pulseYes.bmp");

            Image imgbattery = new Image();
            imgbattery.SetDynamicResource(Image.StyleProperty, "IconFrame");
            //img.SetBinding(Image.SourceProperty, "RemoteControl.Icons.home.png");
            imgbattery.Source = ImageSource.FromResource("RemoteControl.Icons.bat_1.bmp");

            Image imgap = new Image();
            imgap.SetDynamicResource(Image.StyleProperty, "IconFrame");
            //img.SetBinding(Image.SourceProperty, "RemoteControl.Icons.home.png");
            imgap.Source = ImageSource.FromResource("RemoteControl.Icons.ap_clean.bmp");

            Frame = new Frame();
            Frame.SetDynamicResource(Frame.StyleProperty, "FrameLight");

            Grid = new Grid()
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
                }
            };
            Grid.Children.Add(imgpressure, 0, 0);
            Grid.Children.Add(imgpulse, 1, 0);
            Grid.Children.Add(imgbattery, 2, 0);
            Grid.Children.Add(imgap, 3, 0);

        }
    }
}
