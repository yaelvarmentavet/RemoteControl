using FluentValidation;
using FluentValidation.Internal;
using MvvmHelpers;
using RemoteControl.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Behaviors;
using Xamarin.Forms;

namespace RemoteControl.ViewModels
{
    public class Ratings
    {
        public int StarRating { get; set; }
        public string Description { get; set; }
    }
    class MainViewModelUWP : BaseViewModel
    {
        public MainViewModelUWP()
        {
            NextPageCowId = new Command(async () =>
            {
                await App.Current.MainPage.Navigation.PushAsync(new ManualCowIdPage());
                //await App.DataModel.PortConnectRequest("RFID", "");
                //await App.DataModel.PortConnectReply("RFID", "");
                //await App.DataModel.PortConnectReply("REMOTE", "");
            });
            NextPageStatus = new Command(async () =>
            {
                await App.Current.MainPage.Navigation.PushAsync(new StatusPage());
            });



            SelectedBeardRating = BeardRatings[0];
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Command NextPageCowId { get; }
        public Command NextPageStatus { get; }



        ObservableCollection<Ratings> beardRatings = new ObservableCollection<Ratings>()
        {
            new Ratings{Description="Fair", StarRating = 0},
            new Ratings{Description="Good", StarRating = 1},
            new Ratings{Description="Cool", StarRating = 2},
            new Ratings{Description="Great", StarRating = 3},
            new Ratings{Description="Magnificent", StarRating = 4},
        };
        public ObservableCollection<Ratings> BeardRatings
        { get => beardRatings; }
        Ratings selectedRating;
        public Ratings SelectedBeardRating
        {
            get => selectedRating;
            set => SetProperty(ref selectedRating, value);
        }
        public static string[] ValidRatings =
        {
            "Good", "Magnificent"
        };
        ICommand entryPressCommand;
        public ICommand EntryPressCommand => entryPressCommand ?? (entryPressCommand = new Command<string>(ParseBeardText));

        private void ParseBeardText(string input)
        {
            if (input.ToLower().Contains("fair"))
                SelectedBeardRating = BeardRatings[0];
            else if (input.ToLower().Contains("good"))
                SelectedBeardRating = BeardRatings[1];
            else if (input.ToLower().Contains("cool"))
                SelectedBeardRating = BeardRatings[2];
            else if (input.ToLower().Contains("great"))
                SelectedBeardRating = BeardRatings[3];
            else if (input.ToLower().Contains("magnificent"))
                SelectedBeardRating = BeardRatings[4];
        }

        private ValidatableObject<string> _userName = new ValidatableObject<string>();
        public ValidatableObject<string> UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserName)));
                //RaisePropertyChanged(() => UserName);
            }
        }

        private ValidatableObject<string> _password = new ValidatableObject<string>();
        public ValidatableObject<string> Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Password)));
                //RaisePropertyChanged(() => Password);
            }
        }

        private void AddValidations()
        {
            _userName.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "A username is required."
            });
            _password.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "A password is required."
            });
        }

        private bool Validate()
        {
            bool isValidUser = ValidateUserName();
            bool isValidPassword = ValidatePassword();
            return isValidUser && isValidPassword;
        }

        private bool ValidateUserName()
        {
            return _userName.Validate();
        }

        private bool ValidatePassword()
        {
            return _password.Validate();
        }
    }

    public class IsNotNullOrEmptyRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            if (value == null)
            {
                return false;
            }

            var str = value as string;
            return !string.IsNullOrWhiteSpace(str);
        }
    }

    public class EmailRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            if (value == null)
            {
                return false;
            }

            var str = value as string;
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(str);

            return match.Success;
        }
    }

    public interface IValidationRule<T>
    {
        string ValidationMessage { get; set; }
        bool Check(T value);
    }

    public class ValidatableObject<T>
    {
        private List<IValidationRule<T>> _validations = new List<IValidationRule<T>>();
        public List<IValidationRule<T>> Validations
        {
            get => _validations;
            set => _validations = value;
        }

        public T Value;
        public List<string> Errors = new List<string>();
        public bool IsValid = false;

        public bool Validate()
        {
            Errors.Clear();

            IEnumerable<string> errors = _validations
                .Where(v => !v.Check(Value))
                .Select(v => v.ValidationMessage);

            Errors = errors.ToList();
            IsValid = !Errors.Any();

            return this.IsValid;
        }
    }

    public static class LineColorBehavior
    {
        public static readonly BindableProperty ApplyLineColorProperty =
            BindableProperty.CreateAttached(
                "ApplyLineColor",
                typeof(bool),
                typeof(LineColorBehavior),
                false,
                propertyChanged: OnApplyLineColorChanged);

        public static bool GetApplyLineColor(BindableObject view)
        {
            return (bool)view.GetValue(ApplyLineColorProperty);
        }

        public static void SetApplyLineColor(BindableObject view, bool value)
        {
            view.SetValue(ApplyLineColorProperty, value);
        }

        public static readonly BindableProperty LineColorProperty =
                    BindableProperty.CreateAttached(
                "LineColor",
                typeof(bool),
                typeof(LineColorBehavior),
                false);

        public static bool GetLineColor(BindableObject view)
        {
            return (bool)view.GetValue(LineColorProperty);
        }

        public static void SetLineColor(BindableObject view, bool value)
        {
            view.SetValue(LineColorProperty, value);
        }

        private static void OnApplyLineColorChanged(
                    BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as View;
            if (view == null)
            {
                return;
            }

            bool hasLine = (bool)newValue;
            if (hasLine)
            {
                view.Effects.Add(new EntryLineColorEffect());
            }
            else
            {
                var entryLineColorEffectToRemove =
                        view.Effects.FirstOrDefault(e => e is EntryLineColorEffect);
                if (entryLineColorEffectToRemove != null)
                {
                    view.Effects.Remove(entryLineColorEffectToRemove);
                }
            }
        }
    }

    public class EntryLineColorEffect : RoutingEffect
    {
        public EntryLineColorEffect() : base("eShopOnContainers.EntryLineColorEffect")
        {
        }
    }
}
