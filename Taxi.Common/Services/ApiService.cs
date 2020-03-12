using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Taxi.Common.Models;

namespace Taxi.Common.Services
{
    public class ApiService : IApiService
    {
        public async Task<Response> GetTaxiAsync(string plaque, string urlBase, string servicePrefix, string controller)
        {
            //Todo dentro de un try-catch
            try
            {
                //1 Crear el HttpClient
                HttpClient client = new HttpClient
                {//La url viene como parámentro
                    BaseAddress = new Uri(urlBase),
                };

                //Definimos la url con  el prefijo (api)
                //nombre del controlador y la placa
                string url = $"{servicePrefix}{controller}/{plaque}";
                //Realizamos la peticion
                HttpResponseMessage response = await client.GetAsync(url);
                //la leemos en este caso com un string
                string result = await response.Content.ReadAsStringAsync();

                //Si la comunicación falla
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }
                //Si la comunicación es exitosa
                //Deserealizamos el string a objeto
                TaxiResponse model = JsonConvert.DeserializeObject<TaxiResponse>(result);
                return new Response
                {
                    IsSuccess = true,
                    Result = model
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<bool> CheckConnectionAsync(string url)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                return false;
            }

            return await CrossConnectivity.Current.IsRemoteReachable(url);
        }

    }

}
