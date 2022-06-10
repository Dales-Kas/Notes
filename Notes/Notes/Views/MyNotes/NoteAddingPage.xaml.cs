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
    [QueryProperty(nameof(IsNewForm), nameof(IsNewForm))]
    [QueryProperty(nameof(CategoryID), nameof(CategoryID))]
    public partial class NoteAddingPage : ContentPage
    {
        public bool formIsCreated = false;
        private Note currentNote = null;
        public string ItemId
        {
            set
            {
                LoadNote(value);
            }
        }

        public bool IsNewForm
        {
            set { formIsCreated = value; }
        }
        public string CategoryID
        {
            set
            {
                Note note = (Note)BindingContext;

                if (note != null)
                {
                    int.TryParse(value, out int newId);
                    note.Category = newId;

                    SetCategoryName(newId);
                }
            }

            get
            {
                return SetCategoryName(currentNote.Category);
            }
        }

        private string SetCategoryName(int curCategoryID)
        {
            NoteCategory.Text = "";

            NoteCategory noteCategory = App.NotesDB.GetNoteCategoryAsync(curCategoryID).Result;

            if (noteCategory != null)
            {
                NoteCategory.Text = noteCategory.Name;
            }
            else
            {
                NoteCategory.Text = "";
            }

            return NoteCategory.Text;
        }

        public string NoteCategoryDescr
        {
            get { return App.NotesDB.GetNoteCategory(((Note)BindingContext).Category).Name; }
            set { NoteCategoryDescr = value; }
        }

        private string NoteText = "";

        private bool addToolsView = false;

        private bool dontSaveNote = false;

        //public Note curNote = null;        

        public NoteFlags curNoteFlag = null;

        public List<NoteFlags> allNotesflags;
        public NoteAddingPage()
        {
            currentNote = new Note();

            BindingContext = currentNote;

            InitializeComponent();

            stackLayoutAddtools.IsVisible = false;
            gridAddtools.IsVisible = false;
            //SetCategoryName();

            SetElementsVisability();

        }

        #region PageSaveLoad

        private async void LoadNote(string value)
        {
            if (value == null)
            {
                return;
            }

            try
            {

                Guid id = Guid.Parse(value);

                Note note = await App.NotesDB.GetNoteAsync(id);

                BindingContext = note;

                currentNote = note;
                SetCategoryName(currentNote.Category);

                NoteText = note.Text;

                formIsCreated = true;
            }
            catch { }
        }

        private async void LoadAllNoteFlags()
        {
            Note note = (Note)BindingContext;

            allNotesflags = await App.NotesDB.GetNoteFlagsAsync(note.ID);

            collectionView1.ItemsSource = allNotesflags;

            if (allNotesflags.Count > 0)
            {
                collectionView1.ScrollTo(allNotesflags[(allNotesflags.Count - 1)]);
            }

            SetListOfFlagsStatus();
        }

        private async Task<int> SaveNote(bool closeForm = true)
        {
            Note note = (Note)BindingContext;
            return await App.NotesDB.SaveNoteAsync(note, false, true, false);            
        }

        #endregion

        #region PageEvents
        private async void ContentPage_Disappearing(object sender, EventArgs e)
        {
            if (curNoteFlag != null)
            {
                await App.NotesDB.SaveAsync(curNoteFlag);
            }
            if (!dontSaveNote)
            {
                await SaveNote(false);
            }
        }

        #endregion


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
                        if (note.ID == Guid.Empty)
                        {
                            int rowID = await SaveNote(false);

                            //if (note.ID==Guid.Empty)
                            //{
                            //    //SELECT guid FROM Foo WHERE rowid = x;
                            //    Note newNoteInBD = await App.NotesDB.GetNoteAsync(rowID);
                            //    if (newNoteInBD != null)
                            //    {
                            //        note.ID = newNoteInBD.ID;
                            //    }
                            //}
                        }

                        App.NotesDB.CreateNoteFlagsFromNote(note);
                    }

                    LoadAllNoteFlags();
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

            SetListOfFlagsStatus(note);
        }

        private void toolbarItem1_Clicked(object sender, EventArgs e)
        {
            addToolsView = !addToolsView;

            SetElementsVisability();
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

        private async void toDelete_Invoked(object sender, EventArgs e)
        {
            Guid id = (Guid)((MenuItem)sender).CommandParameter;

            if (id != Guid.Empty)
            {
                await App.NotesDB.DeleteNoteFlagAsync(id);

                LoadAllNoteFlags();
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }

        private async void deleteBtn_Clicked(object sender, EventArgs e)
        {
            Note note = (Note)BindingContext;

            if (note != null)
            {
                await App.NotesDB.DeleteNoteFlagsAsync(note.ID);
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
                await App.NotesDB.SaveAsync(noteFlag);
                SetListOfFlagsStatus();
            }

            //SaveNote(false);
        }

        private void CurItem_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.NewTextValue) && e.NewTextValue.EndsWith("\n"))
            {
                (sender as Editor).Text = e.NewTextValue.Trim();
                //Replace("\n", "");
                (sender as Editor).Unfocus();

                //if (curNoteFlag != null)
                //{
                //    curNoteFlag.Text = (sender as Editor).Text;
                //    await App.NotesDB.SaveNoteFlagAsync(curNoteFlag);
                //    curNoteFlag = null;
                //}
                ////SaveNote(false);
                //collectionView1.ScrollTo(((List<NoteFlags>)collectionView1.ItemsSource).Count - 1);
            }
        }

        private void noteDate_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }

        private void noteTime_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

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
            //formIsCreated = true;
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

            //if (note.ID == Guid.Empty && formIsCreated)
            //{
            //    SaveNote(false);
            //}

            if (note != null)
            {
                NoteFlags flag = new NoteFlags
                {
                    NoteID = note.ID,
                    Text = "",
                    Finished = false
                };
                await App.NotesDB.SaveAsync(flag);
            }

            LoadAllNoteFlags();
        }

        private async void CurItem_Unfocused(object sender, FocusEventArgs e)
        {
            //if (curNoteFlag != null)
            //{
            //    curNoteFlag.Text = (sender as Editor).Text;
            //    await App.NotesDB.SaveNoteFlagAsync(curNoteFlag);
            //    curNoteFlag = null;
            //}
            foreach (NoteFlags item in collectionView1.ItemsSource)
            {
                await App.NotesDB.SaveAsync(item);
            }

        }

        private async void BtnSelectCategory_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(NoteCategoriesView), true);
        }

        private void NoteCategory_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}