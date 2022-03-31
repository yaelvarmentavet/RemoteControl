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
    public partial class TopView : ContentView
    {
        public TopView()
        {
            InitializeComponent();

            LblRem.BindingContext = App.DataModel.Aptxs[0];
            EdPort.BindingContext = App.DataModel;
            //LblRem.SetBinding(Label.TextProperty, "Remaining");
        }
    }
}