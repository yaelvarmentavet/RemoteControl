using RemoteControl.Models;
using RemoteControl.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace RemoteControl.ViewModels
{
    class CMTViewModel : INotifyPropertyChanged
    {
        //public class Cmt
        //{
        //    public string Name;
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        //public ObservableRangeCollection<Cmt> cmt { get; set; }
        //public ObservableRangeCollection<Grouping<string, Cmt>> CmtGrouping { get; set; }

        public CMTViewModel()
        {
            //cmt = new ObservableRangeCollection<Cmt>();
            //CmtGrouping = new ObservableRangeCollection<Grouping<string, Cmt>>();

            //cmt.Add(new Cmt() { Name = "(N) &lt; 200K" });
            //cmt.Add(new Cmt() { Name = "(T) 200K – 400K" });
            //cmt.Add(new Cmt() { Name = "(1) 400K – 1200K" });
            //cmt.Add(new Cmt() { Name = "(2) 1200K – 5000K" });
            //cmt.Add(new Cmt() { Name = "(3) > 5000K" });

            //CmtGrouping.Add(new Grouping<string, Cmt>("CMT Results", cmt));

            NextPageHome = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PopToRootAsync();
            });
            NextPageSettings = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new SettingsPage());
            });
            Approve = new Command(async () =>
            {
                Application.Current.MainPage.Navigation.PopAsync();
            });

            Cancel = new Command(async () =>
            {
                App.DataModel.CmtFL = string.Empty;
                App.DataModel.CmtRL = string.Empty;
                App.DataModel.CmtFR = string.Empty;
                App.DataModel.CmtRR = string.Empty;
                Application.Current.MainPage.Navigation.PopAsync();
            });
        }

        public Command NextPageHome { get; set; }
        public Command NextPageSettings { get; set; }
        public Command Approve { get; set; }
        public Command Cancel { get; set; }
    }
}
