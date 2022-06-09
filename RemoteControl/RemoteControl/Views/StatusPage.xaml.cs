
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RemoteControl.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatusPage : ContentPage
    {
        public StatusPage()
        {
            InitializeComponent();

            //LblAptx1.BindingContext = App.DataModel.Aptxs[0];
            //LblAptx2.BindingContext = App.DataModel.Aptxs[1];
            //LblAptx3.BindingContext = App.DataModel.Aptxs[2];
            //LblAptx4.BindingContext = App.DataModel.Aptxs[3];

            LblAptx1.BindingContext = App.DataModel.Aptxs[0];
            LblAptx2.BindingContext = App.DataModel.Aptxs[1];
            LblAptx3.BindingContext = App.DataModel.Aptxs[2];
            LblAptx4.BindingContext = App.DataModel.Aptxs[3];
        }
    }
}