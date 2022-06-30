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
    [QueryProperty(nameof(IsOut), nameof(IsOut))]
    public partial class CashAddForm : ContentPage
    {
        public bool IsOut
    {
            set
            {
                LoadData(value);
            }
        }
        public CashAddForm()
        {
            InitializeComponent();                      
        }

        public async void LoadData(bool isOut)
        {
            Title = isOut ? "Розхід коштів" : "Надходження коштів";
            btnAdd.BackgroundColor = isOut ? Color.Red : Color.Black;

            var Items = await App.NotesDB.SelectAllAsyncFrom<CashFlowOperations,Guid>();

            if (isOut)
            {
                Items = Items.Where(x=>x.OperationType==Models.OperationType.OutOperation && x.TypeID == 1 && x.Client!=Guid.Empty).ToList();
            }
            else 
            {
                Items = Items.Where(x => x.OperationType == Models.OperationType.InOperation && x.TypeID == 1 && x.Client != Guid.Empty).ToList();
            }
            var ItemsTop = Items.Select(x => new TopClient()
                {
                    ClientID = x.Client,
                    Amount = x.Amount
                })
                .GroupBy(x => x.ClientID).Select(x => new TopClient()
                {
                    ClientID = x.First().ClientID,
                    Amount = x.Sum(c => c.Amount)
                }).OrderByDescending(x => x.Amount).Take(10).ToList();

            //TOP 10 CLients for quick access:

            Label[] buttons = new Label[10];
            buttons[0] = (Label)Client0;
            buttons[1] = (Label)Client1;
            buttons[2] = (Label)Client2;
            buttons[3] = (Label)Client3;
            buttons[4] = (Label)Client4;
            buttons[5] = (Label)Client5;
            buttons[6] = (Label)Client6;
            buttons[7] = (Label)Client7;
            buttons[8] = (Label)Client8;
            buttons[9] = (Label)Client9;
            
            int i = 0;
            foreach (var item in ItemsTop)
            {
                item.Client = await App.NotesDB.SelectAsyncFrom<Clients,Guid>(item.ClientID);
                buttons[i].Text = item.Client!=null ? item.Client.Name : item.ClientID.ToString();
                i++;
            }

        }

        public class TopClient 
        {
            public Guid ClientID { get; set; }
            public Clients Client { get; set; }
            public double Amount { get; set; }
        }
    }
}