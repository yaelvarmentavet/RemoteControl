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

            ImgPressureOK.BindingContext = App.DataModel.Aptx;
            ImgPressureLow.BindingContext = App.DataModel.Aptx;
            //ImgSpeedOfBulletOK.BindingContext = App.DataModel.Aptx;
            //ImgSpeedOfBulletLow.BindingContext = App.DataModel.Aptx;
            //ImgOperationRun.BindingContext = App.DataModel.Aptx;
            //ImgOperationStop.BindingContext = App.DataModel.Aptx;
            LblBattery.BindingContext = App.DataModel.Aptx;
            //ImgBatteryOK.BindingContext = App.DataModel.Aptx;
            //ImgBatteryLow.BindingContext = App.DataModel.Aptx;
            ImgBattery100Per.BindingContext = App.DataModel.Aptx;
            ImgBattery75Per.BindingContext = App.DataModel.Aptx;
            ImgBattery50Per.BindingContext = App.DataModel.Aptx;
            ImgBattery25Per.BindingContext = App.DataModel.Aptx;
            ImgBattery15Per.BindingContext = App.DataModel.Aptx;
            //ImgPulsesYes.BindingContext = App.DataModel.Aptx;
            //ImgPulsesNo.BindingContext = App.DataModel.Aptx;
        }
    }
}