using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Models.Budget;

namespace Notes.Views.Budget
{
    [QueryProperty(nameof(OperationId), nameof(OperationId))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CashFlowOperationForm : ContentPage
    {
        public string OperationId
        {
            set
            {
                Guid.TryParse(value, out Guid Id);
                CashFlowOperations operation = App.NotesDB.GetFrom<CashFlowOperations>(Id,name:"ID");
                BindingContext = operation;                
            }
        }

        public CashFlowOperationForm()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await App.NotesDB.SaveAsync(BindingContext as CashFlowOperations);

            await Shell.Current.GoToAsync("..");
        }
    }
}