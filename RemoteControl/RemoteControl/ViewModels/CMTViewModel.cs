using RemoteControl.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.CommunityToolkit.ObjectModel;

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
        }
    }
}
