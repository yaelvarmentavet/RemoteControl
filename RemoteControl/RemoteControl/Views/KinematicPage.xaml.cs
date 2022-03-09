
using RemoteControl.ViewModels;
using Xamarin.CommunityToolkit.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RemoteControl.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class KinematicPage : ContentPage
    {
        public KinematicPage()
        {
            InitializeComponent();

            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.SetBinding(TapGestureRecognizer.CommandProperty, "ZRCWStart");
            ImgRCW.GestureRecognizers.Add(tap);

            tap = new TapGestureRecognizer();
            tap.SetBinding(TapGestureRecognizer.CommandProperty, "ZRCWStop");
            ImgRCWStop.GestureRecognizers.Add(tap);

            tap = new TapGestureRecognizer();
            tap.SetBinding(TapGestureRecognizer.CommandProperty, "ZRCCWStart");
            ImgRCCW.GestureRecognizers.Add(tap);

            tap = new TapGestureRecognizer();
            tap.SetBinding(TapGestureRecognizer.CommandProperty, "ZRCCWStop");
            ImgRCCWStop.GestureRecognizers.Add(tap);



            tap = new TapGestureRecognizer();
            tap.SetBinding(TapGestureRecognizer.CommandProperty, "AYFStart");
            ImgAYF.GestureRecognizers.Add(tap);

            tap = new TapGestureRecognizer();
            tap.SetBinding(TapGestureRecognizer.CommandProperty, "AYFStop");
            ImgAYFStop.GestureRecognizers.Add(tap);

            tap = new TapGestureRecognizer();
            tap.SetBinding(TapGestureRecognizer.CommandProperty, "AYBStart");
            ImgAYB.GestureRecognizers.Add(tap);

            tap = new TapGestureRecognizer();
            tap.SetBinding(TapGestureRecognizer.CommandProperty, "AYBStop");
            ImgAYBStop.GestureRecognizers.Add(tap);



            tap = new TapGestureRecognizer();
            tap.SetBinding(TapGestureRecognizer.CommandProperty, "MZUStart");
            ImgMZU.GestureRecognizers.Add(tap);

            tap = new TapGestureRecognizer();
            tap.SetBinding(TapGestureRecognizer.CommandProperty, "MZUStop");
            ImgMZUStop.GestureRecognizers.Add(tap);

            tap = new TapGestureRecognizer();
            tap.SetBinding(TapGestureRecognizer.CommandProperty, "MZDStart");
            ImgMZD.GestureRecognizers.Add(tap);

            tap = new TapGestureRecognizer();
            tap.SetBinding(TapGestureRecognizer.CommandProperty, "MZDStop");
            ImgMZDStop.GestureRecognizers.Add(tap);



            tap = new TapGestureRecognizer();
            tap.SetBinding(TapGestureRecognizer.CommandProperty, "StartTreatmentPage");
            ImgEmpty.GestureRecognizers.Add(tap);

            tap = new TapGestureRecognizer();
            tap.SetBinding(TapGestureRecognizer.CommandProperty, "StartTreatmentPage");
            ImgMan.GestureRecognizers.Add(tap);

            tap = new TapGestureRecognizer();
            tap.SetBinding(TapGestureRecognizer.CommandProperty, "StartTreatmentPage");
            ImgBreak.GestureRecognizers.Add(tap);

            tap = new TapGestureRecognizer();
            tap.SetBinding(TapGestureRecognizer.CommandProperty, "StartTreatmentPage");
            ImgNext.GestureRecognizers.Add(tap);
        }
    }
}