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
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Charts : ContentPage
    {
        public List<CashFlowOperationsToShow> Operations
        {
            get
            {
                return LoadData();
            }
        }
        public Charts()
        {
            InitializeComponent();

            //LoadData();            

        }

        protected override void OnAppearing()
        {
            //LoadData();

            base.OnAppearing();
        }

        public List<CashFlowOperationsToShow> LoadData()
        {
            DateTime periodStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime periodEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

            return App.NotesDB.GetAllCashOperationsSync(periodStart, periodEnd, null, null, DateTime.MinValue, false, true);

            //BindingContext = this;
        }

    }
}