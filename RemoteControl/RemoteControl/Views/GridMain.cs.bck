using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace RemoteControl.Views
{
    class GridMain
    {
        public Grid Grid;

        public GridMain(Grid gridmid, string home_tapped = null, string settings_tapped = null)
        {
            Frame framemid = new Frame();
            framemid.SetDynamicResource(Frame.StyleProperty, "FrameLight");

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
            GridTop gridtop = new GridTop(home_tapped: home_tapped, settings_tapped: settings_tapped);
            gridmain.Children.Add(gridtop.Frame, 0, 0);
            gridmain.Children.Add(framemid, 0, 1);
            GridBottom gridbottom = new GridBottom();
            gridmain.Children.Add(gridbottom.Frame, 0, 2);
            gridmain.Children.Add(gridtop.Grid, 0, 0);
            gridmain.Children.Add(gridmid, 0, 1);
            gridmain.Children.Add(gridbottom.Grid, 0, 2);

            Grid = gridmain;
        }
    }
}
