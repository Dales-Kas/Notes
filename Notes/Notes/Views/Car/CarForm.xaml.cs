using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Models.Car;

namespace Notes.Views.Car
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public partial class CarForm : TabbedPage
    {
        public List<CarDescription> ListCarDescription { get; set; }
        public List<CarNotes> ListCarNotes { get; set; }
        public string ItemId
        {
            set
            {
                LoadCarInfo(value);
            }
        }
        public CarForm()
        {
            InitializeComponent();
        }

        public async void LoadCarInfo(string value)
        {
            if (value == null)
            {
                return;
            }
            try
            {
                Guid id = Guid.Parse(value);

                Cars car = await App.NotesDB.SelectAsyncFrom<Cars,Guid>(id);

                MainInfo.BindingContext = car;

                CarDescriptionList.ItemsSource = await App.NotesDB.SelectAllAsyncFrom<CarDescription,Guid>(id,name: "CarID");

                CarNotesList.ItemsSource = await App.NotesDB.SelectAllAsyncFrom<CarNotes,Guid>(id, name: "CarID");                

            }
            catch { }
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {

        }

        private async void RefreshView_Refreshing(object sender, EventArgs e)
        {
            //ListCarDescription = await App.NotesDB.GetCarDescriptionAsync(((Cars)MainInfo.BindingContext).ID);
            ListCarDescription = await App.NotesDB.SelectAllAsyncFrom<CarDescription,Guid>(((Cars)MainInfo.BindingContext).ID, name: "CarID");
            CarDescriptionList.BindingContext = ListCarDescription;
            ((RefreshView)sender).IsRefreshing = false;
        }
    }
}