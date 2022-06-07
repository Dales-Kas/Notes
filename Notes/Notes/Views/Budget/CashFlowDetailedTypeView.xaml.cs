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
    public partial class CashFlowDetailedTypeView : ContentPage
    {
        public string searchText = "";

        public List<CashFlowDetailedType> Items { get; set; }

        public CashFlowDetailedTypeView()
        {
            InitializeComponent();

            LoadList();
        }

        public async void LoadList()
        {
            Items = await App.NotesDB.GetCashFlowDetailedTypeAsync();

            if (!string.IsNullOrEmpty(searchText))
            {
                Items = Items.Where(x=>x.Name.ToLower().Contains(searchText.ToLower())).ToList();
            }

            MyListView.ItemsSource = Items.OrderBy(x => x.Name);
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

        private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchText = ((SearchBar)sender).Text;

            LoadList();
        }

        private void RefreshView_Refreshing(object sender, EventArgs e)
        {
            LoadList();
            (sender as RefreshView).IsRefreshing = false;
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Guid id = ((Guid)(e as TappedEventArgs).Parameter);

            if (id!=Guid.Empty)
            {
                await Shell.Current.GoToAsync($"{nameof(CashFlowDetailedTypeForm)}?{nameof(CashFlowDetailedTypeForm.Id)}={id}");                
            }            
        }
    }
}
