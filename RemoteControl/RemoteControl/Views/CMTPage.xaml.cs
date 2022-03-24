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
    public partial class CMTPage : ContentPage
    {
        public CMTPage()
        {
            InitializeComponent();

            LVFL.BindingContext = App.DataModel;
            LVRL.BindingContext = App.DataModel;
            LVFR.BindingContext = App.DataModel;
            LVRR.BindingContext = App.DataModel;
            
            LblCowId.BindingContext = App.DataModel;

            //LVFL.ItemSelected += LVFL_ItemSelected;
        }

        //private void LVFL_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        //{
        //    App.DataModel.Cmt[0] = (string)e.SelectedItem;
        //}
    }
}