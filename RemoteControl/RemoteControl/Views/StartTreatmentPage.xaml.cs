
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RemoteControl.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartTreatmentPage : ContentPage
    {
        public StartTreatmentPage()
        {
            InitializeComponent();

            EdCowId.BindingContext = App.DataModel;
            EdCowId.SetBinding(Editor.TextProperty, "SNum");

            EdInfo.BindingContext = App.DataModel;
            EdInfo.SetBinding(Editor.TextProperty, "SNum");

            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.SetBinding(TapGestureRecognizer.CommandProperty, "Start");
            ImgStart.GestureRecognizers.Add(tap);
        }
    }
}