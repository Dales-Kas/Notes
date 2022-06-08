using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using Notes.Models.Budget;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Data.Services
{
    public static class MonobankAPI
    {
        public static HttpClient monoClient = new HttpClient();

        public static void SetMonoBaseSettings()
        {
            HttpClient monoClient = new HttpClient();
            monoClient.BaseAddress = new Uri("https://api.monobank.ua");
            SetDefaultRequestHeaders();
        }

        static void SetDefaultRequestHeaders()
        {            
            monoClient.DefaultRequestHeaders.Accept.Clear();
            monoClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static async Task GetMonoCurrencies()
        {
            SetMonoBaseSettings();

            string path = "/bank/currency";

            HttpResponseMessage response = await monoClient.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();

                List<Currency> jsonData = JsonConvert.DeserializeObject<List<Currency>>(jsonString);

                //Список усіх валют в програмі:
                List<Currencies> myCurrencyList = await App.NotesDB.GetCurrenciesAsync();

                var currenciesInMono = new List<Currency>();

                foreach (Currency item in jsonData)
                {
                    if (myCurrencyList.Count(x => x.Code == item.currencyCodeA && item.currencyCodeB == 980) > 0)
                    {
                        var sec = TimeSpan.FromSeconds(item.date);
                        var date = new DateTime(sec.Ticks);

                        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                        dateTime = dateTime.AddSeconds(item.date).ToLocalTime();
                        item.dateInFile = dateTime;
                        currenciesInMono.Add(item);
                    }
                }

            }
        }

        public async static Task<List<Statement>> GetStatement(DateTime date, int account=0)
        {
            //https://api.monobank.ua/personal/statement/{account}/{from}/{to}            

            SetMonoBaseSettings();
            string token = await GetMonoToken();
            monoClient.DefaultRequestHeaders.Add("X-Token", token);

            double date_from = (new DateTime(date.Year, date.Month,1) - new DateTime(1970,1,1)).TotalSeconds;
            double date_to = (new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month)) - new DateTime(1970, 1, 1)).TotalSeconds;
            
            string path = $"/personal/statement/{account}/{date_from}/{date_to}";

            HttpResponseMessage response = await monoClient.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();

                List<Statement> jsonData = JsonConvert.DeserializeObject<List<Statement>>(jsonString);

                return jsonData;                
            }

            return new List<Statement>();
        }

        private async static Task<string> GetMonoToken()
        {
            var prSettings = await App.NotesDB.GetProgramSettingsAsync();

            if (string.IsNullOrEmpty(prSettings.MonoToken))
            {
                //await DisplayAlert("Не заповнено token для Monobank!", "", "ОК");
            }

            return prSettings.MonoToken;
        }

        #region AdditionalClassesToTakeDataFromAPI
        public class Statement
        {
            public string id;

            public Int64 time;

            public string description;

            public int mcc;

            public int originalMcc;

            public bool hold;

            public Int64 amount;

            public Int64 operationAmount;

            public int currencyCode;

            public Int64 commissionRate;

            public Int64 cashbackAmount;

            public Int64 balance;

            public string comment;

            public string receiptId;

            public string invoiceId;

            public string counterEdrpou;

            public string counterIban;
        }

        public class Currency
        {
            public int currencyCodeA;
            public int currencyCodeB;
            public Int64 date;
            public float rateSell;
            public float rateBuy;
            public float rateCross;
            public DateTime dateInFile;
        }
        #endregion
    }
}
