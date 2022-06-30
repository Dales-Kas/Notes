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
using Notes.Data.Services;

namespace Notes
{
    public partial class App : Application
    {

        static NotesDB notesDB;
        
        public static HttpClient bankGovUAClient = new HttpClient();

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

            //BankGovUA();

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

        public async void BankGovUA()
        {
            bankGovUAClient.BaseAddress = new Uri("https://bank.gov.ua");
            bankGovUAClient.DefaultRequestHeaders.Accept.Clear();
            bankGovUAClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            var myCurrencyList = await App.NotesDB.SelectAllFrom<Currencies>();

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
                BackgroundColor = Color.FromHex("#708090").MultiplyAlpha(0.85),
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
