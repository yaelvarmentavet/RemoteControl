using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace RemoteControl.Behaviors
{
    public class EntryPressCommandBehavior : Behavior<Entry>
    {
        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create("Command", typeof(ICommand), typeof(EntryPressCommandBehavior), null);
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            if (bindable.BindingContext != null)
                BindingContext = bindable.BindingContext;
            bindable.BindingContextChanged += Bindable_BindingContextChanged;
            bindable.TextChanged += Bindable_TextChanged;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);
            if (bindable.BindingContext != null)
                BindingContext = bindable.BindingContext;
            bindable.BindingContextChanged -= Bindable_BindingContextChanged;
            bindable.TextChanged -= Bindable_TextChanged;
        }

        private void Bindable_TextChanged(object sender, TextChangedEventArgs e)
        {
            Command?.Execute(e.NewTextValue);
        }

        private void Bindable_BindingContextChanged(object sender, EventArgs e)
        {
            base.OnBindingContextChanged();

            if (!(sender is BindableObject bindable))
                return;

            BindingContext = bindable.BindingContext;
        }
    }
}
