﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RemoteControl.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PortsView : ContentView
    {
        public PortsView()
        {
            InitializeComponent();

            //LblPort.BindingContext = App.DataModel;
            BtnDebugOnOff.BindingContext = App.DataModel;
            LblPacketCounter.BindingContext = App.DataModel;
        }
    }
}