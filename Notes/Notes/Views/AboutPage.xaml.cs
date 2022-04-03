using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
    }
}