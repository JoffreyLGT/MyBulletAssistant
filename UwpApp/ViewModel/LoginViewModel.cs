using Core.Data;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UwpApp.Base;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UwpApp.ViewModel
{
    public class LoginViewModel : Observable
    {

        private readonly MbaApiClient client;
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
            client = new MbaApiClient(new Uri("http://localhost:58665/"));
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
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(MainPage), client);
        }
    }
}
