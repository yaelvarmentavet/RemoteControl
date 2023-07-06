using RemoteControl.Models;
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
    public partial class EcomilkView : ContentView
    {
        public EcomilkView()
        {
            InitializeComponent();
            EdCowId.BindingContext = App.DataModel;
            //EdCowId.SetBinding(Editor.TextProperty, "SNum");

            //TapGestureRecognizer tap = new TapGestureRecognizer();
            //tap.SetBinding(TapGestureRecognizer.CommandProperty, "NextPageCowId");
            //ImgId.GestureRecognizers.Add(tap);
        }
    }
}