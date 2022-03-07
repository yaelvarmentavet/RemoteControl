
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

            TapGestureRecognizer tapZRCWStart = new TapGestureRecognizer() { NumberOfTapsRequired = 2 };
            tapZRCWStart.SetBinding(TapGestureRecognizer.CommandProperty, "ZRCWStart");
            ImgRCW.GestureRecognizers.Add(tapZRCWStart);

            TapGestureRecognizer tapZRCWStop = new TapGestureRecognizer() { NumberOfTapsRequired = 1 };
            tapZRCWStop.SetBinding(TapGestureRecognizer.CommandProperty, "ZRCWStop");
            ImgRCW.GestureRecognizers.Add(tapZRCWStop);

            TapGestureRecognizer tapZRCCWStart = new TapGestureRecognizer() { NumberOfTapsRequired = 2 };
            tapZRCCWStart.SetBinding(TapGestureRecognizer.CommandProperty, "ZRCCWStart");
            ImgRCCW.GestureRecognizers.Add(tapZRCCWStart);

            TapGestureRecognizer tapZRCCWStop = new TapGestureRecognizer() { NumberOfTapsRequired = 1 };
            tapZRCCWStop.SetBinding(TapGestureRecognizer.CommandProperty, "ZRCCWStop");
            ImgRCCW.GestureRecognizers.Add(tapZRCCWStop);



            TapGestureRecognizer tapAYFStart = new TapGestureRecognizer() { NumberOfTapsRequired = 2 };
            tapAYFStart.SetBinding(TapGestureRecognizer.CommandProperty, "AYFStart");
            ImgAYF.GestureRecognizers.Add(tapAYFStart);

            TapGestureRecognizer tapAYFStop = new TapGestureRecognizer() { NumberOfTapsRequired = 1 };
            tapAYFStop.SetBinding(TapGestureRecognizer.CommandProperty, "AYFStop");
            ImgAYF.GestureRecognizers.Add(tapAYFStop);

            TapGestureRecognizer tapAYBStart = new TapGestureRecognizer() { NumberOfTapsRequired = 2 };
            tapAYBStart.SetBinding(TapGestureRecognizer.CommandProperty, "AYBStart");
            ImgAYB.GestureRecognizers.Add(tapAYBStart);

            TapGestureRecognizer tapAYBStop = new TapGestureRecognizer() { NumberOfTapsRequired = 1 };
            tapAYBStop.SetBinding(TapGestureRecognizer.CommandProperty, "AYBStop");
            ImgAYB.GestureRecognizers.Add(tapAYBStop);



            TapGestureRecognizer tapMZUStart = new TapGestureRecognizer() { NumberOfTapsRequired = 2 };
            tapMZUStart.SetBinding(TapGestureRecognizer.CommandProperty, "MZUStart");
            ImgMZU.GestureRecognizers.Add(tapMZUStart);

            TapGestureRecognizer tapMZUStop = new TapGestureRecognizer() { NumberOfTapsRequired = 1 };
            tapMZUStop.SetBinding(TapGestureRecognizer.CommandProperty, "MZUStop");
            ImgMZU.GestureRecognizers.Add(tapMZUStop);

            TapGestureRecognizer tapMZDStart = new TapGestureRecognizer() { NumberOfTapsRequired = 2 };
            tapMZDStart.SetBinding(TapGestureRecognizer.CommandProperty, "MZDStart");
            ImgMZD.GestureRecognizers.Add(tapMZDStart);

            TapGestureRecognizer tapMZDStop = new TapGestureRecognizer() { NumberOfTapsRequired = 1 };
            tapMZDStop.SetBinding(TapGestureRecognizer.CommandProperty, "MZDStop");
            ImgMZD.GestureRecognizers.Add(tapMZDStop);
        }
    }
}