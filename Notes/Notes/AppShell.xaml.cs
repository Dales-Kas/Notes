﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Views;

namespace Notes
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(NotePage), typeof(NotePage));
            Routing.RegisterRoute(nameof(NoteAddingPage),typeof(NoteAddingPage));
            
        }
        private async void btnAbout_Clicked(object sender, EventArgs e)
        {
            //Detail = new NavigationPage(new AboutPage())
            //{
            //    BackgroundColor = Color.White
            //};

            FlyoutIsPresented = false;
        }

        private async void btnNotes_Clicked(object sender, EventArgs e)
        {
            //Detail = new NavigationPage(new NotePage())
            //{
            //    BackgroundColor = Color.White
            //};
            FlyoutIsPresented = false;
        }
    }
}
