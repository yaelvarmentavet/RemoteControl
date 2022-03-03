﻿using RemoteControl.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace RemoteControl.ViewModels
{
    class MainViewModelUWP : INotifyPropertyChanged
    {
        public MainViewModelUWP()
        {
            NextPageCowId = new Command(async () =>
            {
                await App.Current.MainPage.Navigation.PushAsync(new ManualCowIdPage());
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Command NextPageCowId { get; }
    }
}