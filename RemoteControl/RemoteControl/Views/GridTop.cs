using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace RemoteControl.Views
{
    class GridTop
    {
        public Frame Frame;
        public Grid Grid;

        public GridTop(EventHandler home_tapped = null, EventHandler settings_tapped = null)
        {
            Image imghome = new Image();
            imghome.SetDynamicResource(Image.StyleProperty, "IconFrame");
            //img.SetBinding(Image.SourceProperty, "RemoteControl.Icons.home.png");
            imghome.Source = ImageSource.FromResource("RemoteControl.Icons.home.png");
            if (home_tapped != null)
            {
                TapGestureRecognizer taphome = new TapGestureRecognizer();
                taphome.Tapped += home_tapped;
                imghome.GestureRecognizers.Add(taphome);
            }

            Label labelpulses = new Label
            {
                Text = "Pulses: 150,000",
            };
            labelpulses.SetDynamicResource(Label.StyleProperty, "LabelSmall");

            Image imgsettings = new Image();
            imgsettings.SetDynamicResource(Image.StyleProperty, "IconFrame");
            //img.SetBinding(Image.SourceProperty, "RemoteControl.Icons.home.png");
            imgsettings.Source = ImageSource.FromResource("RemoteControl.Icons.settings.png");
            if (settings_tapped != null)
            {
                TapGestureRecognizer tapsettings = new TapGestureRecognizer();
                tapsettings.Tapped += settings_tapped;
                imgsettings.GestureRecognizers.Add(tapsettings);
            }

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
                    new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
                }
            };
            Grid.Children.Add(imghome, 0, 0);
            Grid.Children.Add(labelpulses, 1, 0);
            Grid.SetColumnSpan(labelpulses, 3);
            Grid.Children.Add(imgsettings, 4, 0);


        }

    }
}
