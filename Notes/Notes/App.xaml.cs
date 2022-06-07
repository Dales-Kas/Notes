using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Data;
using System.IO;
using Xamarin.CommunityToolkit.UI.Views.Options;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using Notes.Models.Budget;

namespace Notes
{
    public partial class App : Application
    {

        static NotesDB notesDB;

        static HttpClient monoClient = new HttpClient();
        static HttpClient bankGovUAClient = new HttpClient();

        public static NotesDB NotesDB
        {
            get
            {
                if (notesDB == null)
                {
                    string connString = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NotesDataBase.db3");
                    notesDB = new NotesDB(connString);
                }
                return notesDB;
            }
        }

        public App()
        {
            //Device.SetFlags(new string[] { "SwipeView_Experimental", "AppTheme_Experimental" });

            InitializeComponent();

            MainPage = new AppShell();
            //MainPage = new MainPage();

            //Для тесту пробую одержати курси валют з різних бвнків, потрібно буде потім це перекинути у правильний namespace...

            MonoAPI();

            BankGovUA();

        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        public async void MonoAPI()
        {
            monoClient.BaseAddress = new Uri("https://api.monobank.ua");
            monoClient.DefaultRequestHeaders.Accept.Clear();
            monoClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            string path = "/bank/currency";

            HttpResponseMessage response = await monoClient.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();

                List<MonoCurrency> jsonData = JsonConvert.DeserializeObject<List<MonoCurrency>>(jsonString);

                var myCurrencyList = await App.NotesDB.GetCurrenciesAsync();

                var currenciesInMono = new List<MonoCurrency>();

                foreach (MonoCurrency item in jsonData)
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

        public async void BankGovUA()
        {
            bankGovUAClient.BaseAddress = new Uri("https://bank.gov.ua");
            bankGovUAClient.DefaultRequestHeaders.Accept.Clear();
            bankGovUAClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            var myCurrencyList = await App.NotesDB.GetCurrenciesAsync();

            string curDate = DateTime.Now.ToString("yyyMMdd");

            foreach (Currencies item in myCurrencyList)
            {
                string path = $"NBUStatService/v1/statdirectory/exchange?valcode={item.CharCode}&date={curDate}&json";

                HttpResponseMessage response = await bankGovUAClient.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();

                    List<BankGovUACurrency> jsonData = JsonConvert.DeserializeObject<List<BankGovUACurrency>>(jsonString);

                    if (jsonData.Count>0)
                    {
                        float curRate = jsonData[0].rate;
                    }

                }
            }
        }

        public class MonoCurrency
        {
            public int currencyCodeA;
            public int currencyCodeB;
            public Int64 date;
            public float rateSell;
            public float rateBuy;
            public float rateCross;
            public DateTime dateInFile;
        }

        public class BankGovUACurrency
        {
            public int r030;
            public string txt;
            public float rate;
            public string cc;
            public string exchangedate;

        }

        public static ToastOptions GetToastOptions(string messageText)
        {
            var toastOptions = new ToastOptions()
            {
                BackgroundColor = Color.FromHex("#2196F3").MultiplyAlpha(0.85),
                CornerRadius = 25,
                Duration = TimeSpan.FromSeconds(1),
                MessageOptions = new MessageOptions()
                {
                    Message = messageText,
                    Foreground = Color.White,
                    Padding = 30
                }

            };

            return toastOptions;
        }
    }
}
