using System;
using Xamarin.Forms;

namespace RemoteControl.Views
{
    public class CustomButton : ImageButton
    {
        public delegate void DBindableEvent();

        public static readonly BindableProperty CustomPressedProperty =
                 BindableProperty.Create("CustomPressed", typeof(DBindableEvent),
                                  typeof(CustomButton),
                                  null);
        public static readonly BindableProperty CustomReleasedProperty =
                 BindableProperty.Create("CustomReleased", typeof(DBindableEvent),
                                  typeof(CustomButton),
                                  null);

        public DBindableEvent CustomPressed
        {
            get
            {
                return (DBindableEvent)GetValue(CustomPressedProperty);
            }
            set
            {
                SetValue(CustomPressedProperty, value);
            }
        }
        public DBindableEvent CustomReleased
        {
            get
            {
                return (DBindableEvent)GetValue(CustomReleasedProperty);
            }
            set
            {
                SetValue(CustomReleasedProperty, value);
            }
        }

        public void OnCustomPressed()
        {
            CustomPressed?.Invoke();
        }

        public void OnCustomReleased()
        {
            CustomReleased?.Invoke();
        }
    }
}
