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
    public partial class AndroidView : ContentView
    {
        public AndroidView()
        {
            InitializeComponent();

            LblCowId.BindingContext = App.DataModel;

            LblAmId.BindingContext = App.DataModel.Aptx;
            //LblAmId.SetBinding(Label.TextProperty, "SNum");

            LblAptxId.BindingContext = App.DataModel.Aptx;
            //LblAptxId.SetBinding(Label.TextProperty, "AptxId");

            BtnCmt.BindingContext = App.DataModel;
            BtnProcedure.BindingContext = App.DataModel;
        }
    }
}