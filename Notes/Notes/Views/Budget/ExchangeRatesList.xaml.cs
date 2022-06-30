using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Models.Budget;

namespace Notes.Views.Budget
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExchangeRatesList : ContentPage
    {
        public int selectedCurrency = 0;

        public List<Currencies> listOfCurrencies = null;

        public List<ExchangeRatesToShow> Items { get; set; }

        public ExchangeRatesList()
        {
            InitializeComponent();            
            SetCurrenciesListToPick(true);            
            //LoadList();
        }

        public async void LoadList()
        {
            var ItemsCur = await App.NotesDB.SelectAllFrom<ExchangeRates>();

            if (selectedCurrency!=0)
            {
                int selectedCurrencyCode = listOfCurrencies[selectedCurrency-1].Code;

                ItemsCur = ItemsCur.Where(x=>x.CurrencyID == selectedCurrencyCode).ToList();
            }

            Items = ItemsCur.Select(x=>new ExchangeRatesToShow(x)).OrderByDescending(x => x.Period).ToList();

            MyListView.ItemsSource = Items;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private void RefreshView_Refreshing(object sender, EventArgs e)
        {
            LoadList();
            (sender as RefreshView).IsRefreshing = false;
        }

        private async void SetCurrenciesListToPick(bool setSelectedItem = false)
        {
            listOfCurrencies = await App.NotesDB.SelectAllFrom<Currencies>();
            
            string[] vs = new string[listOfCurrencies.Count+1];

            vs[0] = "Всі валюти";

            int i = 1;

            foreach (Currencies currencies in listOfCurrencies) 
            {                
                vs[i] = currencies.Descripton;
                i++;
            }

            (pkrCurrency as Picker).ItemsSource = vs;

            if (setSelectedItem)
            {
                (pkrCurrency as Picker).SelectedItem = vs[0];
            }
        } 
        
        private void pkrCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedCurrency = ((Picker)sender).SelectedIndex;

            LoadList();
        }

        private void AddButton_Clicked(object sender, EventArgs e)
        {

        }
    }
}
