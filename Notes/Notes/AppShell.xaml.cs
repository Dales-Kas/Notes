using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Views;
using Notes.Views.Car;

namespace Notes
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(NotePage), typeof(NotePage));
            Routing.RegisterRoute(nameof(NoteAddingPage),typeof(NoteAddingPage));
            Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
            Routing.RegisterRoute(nameof(NoteCategoriesView), typeof(NoteCategoriesView));
            Routing.RegisterRoute(nameof(NoteCategoryForm), typeof(NoteCategoryForm));
            Routing.RegisterRoute(nameof(CarsView), typeof(CarsView));
            Routing.RegisterRoute(nameof(CarForm), typeof(CarForm));

        }
        private void btnAbout_Clicked(object sender, EventArgs e)
        {
           FlyoutIsPresented = false;
        }

        private void btnNotes_Clicked(object sender, EventArgs e)
        {
           FlyoutIsPresented = false;
        }

        protected override bool OnBackButtonPressed()
        {           
            return base.OnBackButtonPressed();
        }

        private void Shell_Navigated(object sender, ShellNavigatedEventArgs e)
        {
            //NotePage.UpdateNotesList();
        }

    }
}
