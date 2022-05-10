using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Models.Budget;
using System.Globalization;

namespace Notes.Views.Budget
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CashFlowOperationsView : TabbedPage
    {
        public List<CashFlowOperations> Items { get; set; }
        public double inAmount0 { get; set; }
        public double outAmount0 { get; set; }

        public double inAmount1 { get; set; }
        public double outAmount1 { get; set; }

        public double inAmount2 { get; set; }
        public double outAmount2 { get; set; }

        //public List<CashFlowOperations> Items0 { get; set; }
        //public List<CashFlowOperations> Items1 { get; set; }
        //public List<CashFlowOperations> Items2 { get; set; }

        public CashFlowOperationsView()
        {
            InitializeComponent();

            LoadAllData();
        }

        public async void LoadAllData()
        {
            Items = await App.NotesDB.GetCashFlowOperationsAsync();

            MyListView0.ItemsSource = Items.Where(x => x.TypeID == 0).OrderByDescending(x => x.Date);
            MyListView1.ItemsSource = Items.Where(x => x.TypeID == 1).OrderByDescending(x => x.Date);
            MyListView2.ItemsSource = Items.Where(x => x.TypeID == 2).OrderByDescending(x => x.Date);
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            LoadAllData();
        }

        private void MyListView0_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null)
            {
                CashFlowOperations operation = (CashFlowOperations)e.CurrentSelection.FirstOrDefault();
                //string operationPath = $"{nameof(NoteAddingPage)}?{nameof(NoteAddingPage.ItemId)}={note.ID.ToString()}";
                //await Shell.Current.GoToAsync(notePath);
            }

        }
    }
}
