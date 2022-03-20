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

            LblAmId.BindingContext = App.DataModel.Aptxs[0];
            LblAmId.SetBinding(Label.TextProperty, "SNum");

            LblAptxId.BindingContext = App.DataModel.Aptxs[0];
            LblAptxId.SetBinding(Label.TextProperty, "AptxId");
        }
    }
}