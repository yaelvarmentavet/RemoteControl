﻿
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RemoteControl.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TreatmentPage : ContentPage
    {
        public TreatmentPage()
        {
            InitializeComponent();
            //InitMainPage();
            
            LblFL.BindingContext = App.DataModel;
            LblFL.SetBinding(Label.BackgroundColorProperty, "FL");
        }

        //void InitMainPage()
        //{
        //    BindingContext = new TreatmentViewModel();

        //    Button btncmtres = new Button
        //    {
        //        Text = "Enter CMT Results"
        //    };
        //    btncmtres.SetDynamicResource(StyleProperty, "ButtonLarge");
        //    btncmtres.SetBinding(Button.CommandProperty, "CMTResult");

        //    Label lbltitle = new Label
        //    {
        //        Text = "Clinical Treatment – Cow ID ####",
        //    };
        //    lbltitle.SetDynamicResource(StyleProperty, "LabelTitle");

        //    Label lblcmt = new Label
        //    {
        //        Text = "CMT INFO:",
        //    };
        //    lblcmt.SetDynamicResource(StyleProperty, "LabelInfo");

        //    Label lblcmtval1 = new Label
        //    {
        //        Text = "(T) 200K – 400K",
        //    };
        //    lblcmtval1.SetDynamicResource(StyleProperty, "LabelInfo");
        //    //lblcmtval1.SetBinding(Label.TextProperty, "CMT1");

        //    Label lblcmtval2 = new Label
        //    {
        //        Text = "(1) 400K – 1200K",
        //    };
        //    lblcmtval2.SetDynamicResource(StyleProperty, "LabelInfo");
        //    //lblcmtval2.SetBinding(Label.TextProperty, "CMT2");

        //    Label lblcmtval3 = new Label
        //    {
        //        Text = "(2) 1200K – 5000K",
        //    };
        //    lblcmtval3.SetDynamicResource(StyleProperty, "LabelInfo");
        //    //lblcmtval3.SetBinding(Label.TextProperty, "CMT3");

        //    Label lblcmtval4 = new Label
        //    {
        //        Text = "(3) > 5000K",
        //    };
        //    lblcmtval4.SetDynamicResource(StyleProperty, "LabelInfo");
        //    //lblcmtval4.SetBinding(Label.TextProperty, "CMT4");

        //    Frame frminfo = new Frame();
        //    frminfo.SetDynamicResource(StyleProperty, "FrameInfo");

        //    Label lblselect = new Label
        //    {
        //        Text = "Select Quarter for to start treatment",
        //    };
        //    lblselect.SetDynamicResource(StyleProperty, "LabelSubtitle");

        //    Label lblautobtn = new Label
        //    {
        //        BackgroundColor = Color.LightBlue,
        //        //HorizontalTextAlignment = TextAlignment.End,
        //    };
        //    lblautobtn.SetDynamicResource(StyleProperty, "LabelMilli");

        //    Label lblauto = new Label
        //    {
        //        Text = "Auto Transition",
        //        //HorizontalTextAlignment = TextAlignment.End,
        //    };
        //    lblauto.SetDynamicResource(StyleProperty, "LabelMilli");

        //    Label lblpulse = new Label
        //    {
        //        Text = "0000"
        //    };
        //    lblpulse.SetDynamicResource(StyleProperty, "LabelTreat");
        //    //btncmt.SetBinding(Button.CommandProperty, "Treat");

        //    Label lblfl = new Label
        //    {
        //        Text = "Front Left",
        //    };
        //    lblfl.SetDynamicResource(StyleProperty, "LabelTreat");

        //    Label lblflv = new Label
        //    {
        //        Text = "T",
        //    };
        //    lblflv.SetDynamicResource(StyleProperty, "LabelFront");

        //    Label lblrl = new Label
        //    {
        //        Text = "Rear Left",
        //    };
        //    lblrl.SetDynamicResource(StyleProperty, "LabelTreat");

        //    Label lblrlv = new Label
        //    {
        //        Text = "2",
        //    };
        //    lblrlv.SetDynamicResource(StyleProperty, "LabelRear");

        //    Label lblfr = new Label
        //    {
        //        Text = "Front Right",
        //    };
        //    lblfr.SetDynamicResource(StyleProperty, "LabelTreat");

        //    Label lblfrv = new Label
        //    {
        //        Text = "1",
        //    };
        //    lblfrv.SetDynamicResource(StyleProperty, "LabelFront");

        //    Label lblrr = new Label
        //    {
        //        Text = "Rear Right",
        //    };
        //    lblrr.SetDynamicResource(StyleProperty, "LabelTreat");

        //    Label lblrrv = new Label
        //    {
        //        Text = "3",
        //    };
        //    lblrrv.SetDynamicResource(StyleProperty, "LabelRear");

        //    Grid gridmid = new Grid()
        //    {
        //        RowDefinitions =
        //        {
        //            new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)},
        //            new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)},
        //            new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)},
        //            new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)},
        //            new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)},
        //            new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)},
        //            new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)},
        //            new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)},
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
        //            new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
        //            new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
        //            new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
        //            new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
        //            new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
        //            new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
        //            new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
        //            new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
        //            new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
        //            new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)},
        //        }
        //    };
        //    gridmid.Children.Add(lbltitle, 0, 0);
        //    Grid.SetColumnSpan(lbltitle, gridmid.ColumnDefinitions.Count());
        //    Grid.SetRowSpan(lbltitle, 2);

        //    gridmid.Children.Add(frminfo, 0, 1);
        //    Grid.SetColumnSpan(frminfo, gridmid.ColumnDefinitions.Count());
        //    Grid.SetRowSpan(frminfo, 2);

        //    gridmid.Children.Add(lblcmt, 0, 1);
        //    Grid.SetColumnSpan(lblcmt, 3);
        //    gridmid.Children.Add(lblcmtval1, 3, 1);
        //    Grid.SetColumnSpan(lblcmtval1, 3);
        //    gridmid.Children.Add(lblcmtval2, 6, 1);
        //    Grid.SetColumnSpan(lblcmtval2, 3);
        //    gridmid.Children.Add(lblcmtval3, 9, 1);
        //    Grid.SetColumnSpan(lblcmtval3, 3);
        //    gridmid.Children.Add(lblcmtval4, 12, 1);
        //    Grid.SetColumnSpan(lblcmtval4, 3);

        //    gridmid.Children.Add(lblselect, 0, 2);
        //    Grid.SetColumnSpan(lblselect, gridmid.ColumnDefinitions.Count());
        //    Grid.SetRowSpan(lblselect, 2);

        //    gridmid.Children.Add(lblautobtn, 6, 4);
        //    Grid.SetColumnSpan(lblautobtn, 2);
        //    gridmid.Children.Add(lblauto, 6, 5);
        //    Grid.SetColumnSpan(lblauto, 2);

        //    gridmid.Children.Add(lblpulse, 5, 6);
        //    Grid.SetColumnSpan(lblpulse, 4);
        //    Grid.SetRowSpan(lblpulse, 3);

        //    gridmid.Children.Add(lblfl, 0, 5);
        //    Grid.SetColumnSpan(lblfl, 3);
        //    Grid.SetRowSpan(lblfl, 2);
        //    gridmid.Children.Add(lblflv, 3, 5);
        //    Grid.SetColumnSpan(lblflv, 2);
        //    Grid.SetRowSpan(lblflv, 2);

        //    gridmid.Children.Add(lblrl, 0, 8);
        //    Grid.SetColumnSpan(lblrl, 3);
        //    Grid.SetRowSpan(lblrl, 2);
        //    gridmid.Children.Add(lblrlv, 3, 8);
        //    Grid.SetColumnSpan(lblrlv, 2);
        //    Grid.SetRowSpan(lblrlv, 2);

        //    gridmid.Children.Add(lblfr, 11, 5);
        //    Grid.SetColumnSpan(lblfr, 3);
        //    Grid.SetRowSpan(lblfr, 2);
        //    gridmid.Children.Add(lblfrv, 9, 5);
        //    Grid.SetColumnSpan(lblfrv, 2);
        //    Grid.SetRowSpan(lblfrv, 2);

        //    gridmid.Children.Add(lblrr, 11, 8);
        //    Grid.SetColumnSpan(lblrr, 3);
        //    Grid.SetRowSpan(lblrr, 2);
        //    gridmid.Children.Add(lblrrv, 9, 8);
        //    Grid.SetColumnSpan(lblrrv, 2);
        //    Grid.SetRowSpan(lblrrv, 2);

        //    GridMain gridmain = new GridMain(gridmid, home_tapped: "NextPageHome", settings_tapped: "NextPageSettings");

        //    Content = gridmain.Grid;
        //}
    }
}