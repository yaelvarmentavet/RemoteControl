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
    public partial class CowIdPage : ContentPage
    {
        public CowIdPage()
        {
            InitializeComponent();

            EdCowId.BindingContext = App.DataModel;
            EdTagId.BindingContext = App.DataModel;
            BtnAddCow.BindingContext = App.DataModel;
            //EdCowId.SetBinding(Editor.TextProperty, "CowId");
            //EdTagId.SetBinding(Editor.TextProperty, "TagId");

            //CowIdPageInit();
        }

        //void CowIdPageInit()
        //{
        //    //BindingContext = new CowIdViewModel();

        //    Label ltitle = new Label
        //    {
        //        Text = "Enter a new COW",
        //    };
        //    ltitle.SetDynamicResource(StyleProperty, "Label24");

        //    Label lblid = new Label
        //    {
        //        Text = "ID :",
        //    };
        //    lblid.SetDynamicResource(StyleProperty, "Label16");

        //    Editor edtid = new Editor();
        //    edtid.SetDynamicResource(StyleProperty, "Editor16");

        //    Label lbltagid = new Label
        //    {
        //        Text = "Tag ID:",
        //    };
        //    lbltagid.SetDynamicResource(StyleProperty, "Label16");

        //    Editor edttagid = new Editor
        //    {
        //        Placeholder = "*should be auto fill from cow tag",
        //    };
        //    edttagid.SetDynamicResource(StyleProperty, "Editor16");
        //    edttagid.SetBinding(Editor.TextProperty, "Id");

        //    Button btnadd = new Button
        //    {
        //        Text = "Add COW",
        //    };
        //    btnadd.SetDynamicResource(StyleProperty, "ButtonCornerRadius10");

        //    Grid gridmid = new Grid()
        //    {
        //        RowDefinitions =
        //        {
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
        //        }
        //    };
        //    gridmid.Children.Add(ltitle, 0, 0);
        //    Grid.SetColumnSpan(ltitle, gridmid.ColumnDefinitions.Count());
        //    gridmid.Children.Add(lblid, 1, 1);
        //    gridmid.Children.Add(edtid, 2, 1);
        //    Grid.SetColumnSpan(edtid, 2);
        //    gridmid.Children.Add(lbltagid, 1, 2);
        //    gridmid.Children.Add(edttagid, 2, 2);
        //    Grid.SetColumnSpan(edttagid, 2);
        //    gridmid.Children.Add(btnadd, 2, 3);
        //    Grid.SetColumnSpan(btnadd, 2);

        //    GridMain gridmain = new GridMain(gridmid, "NextPageHome", "NextPageSettings");

        //    Content = gridmain.Grid;
        //}

        //
        // Summary:
        //     When overridden, allows the application developer to customize behavior as the
        //     Xamarin.Forms.Page disappears.
        //
        // Remarks:
        //     Xamarin.Forms.Page.OnDisappearing is called when the page disappears due to navigating
        //     away from the page within the app. It is not called when the app disappears due
        //     to an event external to the app (e.g. user navigates to the home screen or another
        //     app, a phone call is received, the device is locked, the device is turned off).
        //protected override void OnDisappearing()
        //{
        //    if ((BindingContext != null) &&
        //        (BindingContext as CowIdViewModel)?.IsPageOpened == true)
        //        (BindingContext as CowIdViewModel).IsPageOpened = false;
        //}

        //private async void Home_Tapped(object sender, EventArgs e)
        //{
        //    await Navigation.PopToRootAsync();
        //}
    }
}
