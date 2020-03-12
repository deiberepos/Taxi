using Prism.Commands;
using Prism.Navigation;
using System.Text.RegularExpressions;
using Taxi.Common.Models;
using Taxi.Common.Services;

namespace Taxi.Prism.ViewModels
{
    public class TaxiHistoryPageViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;
        private TaxiResponse _taxi;
        private DelegateCommand _checkPlaqueCommand;
        private bool _isRunning;

        public TaxiHistoryPageViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            _apiService = apiService;
            Title = "Taxi History";
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public TaxiResponse Taxi
        {
            get => _taxi;
            set => SetProperty(ref _taxi, value);
        }

        public string Plaque { get; set; }

        public DelegateCommand CheckPlaqueCommand => _checkPlaqueCommand ?? (_checkPlaqueCommand = new DelegateCommand(CheckPlaqueAsync));

        //Método que se ejecuta al ingresar una placa y presiona el boton
        private async void CheckPlaqueAsync()
        {
            //Validación de que se ha ingresado algo en en entry
            if (string.IsNullOrEmpty(Plaque))
            {
                //Mensaje de alerta de que no ha ingresado nada
                await App.Current.MainPage.DisplayAlert(
                    "Error",//titulo de la ventana
                    "You must enter a plaque.",//mensaje
                    "Accept");//boton
                return;
            }
            //Aca no tenemos DataAnotation para la validación
            //Se crea una expresión regular para que la placa
            //cumpla con empezar con 3 letras y terminar con 3 números
            Regex regex = new Regex(@"^([A-Za-z]{3}\d{3})$");
            //Si la placa no cumple con esto le muestra una ventana con mensaje de eeror
            if (!regex.IsMatch(Plaque))
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "The plaque must start with three letters and end with three numbers.",
                    "Accept");
                return;
            }

            IsRunning = true;
            //Si la placa ingresada cumple
            //Va al diccionario de recursos y devuelve la direccion de publicación
            string url = App.Current.Resources["UrlAPI"].ToString();
            //Usamos la clase Respuesta creada anteriormente
            //Le enviamos los datos necesarios la url y el controlador
            Response response = await _apiService.GetTaxiAsync(Plaque, url, "api", "/Taxis");
            //Debemos manejar la respuesta si pudo o no realizar la petición
            IsRunning = false;
            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Accept");
                return;
            }
            //Si pudo realizar la consulta
            //Casteamos la respuesta a la propiedad Taxi
            Taxi = (TaxiResponse)response.Result;
        }
    }
}
