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

        //public Note curNote = null;
        private string strOk = "►";
        private string strNOk = "○";

        public NoteFlags curNoteFlag = null;

        public List<NoteFlags> allNotesflags;
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

        private async void SaveNote(bool closeForm = true)
        {
            Note note = (Note)BindingContext;

            if (new DateTime(note.Date.Year, note.Date.Month, note.Date.Day) <= new DateTime(1900, 1, 1))
            {
                note.Date = DateTime.UtcNow;
                note.NoteTime = new TimeSpan(note.Date.Hour, note.Date.Minute, note.Date.Second);
            }

            else

            {
                note.Date = new DateTime(note.Date.Year, note.Date.Month, note.Date.Day, note.NoteTime.Hours, note.NoteTime.Minutes, note.NoteTime.Seconds);
            }

            if (note.IsList)
            {
                note.Text = "";
                int i = 0;
                foreach (NoteFlags item in allNotesflags)
                {
                    i++;
                    string enter = i > 1 ? "\n" : "";

                    if (item.Finished)
                    {
                        note.Text = note.Text + enter + strOk + item.Text;
                    }
                    else
                    {
                        note.Text = note.Text + enter + strNOk + item.Text;
                    }
                }
            }

            //if (!string.IsNullOrWhiteSpace(note.Text))
            //{
                await App.NotesDB.SaveNoteAsync(note);
            //}
            if (closeForm)
            {
                //await Shell.Current.GoToAsync(nameof(NotePage));            
                await Navigation.PopAsync();
            }
        }

        private void Stepper1_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if ((sender as Stepper).Value != 0)
            {
                TextEditor1.FontSize = e.NewValue;
                NoteDescripton.FontSize = TextEditor1.FontSize;
            }
            Slider1.Value = e.NewValue;
            CurFontSize.Text = e.NewValue.ToString();
        }

        private void Slider1_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if ((sender as Slider).Value != 0)
            {
                TextEditor1.FontSize = e.NewValue;
                NoteDescripton.FontSize = TextEditor1.FontSize;
            }
            Stepper1.Value = e.NewValue;
            CurFontSize.Text = e.NewValue.ToString();
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
                            NoteFlags flag = null;

                            foreach (string item in note.Text.Split('\n'))
                            {
                                i++;

                                bool isFinished = item.StartsWith(strOk);

                                string curText = "";

                                if (item.StartsWith(strOk) || item.StartsWith(strNOk))
                                {
                                    curText = item.Substring(1);
                                }
                                else
                                {
                                    curText = item;
                                }

                                flag = new NoteFlags
                                {
                                    NoteID = note.ID,
                                    Text = curText,
                                    Finished = isFinished
                                };
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

            if (note.IsList == true)
            {
                scrollView1.IsVisible = false;
                collectionViewFlags.IsVisible = true;
                TotalFinished.IsVisible = true;
            }

            else
            {
                scrollView1.IsVisible = true;
                collectionViewFlags.IsVisible = false;
                TotalFinished.IsVisible = false;
            }

            SetListOfFlagsStatus(note);
        }

        private async void LoadAllNoteFlags()
        {
            Note note = (Note)BindingContext;

            allNotesflags = await App.NotesDB.GetNoteFlagsAsync(note.ID);

            collectionView1.ItemsSource = allNotesflags;

            SetListOfFlagsStatus();
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
            if (checkBoxBackground != null && checkBoxText != null)
            {
                checkBoxText.IsChecked = !checkBoxBackground.IsChecked;
            }

        }

        private void checkBoxText_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (checkBoxBackground != null && checkBoxText != null)
            {
                checkBoxBackground.IsChecked = !checkBoxText.IsChecked;
            }
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

        private async void cbFinished_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            //NoteFlags noteFlag = (Note)BindingContext;
            //NoteFlags noteFlag = (NoteFlags)e.CurrentSelection.FirstOrDefault();

            NoteFlags noteFlag = (NoteFlags)collectionView1.SelectedItem;

            if (noteFlag != null)
            {
                //curNoteFlag.Finished = noteFlag.Finished;
                await App.NotesDB.SaveNoteFlagAsync(noteFlag);
                SetListOfFlagsStatus();
            }

            //SaveNote(false);
        }

        private async void CurItem_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.NewTextValue) && e.NewTextValue.EndsWith("\n"))
            {
                (sender as Editor).Text = e.NewTextValue.Replace("\n", "");
                (sender as Editor).Unfocus();

                if (curNoteFlag != null)
                {
                    curNoteFlag.Text = (sender as Editor).Text;
                    await App.NotesDB.SaveNoteFlagAsync(curNoteFlag);
                    //curNoteFlag = null;
                }
                //SaveNote(false);
                //collectionView1.ScrollTo(1);
            }
        }

        private void noteDate_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }

        private void noteTime_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }

        private void ContentPage_Disappearing(object sender, EventArgs e)
        {
            if (!dontSaveNote)
            {
                SaveNote(false);
            }
        }

        private void СolorListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
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

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TextEditor1.Text))
            {
                TextEditor1.Focus();
            }
        }

        private void cbFinished_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //SaveNote(false);
        }

        private void collectionView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            curNoteFlag = (NoteFlags)e.CurrentSelection.FirstOrDefault();
        }

        private async void SetListOfFlagsStatus(Note note = null)
        {
            if (note == null)
            {
                note = (Note)BindingContext;
            }           

            if (note.IsList)
            {
                int isFinished = 0;
                allNotesflags = await App.NotesDB.GetNoteFlagsAsync(note.ID);
                int all = allNotesflags.Count;

                foreach (NoteFlags item in allNotesflags)
                {
                    if (item.Finished)
                    {
                        isFinished++;
                    }
                }
                TotalFinished.Text = isFinished.ToString() + "/" + all.ToString();
            }
            else
            {
                TotalFinished.Text = "";
            }

        }

        private async void AddButton_Clicked(object sender, EventArgs e)
        {
            Note note = (Note)BindingContext;

            if (note.ID == 0 && formIsCreated)
            {
                SaveNote(false);
            }

            if (note != null)
            {
                NoteFlags flag = new NoteFlags
                {
                    NoteID = note.ID,
                    Text = "",
                    Finished = false
                };
                await App.NotesDB.SaveNoteFlagAsync(flag);
            }

            LoadAllNoteFlags();
        }
    }
}