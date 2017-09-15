using System;
using System.Collections.ObjectModel;
using Lumi.Model;
using Xamarin.Forms;
using Plugin.Geolocator;
using System.Threading.Tasks;

namespace Lumi.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private double _valor;

        public double Valor
        {
            get { return _valor; }
            set { _valor = value; OnPropertyChanged(); }
           
        }

        private double _lat;

        public double Lat
        {
            get { return _lat; }
            set { _lat = value; OnPropertyChanged(); }
        }

        private double _longi;

        public double Longi
        {
            get { return _longi; }
            set { _longi = value; OnPropertyChanged(); }
        }

        private DateTime _data;

        public DateTime Data
        {
            get { return _data; }
            set { _data = value; OnPropertyChanged(); }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged(); SendCommand.ChangeCanExecute(); }
        }

        Random rand;
        public Command SendCommand { get; }

        public ObservableCollection<Luminosidade> Luminosidades { get; set; }

        public MainViewModel()
        {
            Luminosidades = new ObservableCollection<Luminosidade>();
            rand = new Random();
            SendCommand = new Command(async () => await ExecuteSendCommand(), () => !IsBusy);

        }

        async Task ExecuteSendCommand()
        {
            Data = DateTime.Now;
            //PegarPosicao();
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;

            var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(30));
            Lat = position.Latitude;
            Longi = position.Longitude;

            Valor = rand.NextDouble() * 1000;

            var NovoValor = new Luminosidade
            {
                Valor = Valor,
                Lat = Lat,
                Longi = Longi,
                Data = Data
            };

            EnviarAzure(NovoValor);
            
        }
       

        async void EnviarAzure(Luminosidade novoValor)
        {
            if(!IsBusy)
            {
                Exception Error = null;
                try
                {
                    IsBusy = true;
                    var AzureService = new Services.AzureServices<Luminosidade>();
                    await AzureService.Enviar(novoValor);
                }
                catch (Exception ex)
                {

                    Error = ex;
                }

                finally
                {
                    IsBusy = false;
                }
                if (Error != null)
                    await App.Current.MainPage.DisplayAlert("Erro!", Error.Message, "Ok");
                
            }

            return;
        }
    }
}