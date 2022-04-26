using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Newtonsoft.Json;
using Notes.Models;

namespace Notes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            //// SwipeItems
            //SwipeItem favoriteSwipeItem = new SwipeItem
            //{
            //    Text = "Favorite",
            //    IconImageSource = "pic2.png",
            //    BackgroundColor = Color.LightGreen
            //};
            //favoriteSwipeItem.Invoked += OnFavoriteSwipeItemInvoked;

            //SwipeItem deleteSwipeItem = new SwipeItem
            //{
            //    Text = "Delete",
            //    IconImageSource = "pic1.png",
            //    BackgroundColor = Color.LightPink
            //};
            //deleteSwipeItem.Invoked += OnDeleteSwipeItemInvoked;

            //List<SwipeItem> swipeItems = new List<SwipeItem>() { favoriteSwipeItem, deleteSwipeItem };

            //// SwipeView content
            //Grid grid = new Grid
            //{
            //    HeightRequest = 60,
            //    WidthRequest = 300,
            //    BackgroundColor = Color.LightGray
            //};
            //grid.Children.Add(new Label
            //{
            //    Text = "Swipe right",
            //    HorizontalOptions = LayoutOptions.Center,
            //    VerticalOptions = LayoutOptions.Center
            //});

            //SwipeView swipeView = new SwipeView
            //{
            //    LeftItems = new SwipeItems(swipeItems),
            //    Content = grid
            //};

            //Content = grid;

            //base.OnAppearing();
        }

        private void OnDeleteSwipeItemInvoked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnFavoriteSwipeItemInvoked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SwipeItem_Invoked(object sender, EventArgs e)
        {

        }

        private void SwipeItem_Invoked_1(object sender, EventArgs e)
        {

        }

        private void ImportJSON_Clicked(object sender, EventArgs e)
        {
            var res = PickAndShow();
        }

        private async Task<FileResult> PickAndShow()
        {
            string resault1 = await DisplayActionSheet("Виберіть тип завантаження", "Відміна", null, "Notes", "NotesFlags");

            if (resault1 != null && resault1 != "Відміна")
            {

            }
            else
            {
                return null;
            }

            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                        {
                            { DevicePlatform.Android, new[] { "application/json" } 
                        }
                    });
            var options = new PickOptions
            {
                PickerTitle = "Please select a json file",
                FileTypes = customFileType,
            };

            try
            {
                var result = await FilePicker.PickAsync(options);
                if (result != null)
                {
                    string Text = $"File Name: {result.FileName}";
                    var stream = await result.OpenReadAsync();                    
                    var reader = new System.IO.StreamReader(stream);

                    var jsonString = reader.ReadToEnd();

                    switch (resault1)
                    {
                        case "Notes":
                            {
                                List<Note> jsonData = JsonConvert.DeserializeObject<List<Note>>(jsonString);

                                foreach (Note item in jsonData)
                                {
                                    item.Date = DateTime.Now;
                                    await App.NotesDB.SaveNoteAsync(item, true,true, false);
                                }
                                break;
                            }
                        case "NotesFlags":
                            {
                                List<NoteFlags> jsonData = JsonConvert.DeserializeObject<List<NoteFlags>>(jsonString);

                                foreach (NoteFlags item in jsonData)
                                {
                                    await App.NotesDB.SaveNoteFlagAsync(item, true);
                                }
                                break;
                            }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                await DisplayAlert("УВАГА!", ex.Message, "ОК");                
            }

            return null;
        }
    }
}