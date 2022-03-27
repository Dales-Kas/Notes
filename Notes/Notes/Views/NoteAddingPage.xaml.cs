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

        public NoteAddingPage()
        {
            InitializeComponent();
            
            BindingContext = new Note();
        }

        private async void LoadNote(string value)
        {
            try
            {
                int id = Convert.ToInt32(value);

                Note note = await App.NotesDB.GetNoteAsync(id);

                BindingContext = note;
            }
            catch { }
        }

        private async void OnSaveButton_Clicked(object sender, EventArgs e)
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
            Note note = (Note)BindingContext;
            await App.NotesDB.DeleteNoteAsync(note);
            //await Shell.Current.GoToAsync(nameof(NotePage));
            await Navigation.PopAsync();
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
    }
}