using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Models;

namespace Notes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotePage : ContentPage
    {
        public List<Note> allNotes = null;

        public NotePage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            allNotes = await App.NotesDB.GetNotesAsync();
            collectionView.ItemsSource = allNotes;
            base.OnAppearing();
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

        private async void Alert1_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Увага","повідомлення від програми","ОК");
        } 
        private async void Alert2_Clicked(object sender, EventArgs e)
        {
            bool resault = await DisplayAlert("Увага", "повідомлення від програми", "ОК","Відміна");

            if (resault)
            {
                await DisplayAlert("Ви вибрали", resault.ToString(), "OK");
            }
        }
        private async void Alert3_Clicked(object sender, EventArgs e)
        {
            string resault = await DisplayActionSheet("Виберіть колір","Відміна",null,"Червоний","Синій","Зелений");
        }
        private async void Alert4_Clicked(object sender, EventArgs e)
        {
            string resault = await DisplayPromptAsync("Введіть кількість нотаток","для відображення","ОК","Відміна","Кількість нотаток",initialValue:"3",keyboard:Keyboard.Numeric);

            await DisplayAlert("Вибрано:", resault, "OK");
        }

        private void CurItem_BindingContextChanged(object sender, EventArgs e)
        {
            string curText = ((Label)sender).Text;

            if (curText.Length >= 200)
            {
                curText = string.Concat(curText.Substring(0,100),"\n...\n",curText.Substring(curText.Length-99, 99));
            }
            
            ((Label)sender).Text = curText;


        }

        private void srchEntry_BindingContextChanged(object sender, EventArgs e)
        {

        }

        private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
           var filterList = allNotes.Where(x=>x.Text.Contains(searchBar.Text)).ToList();
            
           collectionView.ItemsSource = filterList;
        }
    }
}