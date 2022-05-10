using System;
using Xamarin.Forms;

namespace RemoteControl.Views
{
    public class CustomButton : ImageButton
    {
        public event EventHandler Pressed;
        public event EventHandler Released;

        public virtual void OnPressed()
        {
            Pressed?.Invoke(this, EventArgs.Empty);
        }

        public virtual void OnReleased()
        {
            Released?.Invoke(this, EventArgs.Empty);
        }
    }
}
