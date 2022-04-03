using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Models;
using Notes.Data;

namespace Notes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotePage : CarouselPage
    {
        public List<Note> allNotes = null;

        public NotePage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            UpdateNotesList();

            base.OnAppearing();
        }

        public async void UpdateNotesList()
        {
            allNotes = await App.NotesDB.GetNotesAsync();
            collectionView.ItemsSource = allNotes.Where(x => x.IsArchived == false);
            collectionView1.ItemsSource = allNotes.Where(x => x.IsArchived == true);
        }


        private async void AddButton_Clicked(object sender, EventArgs e)
        {
            //await Shell.Current.GoToAsync(nameof(NoteAddingPage));
            await Navigation.PushAsync(new NoteAddingPage());
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null)
            {
                Note note = (Note)e.CurrentSelection.FirstOrDefault();
                string notePath = $"{nameof(NoteAddingPage)}?{nameof(NoteAddingPage.ItemId)}={note.ID.ToString()}";
                await Shell.Current.GoToAsync(notePath);
                //NoteAddingPage myPage = new NoteAddingPage();
                //await Navigation.PushAsync(myPage);
            }
        }

        #region ToDelete

        private async void Alert1_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Увага", "повідомлення від програми", "ОК");
        }
        private async void Alert2_Clicked(object sender, EventArgs e)
        {
            bool resault = await DisplayAlert("Увага", "повідомлення від програми", "ОК", "Відміна");

            if (resault)
            {
                await DisplayAlert("Ви вибрали", resault.ToString(), "OK");
            }
        }
        private async void Alert3_Clicked(object sender, EventArgs e)
        {
            string resault = await DisplayActionSheet("Виберіть колір", "Відміна", null, "Червоний", "Синій", "Зелений");
        }
        private async void Alert4_Clicked(object sender, EventArgs e)
        {
            string resault = await DisplayPromptAsync("Введіть кількість нотаток", "для відображення", "ОК", "Відміна", "Кількість нотаток", initialValue: "3", keyboard: Keyboard.Numeric);

            await DisplayAlert("Вибрано:", resault, "OK");
        }

        #endregion

        private void CurItem_BindingContextChanged(object sender, EventArgs e)
        {
            string curText = ((Label)sender).Text;

            if (curText.Length >= 200)
            {
                curText = string.Concat(curText.Substring(0, 100), "\n...\n", curText.Substring(curText.Length - 99, 99));
            }

            ((Label)sender).Text = curText;


        }

        private void srchEntry_BindingContextChanged(object sender, EventArgs e)
        {

        }

        private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = ((SearchBar)sender).Text;
            FilterLists("searchBar", searchText);
        }

        private void FilterLists(string nameOfList, string searchText)
        {
            if (nameOfList == "searchBar")
            {
                searchBar1.Text = searchText;
            }

            if (nameOfList == "searchBar1")
            {
                searchBar.Text = searchText;
            }

            var filterList = allNotes.Where(x => x.Text.Contains(searchText) && x.IsArchived == false).ToList();
            var filterList1 = allNotes.Where(x => x.Text.Contains(searchText) && x.IsArchived == true).ToList();

            collectionView.ItemsSource = filterList;
            collectionView1.ItemsSource = filterList1;
        }


        private void searchBar1_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = ((SearchBar)sender).Text;
            FilterLists("searchBar1", searchText);
        }

        private async void toArchiv1_Invoked(object sender, SwipedEventArgs e)
        {
            int id = Convert.ToInt32((sender as MenuItem).CommandParameter);

            if (id != 0)
            {               
                Note note = await App.NotesDB.GetNoteAsync(id);
                note.IsArchived = !note.IsArchived;
                await App.NotesDB.SaveNoteAsync(note);

                UpdateNotesList();
                //DisplayAlert("Відправлено в архів...", "", "ОК");
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            TappedEventArgs curEventArg = (TappedEventArgs)e;

            if (curEventArg.Parameter != null)
            {
                string notePath = $"{nameof(NoteAddingPage)}?{nameof(NoteAddingPage.ItemId)}={curEventArg.Parameter.ToString()}";
                await Shell.Current.GoToAsync(notePath);
            }
        }

        private async void toDelete_Invoked(object sender, EventArgs e)
        {
            bool resault = await DisplayAlert("Нотатку буде видалено", "Бажаєте продовжити?", "Так", "Ні, не видаляти");

            if (resault)
            {
                int id = Convert.ToInt32((sender as MenuItem).CommandParameter);

                if (id != 0)
                {
                    Note note = await App.NotesDB.GetNoteAsync(id);
                    await App.NotesDB.DeleteNoteAsync(note);

                    UpdateNotesList();
                }
            }
        }

        private void toArchiv1_Invoked(object sender, EventArgs e)
        {

        }
    }
}