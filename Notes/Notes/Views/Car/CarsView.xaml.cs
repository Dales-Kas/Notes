﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Models.Car;

namespace Notes.Views.Car
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CarsView : ContentPage
    {
        public List<Cars> Items { get; set; }

        public CarsView()
        {
            InitializeComponent();

            Items = App.NotesDB.GetCarsAsync().Result;
            //Items = new ObservableCollection<string>
            //{
            //    "Item 1",
            //    "Item 2",
            //    "Item 3",
            //    "Item 4",
            //    "Item 5"
            //};

            MyListView.ItemsSource = Items;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private void MyListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (e.CurrentSelection != null)
            //{
            //    Cars car = (Cars)e.CurrentSelection.FirstOrDefault();
            //    string notePath = $"{nameof(CarForm)}?{nameof(CarForm.ItemId)}={car.ID}";
            //    await Shell.Current.GoToAsync(notePath);
            //}
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            TappedEventArgs e1 = (TappedEventArgs)e;

            if (e1.Parameter != null)
            {
                Cars car = (Cars)e1.Parameter;
                string notePath = $"{nameof(CarForm)}?{nameof(CarForm.ItemId)}={car.ID}";
                await Shell.Current.GoToAsync(notePath);
            }
        }
    }
}
