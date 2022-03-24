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
    public partial class BottomView : ContentView
    {
        public BottomView()
        {
            InitializeComponent();

            ImgPressureOK.BindingContext = App.DataModel;
            ImgPressureLow.BindingContext = App.DataModel;
            ImgSpeedOfBulletOK.BindingContext = App.DataModel;
            ImgSpeedOfBulletLow.BindingContext = App.DataModel;
            ImgBatteryOK.BindingContext = App.DataModel;
            ImgBatteryLow.BindingContext = App.DataModel;
            ImgPulsesYes.BindingContext = App.DataModel;
            ImgPulsesNo.BindingContext = App.DataModel;
        }
    }
}