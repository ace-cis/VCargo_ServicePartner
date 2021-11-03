using System;
using System.Collections.Generic;
using System.Text;
using VCargo_ServicePartner.Views;
using Xamarin.Forms;

namespace VCargo_ServicePartner.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand { get; }

        //public Command LoginCommand { get; }

        public string _UserName;
        public string UserName
        {
            get => _UserName;
            set
            {
                _UserName = value;

            }
        }

        public string _Password;
        public string Password
        {
            get => _Password;
            set
            {
                _Password = value;

            }
        }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);

            // LoginCommand = new Command(async () => await OnLoginClickedAsync());
        }


        private async void OnLoginClicked(object obj)
        {

            if (_UserName == "" || _UserName == null)
            {
                await Application.Current.MainPage.DisplayAlert("VCargo", "Login Failed!", "Ok");
                return;
            }

            if (_Password == "" || _Password == null)
            {
                await Application.Current.MainPage.DisplayAlert("VCargo", "Login Failed!", "Ok");
                return;
            }


            // var user = await UserDataStore.GetUserAsync(true);
            var userValidation = await UserDataStore.GetUserAsync(_UserName);

            string UserX = userValidation.userCode;
            string PasswordX = userValidation.pssword;
            string CustomerType = userValidation.position;

            if (UserX.ToString() == _UserName && PasswordX.ToString() == _Password)
            {
                //Application.Current.MainPage = new AppShell();


                if (CustomerType == "Service Partner")
                {


                    Application.Current.Properties["UserCode"] = _UserName;
                   // await Application.Current.MainPage.DisplayAlert("Message", " Login successfully.", "Ok");


                    Application.Current.MainPage = new AppShell();



                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Message", "This application is for Service Partner only.", "Ok");
                    return;
                }




            }
            else
            {

                await Application.Current.MainPage.DisplayAlert("VCargo", " Login failed!", "Ok");
            }




        }


    }
}
