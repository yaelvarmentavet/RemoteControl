using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace RemoteControl.Behaviors
{
    public class SimplePickerValidator : Behavior<Picker>
    {
        protected override void OnAttachedTo(Picker bindable)
        {
            bindable.SelectedIndexChanged += Bindable_SelectedIndexChanged;
        }

        protected override void OnDetachingFrom(Picker bindable)
        {
            bindable.SelectedIndexChanged -= Bindable_SelectedIndexChanged;
        }

        private void Bindable_SelectedIndexChanged(object sender, EventArgs e)
        {
            var bindable = (Picker)sender;
            var isValid = bindable.SelectedIndex >= bindable.Items.Count / 2;
            if (isValid)
                bindable.BackgroundColor = Color.Default;
            else
                bindable.BackgroundColor = Color.Salmon;
        }
    }
}
