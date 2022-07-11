using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Notes.Models.Budget;

namespace Notes.Models
{
    internal class ViewModel
    {
        public List<LandAreaItem> LandAreas { get; set; }

        public List<CashFlowOperations> Operations
        {
            get
            {
                return LoadData().OrderByDescending(x => x.Amount).ToList();
            }
        }

        public List<CashFlowOperations> SeriesData { get; set; }

        public ViewModel()
        {
            //DateTime periodStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //DateTime periodEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

            //LandAreas = App.NotesDB.GetAllCashOperations(periodStart, periodEnd, null, null, DateTime.MinValue, false, true).Result;

            //LoadData();
            //LandAreas = App.NotesDB.SelectAllFrom<CashFlowOperations,Guid>();
            LandAreas = new List<LandAreaItem>() {
            new LandAreaItem("Ukraine", 17.098),
            new LandAreaItem("Canada", 9.985),
            new LandAreaItem("People's Republic of China", 9.597),
            new LandAreaItem("United States of America", 9.834),
            new LandAreaItem("Brazil", 8.516),
            new LandAreaItem("Australia", 7.692),
            new LandAreaItem("India", 3.287),
            new LandAreaItem("Others", 81.2)
            };

            //Operations = LoadData().OrderByDescending(x=>x.Amount).ToList();

            //SeriesData = LoadData("OperationType");

            //LoadPage().Wait();
        }

        public async Task LoadPage()
        {
            //Operations = await LoadDataAsync();
        }

        //private async Task LoadData()
        //{
        //    DateTime periodStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        //    DateTime periodEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

        //   // LandAreas = await App.NotesDB.GetAllCashOperations(periodStart, periodEnd, null, null, DateTime.MinValue, false, true);
        //}

        public List<CashFlowOperations> LoadData(string fieldName = "DetailedTypeID")
        {
            DateTime periodStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime periodEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

            var oper =  App.NotesDB.GetAllCashOperationsSync(periodStart, periodEnd, null, null, DateTime.MinValue, false, true);

            List<Guid> ids = oper.Select(x => x.ID).ToList();

            var resault = App.NotesDB.GetAllCashOperationsToPrintSync(ids, fieldName);

            return resault;
            //BindingContext = this;
        }

        public async Task<List<CashFlowOperations>> LoadDataAsync()
        {
            DateTime periodStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime periodEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

            var oper = await App.NotesDB.GetAllCashOperations(periodStart, periodEnd, null, null, DateTime.MinValue, false, true);

            List<Guid> ids = oper.Select(x => x.ID).ToList();

            return await App.NotesDB.GetAllCashOperationsToPrint(ids, "DetailedTypeID");

            //BindingContext = this;
        }

    }
    class LandAreaItem
    {
        public string CountryName { get; }
        public double Area { get; }

        public LandAreaItem(string countryName, double area)
        {
            this.CountryName = countryName;
            this.Area = area;
        }
    }
}
