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
    [QueryProperty(nameof(CategoryId), nameof(CategoryId))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoteCategoryForm : ContentPage
    {
        public string CategoryId
        {
            set
            {
                int.TryParse(value,out int Id);
                NoteCategory category = App.NotesDB.SelectFrom<NoteCategory,int>(Id);
                BindingContext = category;
            }
        }
        public NoteCategoryForm()
        {
            InitializeComponent();
            BindingContext = new NoteCategory();
        }

        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            NoteCategory category = (NoteCategory)BindingContext;

            //if (category == null)
            //{
            //    category = new NoteCategory() { Name = CategoryName.Text };
            //}
            
            if (category!=null)
            {
                //await App.NotesDB.SaveNoteCategoryAsync(category);
                await App.NotesDB.SaveAsync(category);
                await Shell.Current.GoToAsync("..");
            }

        }
    }
}