using System.ComponentModel;
using UwpApp.Base;

namespace UwpApp.ViewModel
{
    public interface ILoginViewModel : INotifyPropertyChanged
    {
        string Email { get; set; }
        bool HasAuthenticationFailed { get; set; }
        bool InvalidEmail { get; set; }
        bool InvalidPassword { get; set; }
        string Password { get; set; }
        bool IsLoading { get; set; }

        void Login();
    }
}