using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace Lumi.Services
{
    public class AzureServices<T>
    {
        IMobileServiceClient Client;
        IMobileServiceTable<Model.Luminosidade> Table;

        public AzureServices()
        {
            string MyAppServiceURL = "http://pedro1teste.azurewebsites.net";
            Client = new MobileServiceClient(MyAppServiceURL);
            Table = Client.GetTable<Model.Luminosidade>();
        }

        public Task<IEnumerable<Model.Luminosidade>> PegaTable()
        {
            return Table.ToEnumerableAsync();
        }

        public async Task Enviar(Model.Luminosidade lumi)
        {
            await Table.InsertAsync(lumi);
            await App.Current.MainPage.DisplayAlert("Sucesso", "Mensagem envida", "Ok");
        }
    }
}
