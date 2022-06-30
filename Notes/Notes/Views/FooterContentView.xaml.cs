using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Models.Budget;
using Notes.Views.Budget;

namespace Notes.Views
{
    public partial class FooterContentView : ContentView
    {
        public List<ExchangeRatesToShow> Items { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public FooterContentView()
        {
            InitializeComponent();
            
            Date = DateTime.Now;
            LoadList();
        }

        public async void LoadList()
        {
            var ItemsTemp = await App.NotesDB.SelectAllFrom<ExchangeRates>();

            Items = ItemsTemp
                .Where(x => x.Period == new DateTime(Date.Year, Date.Month, Date.Day))
                .Select(x=> new ExchangeRatesToShow(x))
                .OrderByDescending(x => x.CurrencyID)
                .ToList();           

            MyListView.ItemsSource = Items;
        }
        private void RefreshView_Refreshing(object sender, EventArgs e)
        {
            LoadList();
            (sender as RefreshView).IsRefreshing = false;
        }

        private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            Date = e.NewDate;

            LoadList();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = false;
            await Shell.Current.GoToAsync(nameof(ExchangeRatesList),true);            
        }
    }
}