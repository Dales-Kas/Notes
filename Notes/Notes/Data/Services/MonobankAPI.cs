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
            monoClient = new HttpClient();
            monoClient.BaseAddress = new Uri("https://api.monobank.ua");
            SetDefaultRequestHeaders();
        }

        static void SetDefaultRequestHeaders()
        {
            monoClient.DefaultRequestHeaders.Accept.Clear();
            monoClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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

        public static async Task<double> GetMonoCurrencyExchRate(int currency1, int currency2, DateTime date)
        {
            var listFromBank = await GetMonoCurrencies();

            Currency curExch  = listFromBank.Where(x => x.currencyCodeA == currency1 && x.currencyCodeB == currency2).FirstOrDefault();

            return curExch==null ? 0 : Math.Round((double)curExch.rateSell,2);
        }

        public static async Task<List<Currency>> GetMonoCurrencies()
        {
            SetMonoBaseSettings();

            string path = "/bank/currency";

            HttpResponseMessage response = await monoClient.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();

                List<Currency> jsonData = JsonConvert.DeserializeObject<List<Currency>>(jsonString);

                //Список усіх валют в програмі:
                List<Currencies> myCurrencyList = await App.NotesDB.SelectAllFrom<Currencies>();

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

                return currenciesInMono;
            }

            return new List<Currency>();
        }

        public async static Task<List<Statement>> GetStatement(DateTime date, int account = 0)
        {
            //https://api.monobank.ua/personal/statement/{account}/{from}/{to}            

            SetMonoBaseSettings();

            monoClient.DefaultRequestHeaders.Add("X-Token", await GetMonoToken());

            int date_from = (int)ConvertValues.GetUnixTime(new DateTime(date.Year, date.Month, 1,0,0,0,0,DateTimeKind.Local));
            int date_to = (int)ConvertValues.GetUnixTime(new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month),23,59,59,59,DateTimeKind.Local),true);

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

        public static async Task<int> GetStatementAndSave(DateTime date, int account = 0)
        {
            List<Statement> statements = await GetStatement(date, account);

            int i = 0;

            foreach (Statement statement in statements)
            {
                var operation = await App.NotesDB.SelectFrom<CashFlowOperations>(strid:statement.id,name: "MonoId");

                if (operation != null)
                {
                    continue;
                }
                else
                {
                    double amount = (double)statement.amount / 100;

                    amount = amount < 0 ? -amount : amount;

                    DateTime operDate = ConvertValues.GetDateFromUnixTime(statement.time);

                    operation = await App.NotesDB.GetCashFlowOperationsAsync(operDate, amount);

                    if (operation == null)
                    {
                        i++;

                        CashFlowOperations newOperation = new CashFlowOperations()
                        {
                            MonoId = statement.id,
                            Date = operDate,
                            OperationType = statement.amount > 0 ? Models.OperationType.InOperation : Models.OperationType.OutOperation,
                            TypeID = 0,
                            Client = Guid.Empty,
                            StorageID = Guid.Empty,
                            DetailedTypeID = Guid.Empty,
                            CurrencyID = statement.currencyCode,
                            Amount = amount,
                            Description = statement.description,
                            AmountСurrency = 0,
                            ExchangeRate = 0,
                            CurrencyExchangeRate = 0,
                            IsIncludeCommission = true,
                            IsСurrencyOperartion = false,
                            IsFixedOperartion = false,
                            AmountCommission = statement.commissionRate/100,
                            AmountDelayCommission = 0,
                            AmountBalanceAfterOperation = statement.balance/100,
                            DontUseInCashFlow = false,
                            AmountInSMS = amount,
                            TextToIdentifyClient = statement.description,
                            MCC = statement.mcc.ToString()
                        };

                        newOperation.ExchangeRate = await App.NotesDB.GetSetExchangeRateAsync(840, new DateTime(operDate.Year,operDate.Month,operDate.Day));
                        newOperation.AmountСurrency = newOperation.ExchangeRate==0 ? 0 : newOperation.Amount / newOperation.ExchangeRate;

                        if (!string.IsNullOrEmpty(newOperation.MCC))
                        {
                            var MCCCode = await App.NotesDB.SelectFrom<MCCCodes>(strid: newOperation.MCC, name: "MCC");

                            if (MCCCode != null)
                                newOperation.DetailedTypeID = MCCCode.DetailedTypeID;
                        }

                        if (!string.IsNullOrEmpty(newOperation.TextToIdentifyClient))
                        {
                            var ClientDescr = await App.NotesDB.SelectFrom<ClientIdentificationTexts>(strid: newOperation.TextToIdentifyClient, name: "Description");

                            if (ClientDescr != null)
                                newOperation.Client = ClientDescr.Client;
                        }

                        if (newOperation.DetailedTypeID == Guid.Empty && newOperation.Client != Guid.Empty)
                        {
                            var client = await App.NotesDB.SelectFrom<Clients>(newOperation.Client,name:"ID");
                            if (client != null)
                                newOperation.DetailedTypeID = client.DefaultCashFlowDetailedType;
                            }

                        await App.NotesDB.SaveAsync(newOperation);
                    }
                }
            }
            return i;
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
