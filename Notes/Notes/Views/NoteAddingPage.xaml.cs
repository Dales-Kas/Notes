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
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public partial class NoteAddingPage : ContentPage
    {
        public string ItemId

        {
            set
            {
                LoadNote(value);
            }
        }

        private string NoteText = "";

        private bool addToolsView = false;
        public NoteAddingPage()
        {
            InitializeComponent();

            BindingContext = new Note();

            stackLayoutAddtools.IsVisible = false;
            gridAddtools.IsVisible = false;
        }

        private async void LoadNote(string value)
        {
            try
            {
                int id = Convert.ToInt32(value);

                Note note = await App.NotesDB.GetNoteAsync(id);

                BindingContext = note;

                NoteText = note.Text;
            }
            catch { }
        }

        private void OnSaveButton_Clicked(object sender, EventArgs e)
        {
            SaveNote();
        }
        private async void SaveNote()
        {
            Note note = (Note)BindingContext;
            note.Date = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(note.Text))
            {
                await App.NotesDB.
                    SaveNoteAsync(note);
            }
            //await Shell.Current.GoToAsync(nameof(NotePage));            
            await Navigation.PopAsync();
        }

        private async void OnDeleteButton_Clicked(object sender, EventArgs e)
        {
            bool resault = await DisplayAlert("Нотатку буде видалено", "Бажаєте продовжити?", "Так", "Ні, не видаляти");

            if (resault)
            {
                Note note = (Note)BindingContext;
                await App.NotesDB.DeleteNoteAsync(note);
                //await Shell.Current.GoToAsync(nameof(NotePage));
                await Navigation.PopAsync();
            }
        }

        private void Stepper1_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if ((sender as Stepper).Value != 0)
            {
                TextEditor1.FontSize = e.NewValue;
            }
            Slider1.Value = e.NewValue;
            CurFontSize.Text = e.NewValue.ToString();
        }

        private void Slider1_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if ((sender as Slider).Value != 0)
            {
                TextEditor1.FontSize = e.NewValue;
            }
            Stepper1.Value = e.NewValue;
            CurFontSize.Text = e.NewValue.ToString();
        }

        protected override bool OnBackButtonPressed()
        {
            return base.OnBackButtonPressed();
        }

        protected override void OnDisappearing()
        {
            Note note = (Note)BindingContext;

            if (NoteText != note.Text && (note.ID == 0 ? !string.IsNullOrEmpty(note.Text) : true))
            {
                //const string text_Ok = "Так, вийти";
                const string text_Save = "Так";
                const string text_NOK = "Ні";

                Device.BeginInvokeOnMainThread(async () =>
                {
                    //your logic             
                    bool resault = await DisplayAlert("Зберегти нотатку?", "", text_Save, text_NOK);

                    if (resault)
                    {
                        SaveNote();
                    }

                });

                //return true; //you handled the back button press

            }

            else
            {
                //return true;//base.OnBackButtonPressed();
            }
            //return true;

            //base.OnDisappearing();
        }

        private void IsList_Toggled(object sender, ToggledEventArgs e)
        {
            Note note = (Note)BindingContext;

            if (note != null)
            {

                if (note.IsList == true)
                {
                    TextEditor1.IsVisible = false;
                }

                else
                {
                    TextEditor1.IsVisible = true;
                }

                if (!string.IsNullOrEmpty(note.Text))
                {
                    foreach (var item in note.Text.Split('\n'))
                    {
                        TextCell cell = new TextCell();
                        cell.Text = item;

                        //scrollView1.
                            //Children.Add(cell);
                    }
                }

            }

        }

        private void toolbarItem1_Clicked(object sender, EventArgs e)
        {
            addToolsView = !addToolsView;

            if (addToolsView)
            {
                stackLayoutAddtools.IsVisible = true;
                gridAddtools.IsVisible = true;
            }

            else
            {
                stackLayoutAddtools.IsVisible = false;
                gridAddtools.IsVisible = false;
            }            
        }
    }
}