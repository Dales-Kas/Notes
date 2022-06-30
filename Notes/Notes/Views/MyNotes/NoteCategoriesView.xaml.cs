using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Models;

namespace Notes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoteCategoriesView : ContentPage
    {
        public List<NoteCategory> Items { get; set; }

        public NoteCategoriesView()
        {
            InitializeComponent();

            LoadList();
            //Items = await App.NotesDB.GetNotesCategoriesAsync();
            //    new ObservableCollection<string>
            //{
            //    "Item 1",
            //    "Item 2",
            //    "Item 3",
            //    "Item 4",
            //    "Item 5"
            //};

            //MyListView.ItemsSource = Items;
        }
        public async void LoadList()
        {
            Items = await App.NotesDB.SelectAllFrom<NoteCategory>();
            MyListView.ItemsSource = Items;
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            //await DisplayAlert("Item Tapped", "An item was tapped.", "OK");
            string curID = ((NoteCategory)e.Item).ID.ToString();

            OpenOrEditCell(curID,false);

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        public async void OpenOrEditCell(string curID, bool flOpen)
        {            
            string route = null;

            if (flOpen)
            {
                route = $"{nameof(NoteCategoryForm)}?{nameof(NoteCategoryForm.CategoryId)}={curID}";
            }
            else
            {
                route = $"..?{nameof(NoteAddingPage.CategoryID)}={curID}";
            }

            await Shell.Current.GoToAsync(route);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            string notePath = nameof(NoteCategoryForm);
            await Shell.Current.GoToAsync(notePath, true);
        }

        private void MyListView_Refreshing(object sender, EventArgs e)
        {
            LoadList();
            MyListView.IsRefreshing = false;
        }

        private void BtnEdit_Invoked(object sender, EventArgs e)
        {
            string curID = ((sender as MenuItem).CommandParameter).ToString();
            OpenOrEditCell(curID, true);
            //if (e.Item == null)
            //    return;

        }
    }
}
