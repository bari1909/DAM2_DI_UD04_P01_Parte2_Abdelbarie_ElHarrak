using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Veterinario_MVVM.Models;
using Veterinario_MVVM.Services;
using Veterinario_MVVM.ViewModels.Base;
using Veterinario_MVVM.Views;

namespace Veterinario_MVVM.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        // 1. Atributos
        private readonly AuthService _authService = new AuthService();
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _mensajeError = string.Empty;
        private bool _hayError;
        private bool _isBusy;

        // 2. Propiedades observables: Username, Password, MensajeError, HayError e IsBusy
        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value))
                {
                    HayError = false;
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                {
                    HayError = false;
                }
            }
        }

        public string MensajeError
        {
            get => _mensajeError;
            set => SetProperty(ref _mensajeError, value);
        }

        public bool HayError
        {
            get => _hayError;
            set => SetProperty(ref _hayError, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        // 3. Comandos
        public ICommand LoginCommand { get; }
        public ICommand SalirCommand { get; }

        // 4. Constructor
        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(async p => await PerformLoginAsync(p), p => CanExecuteLogin());
            SalirCommand = new RelayCommand(p => PerformSalir(p));
        }

        // 5. Métodos Execute y CanExecute
        private bool CanExecuteLogin()
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password) && !IsBusy;
        }

        private async Task PerformLoginAsync(object? parametro)
        {
            IsBusy = true;
            try
            {
                // Simulamos el tiempo de comprobación de las credenciales
                await Task.Delay(600);

                Usuario? usuario = _authService.ValidarLogin(Username.Trim(), Password);

                if (usuario == null)
                {
                    MensajeError = "Usuario o contraseña incorrectos. Revisa los datos e inténtalo de nuevo.";
                    HayError = true;
                    return;
                }

                // Navegación limpia: instancia MainView con su MainViewModel y cierra la ventana actual
                MainView mainView = new MainView();
                mainView.DataContext = new MainViewModel(usuario);
                mainView.Show();

                if (parametro is Window ventanaLogin)
                {
                    ventanaLogin.Close();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void PerformSalir(object? parametro)
        {
            if (parametro is Window ventana)
            {
                ventana.Close();
            }
        }
    }
}
