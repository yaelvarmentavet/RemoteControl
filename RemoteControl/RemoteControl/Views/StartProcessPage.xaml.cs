
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RemoteControl.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartProcessPage : ContentPage
    {
        public StartProcessPage()
        {
            InitializeComponent();

            LblCowId.BindingContext = App.DataModel;

            //PrgPulses0.BindingContext = App.DataModel.Aptxs[0];
            //PrgPulses1.BindingContext = App.DataModel.Aptxs[1];
            //PrgPulses2.BindingContext = App.DataModel.Aptxs[2];
            //PrgPulses3.BindingContext = App.DataModel.Aptxs[3];

            //LblPulses0.BindingContext = App.DataModel.Aptxs[0];
            //LblPulses1.BindingContext = App.DataModel.Aptxs[1];
            //LblPulses2.BindingContext = App.DataModel.Aptxs[2];
            //LblPulses3.BindingContext = App.DataModel.Aptxs[3];

            PrgPulses0.BindingContext = App.DataModel.Aptxs[0];
            PrgPulses1.BindingContext = App.DataModel.Aptxs[1];
            PrgPulses2.BindingContext = App.DataModel.Aptxs[2];
            PrgPulses3.BindingContext = App.DataModel.Aptxs[3];

            LblPulses0.BindingContext = App.DataModel.Aptxs[0];
            LblPulses1.BindingContext = App.DataModel.Aptxs[1];
            LblPulses2.BindingContext = App.DataModel.Aptxs[2];
            LblPulses3.BindingContext = App.DataModel.Aptxs[3];
        }
    }
}