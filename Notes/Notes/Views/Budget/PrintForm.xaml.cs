using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Notes.Models.Budget;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace Notes.Views.Budget
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(ListInJson), nameof(ListInJson))]
    [QueryProperty(nameof(ForeignCurrency), nameof(ForeignCurrency))]
    [QueryProperty(nameof(OperationsDate), nameof(OperationsDate))]
    
    public partial class PrintForm : ContentPage
    {
        private string listInJson { set; get; }
        public string ListInJson 
        {
            set 
            {
                listInJson = value; 
            }
            get 
            {
                return listInJson;
            }
        }
                
        public bool ForeignCurrency { get; set; }

        public string OperationsDate { get; set; }

        string operationsTypes = "";

        bool operationsTypesFull = true;

        //private List<CashFlowOperations> Operations {get; set;}

        public PrintForm()
        {
            InitializeComponent();                      
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            LoadData();
        }

        private async Task LoadData(string groupField = "Description")
        {
            if (listInJson != null)
            {
                try
                {
                    var settings = new JsonSerializerSettings
                    {
                        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                    };
                    var errors = new List<string>();

                    List<Guid> operations = JsonConvert.DeserializeObject<List<Guid>>(ListInJson, new JsonSerializerSettings
                    {
                        Error = delegate (object sender, ErrorEventArgs args)
                        {
                            errors.Add(args.ErrorContext.Error.Message);
                            args.ErrorContext.Handled = true;
                        }
                    });
                    var ids = App.NotesDB.GuidsToString(operations);

                    var res = await App.NotesDB.SelectAllAsyncFrom<CashFlowOperations, string>(ids, "ID", true);

                    MyListView.ItemsSource = await App.NotesDB.GetAllCashOperationsToPrint(operations, groupField);
                    
                    var types = res.GroupBy(x => x.DetailedTypeID);

                    operationsTypes = "";

                    foreach (var item in types)
                    {
                        var typeRef = await App.NotesDB.SelectAsyncFrom<CashFlowDetailedType, Guid>(item.Key);

                        if (typeRef != null)
                            operationsTypes += typeRef.Name;
                        else
                            operationsTypes += item.Key.ToString();

                        operationsTypes += "; ";
                    }

                    if (operationsTypes.Length > 203)
                    {
                        operationsTypesFull = false;
                    }

                    SetOperationText();

                    OperationsPeriod.Text = OperationsDate;

                    double inAmount = res
                        .Where(x => x.OperationType == Models.OperationType.InOperation)
                        .Sum(x => ForeignCurrency ? x.AmountСurrency : x.Amount);
                    double outAmount = res
                        .Where(x => x.OperationType == Models.OperationType.OutOperation)
                        .Sum(x => ForeignCurrency ? x.AmountСurrency : x.Amount);

                    var sum = inAmount > 0 ? inAmount - outAmount : outAmount;

                    TotalAmount.Text = sum.ToString("#,0.00");
                }
                catch (Exception e)
                {

                    throw;
                }
            }
        }

        private static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            operationsTypesFull = !operationsTypesFull;
            SetOperationText();
        }

        private void SetOperationText()
        {
            if (!operationsTypesFull && operationsTypes.Length > 203)
                OperationsTypes.Text = operationsTypes.Substring(0, 200) + "...";
            else
                OperationsTypes.Text = operationsTypes;
        }

        private void BtnClient_Clicked(object sender, EventArgs e)
        {
            LoadData("Client");
        }

        private void BtnDescr_Clicked(object sender, EventArgs e)
        {
            LoadData("Description");
        }

        private void BtnDetail_Clicked(object sender, EventArgs e)
        {
            LoadData("DetailedTypeID");
        }

        private void BtnInOut_Clicked(object sender, EventArgs e)
        {
            LoadData("");
        }
    }
}
