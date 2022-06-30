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
    public partial class ClientsView : ContentPage
    {
        public List<ClientsToShow> Items { get; set; }
        public List<Clients> ParentsItems { get; set; }

        public Guid CurrentParentGuid = Guid.Empty;

        public string CurrentParentName { get; set; }
        public ClientsView()
        {
            InitializeComponent();

            OnFormLoad();
        }

        private async Task OnFormLoad()
        {
            var ItemsClients = (await App.NotesDB.SelectAllAsyncFrom<Clients,Guid>()).OrderBy(x => x.Name);

            Items = new List<ClientsToShow>();

            foreach (var item in ItemsClients)
            {
                ClientsToShow newClient = new ClientsToShow
                {
                    ID = item.ID,
                    ParentID = item.ParentID,
                    IsGroup = item.IsGroup,
                    Name = item.Name,
                    Comment = item.Comment,
                    DefaultCashFlowDetailedType = item.DefaultCashFlowDetailedType,
                    PercentageCommission = item.PercentageCommission
                };

                if (newClient.IsGroup)
                {
                    newClient.FullName = "_" + newClient.Name;
                }
                else
                {
                    newClient.FullName = newClient.Name;
                }

                Items.Add(newClient);
            }

            //Items.OrderBy(x => x.FullName);

            ParentsItems = ItemsClients.ToList();
            ParentsItems.Clear();

            LoadClientsList();

            //backParentButton.BindingContext = this;

        }

        public async void LoadClientsList(bool addClient = true)
        {
            MyListView.ItemsSource = Items.Where(x => x.ParentID == CurrentParentGuid).OrderBy(x=>x.FullName).ToList();

            if (CurrentParentGuid == Guid.Empty)
            {
                ParentsItems?.Clear();
                CurrentParentName = "";
                backParentButton.IsVisible = false;
                pathURL.IsVisible = false;
            }
            else
            {
                if (addClient)
                {
                    Clients client = await App.NotesDB.SelectAsyncFrom<Clients,Guid>(CurrentParentGuid);
                    ParentsItems.Add(client);
                    backParentButton.IsVisible = true;
                    backParentButton.Text = client.Name;
                    CurrentParentName = client.Name;
                    pathURL.IsVisible = true;
                }
            }

            string pathStr = "Контрагенти";

            for (int i = 0; i < ParentsItems.Count - 1; i++)
            {

                if (i == ParentsItems.Count - 1)
                {
                    break;
                }

                pathStr += "/" + ParentsItems[i].Name;
            }

            pathURL.Text = pathStr + "/";
            //MyParentsListView.ItemsSource = ParentsItems;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private void MyListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null)
            {
                CurrentParentGuid = ((Clients)e.CurrentSelection.FirstOrDefault()).ID;
                CurrentParentName = ((Clients)e.CurrentSelection.FirstOrDefault()).Name;
                LoadClientsList();

                //CashFlowOperations operation = (CashFlowOperations)e.CurrentSelection.FirstOrDefault();
                //string operationPath = $"{nameof(NoteAddingPage)}?{nameof(NoteAddingPage.ItemId)}={note.ID.ToString()}";
                //await Shell.Current.GoToAsync(notePath);
            }
        }

        private void backParentButton_Clicked(object sender, EventArgs e)
        {
            Clients client = ParentsItems.ElementAt(ParentsItems.Count - 1);
            ParentsItems.Remove(client);

            if (ParentsItems.Count == 0)
            {
                CurrentParentGuid = Guid.Empty;
            }

            else
            {
                CurrentParentGuid = ParentsItems.ElementAt(ParentsItems.Count - 1).ID;
                CurrentParentName = ParentsItems.ElementAt(ParentsItems.Count - 1).Name;
                backParentButton.Text = CurrentParentName;
            }

            LoadClientsList(false);

        }

        private void AddButton_Clicked(object sender, EventArgs e)
        {

        }

        //private void MyParentsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (e.CurrentSelection != null)
        //    {
        //        Clients client = ((Clients)e.CurrentSelection.FirstOrDefault());
        //        ParentsItems.Remove(client);

        //        if (ParentsItems.Count==0)
        //        {
        //            CurrentParentGuid = Guid.Empty;
        //        }

        //        else
        //        {
        //            CurrentParentGuid = ParentsItems.ElementAt(ParentsItems.Count-1).ID;
        //        }

        //        LoadClientsList();             

        //    }
        //}
    }
}
