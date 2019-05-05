namespace UwpApp.ViewModel
{
    public interface ILoginViewModel
    {
        string Email { get; set; }
        bool HasAuthenticationFailed { get; set; }
        bool InvalidEmail { get; set; }
        bool InvalidPassword { get; set; }
        string Password { get; set; }

        void Login();
    }
}