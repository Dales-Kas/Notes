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
    public partial class CurrenciesView : ContentPage
    {
        public List<Currencies> Items { get; set; }

        public CurrenciesView()
        {
            InitializeComponent();

            LoadAllData();
        }

        private async Task LoadAllData()
        {
            MyListView.ItemsSource = await App.NotesDB.SelectAllAsyncFrom<Currencies,int>();
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private void AddButton_Clicked(object sender, EventArgs e)
        {

        }
    }
}
