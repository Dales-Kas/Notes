using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Views.Budget
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public partial class CashFlowDetailedTypeForm : ContentPage
    {        
        public string ItemId
        {
            set
            {
                LoadRef(value);
            }
        }
        public CashFlowDetailedTypeForm()
        {
            InitializeComponent();
        }

        public void LoadRef(string id)
        {
            Guid.TryParse(id, out Guid guid);

            if (guid!=Guid.Empty)
            {

            }
        }
    }
}