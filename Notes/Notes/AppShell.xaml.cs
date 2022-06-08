using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Views;
using Notes.Views.Car;
using Notes.Views.Budget;

namespace Notes
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(NotePage), typeof(NotePage));
            Routing.RegisterRoute(nameof(NoteAddingPage),typeof(NoteAddingPage));
            Routing.RegisterRoute(nameof(ProgramSettingsView), typeof(ProgramSettingsView));
            Routing.RegisterRoute(nameof(NoteCategoriesView), typeof(NoteCategoriesView));
            Routing.RegisterRoute(nameof(NoteCategoryForm), typeof(NoteCategoryForm));
            Routing.RegisterRoute(nameof(CarsView), typeof(CarsView));
            Routing.RegisterRoute(nameof(CarForm), typeof(CarForm));
            Routing.RegisterRoute(nameof(CurrenciesView), typeof(CurrenciesView));
            Routing.RegisterRoute(nameof(CashFlowDetailedTypeView), typeof(CashFlowDetailedTypeView));
            Routing.RegisterRoute(nameof(CashFlowOperationsView), typeof(CashFlowOperationsView));
            Routing.RegisterRoute(nameof(CashFlowOperationForm), typeof(CashFlowOperationForm));
            Routing.RegisterRoute(nameof(ExchangeRatesList), typeof(ExchangeRatesList));
            Routing.RegisterRoute(nameof(CashFlowDetailedTypeForm), typeof(CashFlowDetailedTypeForm));
            Routing.RegisterRoute(nameof(CashAddForm), typeof(CashAddForm));            
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
