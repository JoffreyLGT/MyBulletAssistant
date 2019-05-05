using Core.Data;
using Core.Models;
using System;
using System.Text.RegularExpressions;
using UwpApp.Base;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Security.Credentials;
using UwpApp.Startup;
using Autofac;

namespace UwpApp.ViewModel
{
    public class LoginViewModel : Observable, ILoginViewModel
    {

        private readonly IDataProvider client;
        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                OnPropertyChanged();
            }

        }
        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged();
            }
        }

        private bool invalidPassword;

        public bool InvalidPassword
        {
            get { return invalidPassword; }
            set
            {
                invalidPassword = value;
                OnPropertyChanged();
            }
        }

        private bool invalidEmail;

        public bool InvalidEmail
        {
            get { return invalidEmail; }
            set
            {
                invalidEmail = value;
                OnPropertyChanged();
            }
        }

        private bool hasAuthenticationFailed;

        public bool HasAuthenticationFailed
        {
            get { return hasAuthenticationFailed; }
            set
            {
                hasAuthenticationFailed = value;
                OnPropertyChanged();
            }
        }

        public LoginViewModel()
        {
            client = ConfigureServices.Container().Resolve<IDataProvider>();

            var loginCredential = GetCredentialFromLocker();
            if (loginCredential != null)
            {
                loginCredential.RetrievePassword();
                Email = loginCredential.UserName;
                Password = loginCredential.Password;
                Login();
            }
        }

        private PasswordCredential GetCredentialFromLocker()
        {
            PasswordCredential credential = null;

            var vault = new PasswordVault();
            try
            {
                var credentialList = vault.FindAllByResource("My Bullet Assistant");
                if (credentialList.Count > 0)
                {
                    credential = credentialList[0];
                }
            }
            catch (Exception) { }

            return credential;
        }

        public async void Login()
        {
            // Check the user inputs
            InvalidEmail = string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            InvalidPassword = string.IsNullOrWhiteSpace(password) || !Regex.IsMatch(password, @"((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[\w]).{6,20})");
            if (InvalidEmail || InvalidPassword)
            {
                return;
            }

            // Try to login
            var result = await client.Login(new LoginModel { Email = email, Password = password });
            if (!result)
            {
                // Fail, inform the user.
                HasAuthenticationFailed = true;
                return;
            }

            var vault = new PasswordVault();
            vault.Add(new PasswordCredential(
                "My Bullet Assistant", email, password));

            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(MainPage));
        }
    }
}
