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
    public partial class Aptx2View : ContentView
    {
        public Aptx2View()
        {
            InitializeComponent();

            //LblCowId.BindingContext = App.DataModel;

            LblAmId.BindingContext = App.DataModel.Aptx;
            //LblAmId.SetBinding(Label.TextProperty, "SNum");
            LblAmIdRemaining.BindingContext = App.DataModel.Aptx;

            LblAptxId.BindingContext = App.DataModel.Aptx;
            //LblAptxId.SetBinding(Label.TextProperty, "AptxId");
            LblAptxIdRemaining.BindingContext = App.DataModel.Aptx;

            //BtnCmt.BindingContext = App.DataModel;
            //BtnProcedure.BindingContext = App.DataModel;
        }
    }
}