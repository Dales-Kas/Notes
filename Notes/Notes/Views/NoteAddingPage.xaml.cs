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
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public partial class NoteAddingPage : ContentPage
    {
        public bool formIsCreated = false;
        public string ItemId

        {
            set
            {
                LoadNote(value);
            }
        }

        private string NoteText = "";

        private bool addToolsView = false;

        private bool dontSaveNote = false;
        public NoteAddingPage()
        {
            InitializeComponent();

            BindingContext = new Note();

            stackLayoutAddtools.IsVisible = false;
            gridAddtools.IsVisible = false;

            SetElementsVisability();
            
        }

        private async void LoadNote(string value)
        {
            try
            {
                int id = Convert.ToInt32(value);

                Note note = await App.NotesDB.GetNoteAsync(id);

                BindingContext = note;

                NoteText = note.Text;
                
                formIsCreated = true;
            }
            catch { }
        }

        private void OnSaveButton_Clicked(object sender, EventArgs e)
        {
            SaveNote();
        }
        private async void SaveNote(bool closeForm = true)
        {
            Note note = (Note)BindingContext;
            note.Date = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(note.Text))
            {
                await App.NotesDB.
                    SaveNoteAsync(note);
            }
            //await Shell.Current.GoToAsync(nameof(NotePage));            
            if (closeForm)
            {
                await Navigation.PopAsync();
            }            
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
            if (!dontSaveNote)
            {
                SaveNote(false);
            }
            base.OnDisappearing();

        }

        private async void IsList_Toggled(object sender, ToggledEventArgs e)
        {
            Note note = (Note)BindingContext;

            if (note != null)
            {

                if (note.IsList == true)
                {

                    if (formIsCreated)
                    {
                        App.NotesDB.DeleteNoteFlagsAsync(note.ID);

                        if (!string.IsNullOrEmpty(note.Text))
                        {
                            int i = 0;
                            //NoteFlags flag = null;

                            foreach (var item in note.Text.Split('\n'))
                            {
                                i++;
                                NoteFlags flag = new NoteFlags();
                                flag.NoteID = note.ID;
                                flag.Text = item;
                                flag.Finished = false;
                                await App.NotesDB.SaveNoteFlagAsync(flag);
                            }
                        }
                    }
                    
                    LoadAllNoteFlags();
                }

                else
                {
                    //TextEditor1.IsVisible = true;
                }

                SetElementsVisability();

            }
        }

        public void SetElementsVisability()
        {
            Note note = (Note)BindingContext;

            if (note.IsList==true)
            {
                scrollView1.IsVisible = false;
                collectionViewFlags.IsVisible = true;
            }

            else
            {
                scrollView1.IsVisible = true;
                collectionViewFlags.IsVisible = false;
            }
        }

        private async void LoadAllNoteFlags()
        {
            Note note = (Note)BindingContext;

            var allNotesflags = await App.NotesDB.GetNoteFlagsAsync(note.ID);

            collectionView1.ItemsSource = allNotesflags;
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

        private void colorListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Color curColor = (e.SelectedItem as NamedColor).Color;

            if (checkBoxText.IsChecked)
            {
                TextEditor1.TextColor = curColor;
                ColorTextEntry.Text = curColor.ToHex().ToString();
            }
            else
            {
                MainStackLayout.BackgroundColor = curColor;
                TextEditor1.BackgroundColor = MainStackLayout.BackgroundColor;
                ColorBackgroundEntry.Text = curColor.ToHex().ToString();
            }

            
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            colorListView.IsVisible = !colorListView.IsVisible;

            if (colorListView.IsVisible)
            {
                if (colorListView.SelectedItem != null)
                {
                    colorListView.ScrollTo(colorListView.SelectedItem,
                                           ScrollToPosition.MakeVisible,
                                           true);
                }
            }
        }

        private void checkBoxBackground_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            checkBoxText.IsChecked = !checkBoxBackground.IsChecked;            
        }

        private void checkBoxText_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            checkBoxBackground.IsChecked = !checkBoxText.IsChecked;
        }

        private void toDelete_Invoked(object sender, EventArgs e)
        {

        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }

        private void deleteBtn_Clicked(object sender, EventArgs e)
        {
            Note note = (Note)BindingContext;

            if (note != null)
            {
                App.NotesDB.DeleteNoteFlagsAsync(note.ID);
                LoadAllNoteFlags();
            }
        }

        private void cbFinished_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            
        }

        private void CurItem_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.NewTextValue) && e.NewTextValue.EndsWith("\n"))
            {
                (sender as Editor).Text = e.NewTextValue.Replace("\n", "");
                (sender as Editor).Unfocus();
                //collectionView1.ScrollTo(1);
            }
        }
    }
}