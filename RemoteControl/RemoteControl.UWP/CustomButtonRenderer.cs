using RemoteControl.UWP;
using RemoteControl.Views;
using System;
using Windows.UI.Xaml.Input;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(CustomButton), typeof(CustomButtonRenderer))]
namespace RemoteControl.UWP
{
    public class CustomButtonRenderer : ImageButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ImageButton> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
            }
            if (e.NewElement != null)
            {
                if (Control != null)
                {
                    CustomButton customButton = e.NewElement as CustomButton;

                    FormsButton thisButton = Control;

                    //thisButton.Holding += (sender, args) =>
                    // {
                    //     if (args.HoldingState == Windows.UI.Input.HoldingState.Started)
                    //         customButton.OnPressed();
                    //     if (args.HoldingState == Windows.UI.Input.HoldingState.Completed)
                    //         customButton.OnReleased();
                    // };

                    thisButton.AddHandler(PointerPressedEvent, new PointerEventHandler((sender, args) => { customButton.OnCustomPressed(); }), true);
                    thisButton.AddHandler(PointerReleasedEvent, new PointerEventHandler((sender, args) => { customButton.OnCustomReleased(); }), true);
                }
            }
        }
    }
}
