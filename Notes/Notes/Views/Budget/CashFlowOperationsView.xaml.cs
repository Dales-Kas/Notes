using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Models.Budget;
using System.Globalization;

namespace Notes.Views.Budget
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CashFlowOperationsView : TabbedPage
    {
        public List<CashFlowOperations> Items { get; set; }

        public CashFlowDetailedType detailedTypeFilter = null;

        public string DetailedTypeFilterName
        {
            get
            {
                if (detailedTypeFilter != null)
                {
                    return detailedTypeFilter.Name;
                }
                return "";
            }
        }

        public Clients clientFilter = null;

        public string ClientFilterName
        {
            get
            {
                if (clientFilter != null)
                {
                    return clientFilter.Name;
                }
                return "";
            }
            set { }
        }

        public int LastItem1 = 0;

        public DateTime dateFilter = DateTime.MinValue;

        public DateTime periodStart;
        public DateTime periodEnd;

        public bool inOperationFilter = false;
        public bool outOperationFilter = false;

        public bool foreignCurrency { get; set; }

        public double inAmount0 = 0;
        public double outAmount0 = 0;
        public double inAmount1 = 0;
        public double outAmount1 = 0;
        public double inAmount2 = 0;
        public double outAmount2 = 0;

        public List<PeriodsOfOperations> PeriodsOfOperations { get; set; } = new List<PeriodsOfOperations>();
        public List<PeriodsOfOperations> YearsOfOperations { get; set; } = new List<PeriodsOfOperations>();

        public PeriodsOfOperations currentPeriodOfUserChoice;

        public string[] periodViewChoise = new string[3];

        public int typeOfPeriod = 0;

        public bool IsEditable { get; set; }

        Dictionary<string, Button> buttonsVisible = new Dictionary<string, Button>();
        
        public CashFlowOperationsView()
        {
            InitializeComponent();

            periodStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            periodEnd = new DateTime(periodStart.Year, periodStart.Month, DateTime.DaysInMonth(periodStart.Year, periodStart.Month), 23, 59, 59);

            LoadAllData(true);

            IsBusy = false;
            refreshMyListView0.IsRefreshing = false;
            SetFilterViewOnForm(true);

            IsEditable = false;

            BindingContext = this;

            buttonsVisible.Add("AddItemButton1", AddItemButton1);
            buttonsVisible.Add("List1Up", List1Up);
            buttonsVisible.Add("List1Down", List1Down);

        }

        #region GetDataFromBase

        public async void LoadAllData(bool getAllPeriods = false, bool setTypeOfPeriod = true)
        {
            int badgeFilterNum = 0;

            Items = await App.NotesDB.GetCashFlowOperationsAsync();

            //Get all mounth from Items:
            if (getAllPeriods)
            //if (PeriodsOfOperations.Count==0 || YearsOfOperations.Count==0)
            {
                GetAndSetAllPeriods(setTypeOfPeriod);
            }

            var filteredItems = Items.Where(x => x.Date >= periodStart && x.Date <= periodEnd);

            if (detailedTypeFilter != null)
            {
                filteredItems = filteredItems.Where(x => x.DetailedTypeID == detailedTypeFilter.ID);
                badgeFilterNum++;
            }

            if (clientFilter != null)
            {
                filteredItems = filteredItems.Where(x => x.Client == clientFilter.ID);
                badgeFilterNum++;
            }

            if (dateFilter > DateTime.MinValue)
            {
                DateTime dateFilterMin = new DateTime(dateFilter.Date.Year, dateFilter.Date.Month, dateFilter.Date.Day);
                DateTime dateFilterMax = new DateTime(dateFilter.Date.Year, dateFilter.Date.Month, dateFilter.Date.Day, 23, 59, 59);

                filteredItems = filteredItems.Where(x => x.Date >= dateFilterMin && x.Date <= dateFilterMax);

                badgeFilterNum++;
            }

            if (inOperationFilter)
            {
                filteredItems = filteredItems.Where(x => x.OperationType == Models.OperationType.InOperation);

                badgeFilterNum++;
            }

            if (outOperationFilter)
            {
                filteredItems = filteredItems.Where(x => x.OperationType == Models.OperationType.OutOperation);

                badgeFilterNum++;
            }

            //GetInOutValues(ref inAmount0, ref outAmount0, 0);
            //GetInOutValues(ref inAmount1, ref outAmount1, 1);
            //GetInOutValues(ref inAmount2, ref outAmount2, 2);

            //Final values on top of Lists:

            inAmount0 = 0;
            outAmount0 = 0;
            inAmount1 = 0;
            outAmount1 = 0;
            inAmount2 = 0;
            outAmount2 = 0;

            filteredItems.AsParallel().ForAll(item =>
            {
                double curValue = foreignCurrency ? item.AmountСurrency : item.Amount;

                if (item.DontUseInCashFlow) { }
                else if (item.TypeID == 0)
                {
                    if (item.OperationType == Models.OperationType.InOperation)
                    {
                        inAmount0 += curValue;
                    }
                    else if (item.OperationType == Models.OperationType.OutOperation)
                    {
                        outAmount0 += curValue;
                    }
                }
                else if (item.TypeID == 1)
                {
                    if (item.OperationType == Models.OperationType.InOperation)
                    {
                        inAmount1 += curValue;
                    }
                    else if (item.OperationType == Models.OperationType.OutOperation)
                    {
                        outAmount1 += curValue;
                    }
                }
                else if (item.TypeID == 2)
                {
                    if (item.OperationType == Models.OperationType.InOperation)
                    {
                        inAmount1 += curValue;
                    }
                    else if (item.OperationType == Models.OperationType.OutOperation)
                    {
                        outAmount1 += curValue;
                    }
                }
            });

            MyListView0.ItemsSource = filteredItems.Where(x => x.TypeID == 0).OrderByDescending(x => x.Date).ToList();
            MyListView1.ItemsSource = filteredItems.Where(x => x.TypeID == 1).OrderByDescending(x => x.Date).ToList();
            MyListView2.ItemsSource = filteredItems.Where(x => x.TypeID == 2).OrderByDescending(x => x.Date).ToList();

            //Set current items:

            MyListView1.ScrollTo(LastItem1, animate: false);

            string amountFormat = "#,#.00";
            string amountFormat0 = "-";

            CardIn0.Text = inAmount0 != 0 ? inAmount0.ToString(amountFormat) : amountFormat0;
            CardOut0.Text = outAmount0 != 0 ? outAmount0.ToString(amountFormat) : amountFormat0;
            CardIn1.Text = inAmount1 != 0 ? inAmount1.ToString(amountFormat) : amountFormat0;
            CardOut1.Text = outAmount1 != 0 ? outAmount1.ToString(amountFormat) : amountFormat0;
            CardIn2.Text = inAmount2 != 0 ? inAmount2.ToString(amountFormat) : amountFormat0;
            CardOut2.Text = outAmount2 != 0 ? outAmount2.ToString(amountFormat) : amountFormat0;

            SetTitleText(currentPeriodOfUserChoice);

            // badgeFilter.Text = badgeFilterNum.ToString();
        }

        public void GetAndSetAllPeriods(bool setTypeOfPeriod = true)
        {
            //Get all periods:
            
            if (setTypeOfPeriod)
            {
                PeriodsOfOperations.Clear();

                PeriodsOfOperations = Items
                        .GroupBy(p => new DateTime(p.Date.Year, p.Date.Month, 1))
                        //.Select(g => new { Date = g.Key }).ToList();
                        .Select(period => new PeriodsOfOperations()
                        {
                            PeriodName = period.Key.ToString("MMMM yyyy").ToUpper(),
                            PeriodStart = period.Key,
                            PeriodEnd = new DateTime(period.Key.Year, period.Key.Month, DateTime.DaysInMonth(period.Key.Year, period.Key.Month))
                        }
                        ).OrderBy(x => x.PeriodStart).ToList();

                YearsOfOperations.Clear();

                YearsOfOperations = Items
                        .GroupBy(p => new DateTime(p.Date.Year, 1, 1))
                        //.Select(g => new { Date = g.Key }).ToList();
                        .Select(period => new PeriodsOfOperations()
                        {
                            PeriodName = period.Key.ToString("yyyy").ToUpper(),
                            PeriodStart = new DateTime(period.Key.Year, 1, 1),
                            PeriodEnd = new DateTime(period.Key.Year, 12, 31)
                        }
                        ).OrderBy(x => x.PeriodStart).ToList();

                allPeriods.ItemsSource = PeriodsOfOperations;
                allYears.ItemsSource = YearsOfOperations;
            }

            if (typeOfPeriod == 0)
            {
                allPeriods.IsVisible = true;
                allYears.IsVisible = false;
                SetCurrentPeriodView(PeriodsOfOperations, 0);
            }

            else if (typeOfPeriod == 1)
            {
                allPeriods.IsVisible = false;
                allYears.IsVisible = true;
                SetCurrentPeriodView(YearsOfOperations, 1);
            }
            else
            {
                allPeriods.IsVisible = false;
                allYears.IsVisible = false;
                periodStart = DateTime.MinValue;
                periodEnd = DateTime.MaxValue;
            }

            //Type of period:

            if (setTypeOfPeriod)
            {
                periodViewChoise[0] = "Дані за місяць";
                periodViewChoise[1] = "Дані за рік";
                periodViewChoise[2] = "Усі дані";
                
                periodView.Text = periodViewChoise[0];
            }
        }

        private void GetInOutValues(ref double inValue, ref double outValue, int typeID)
        {
            inValue = 0;
            outValue = 0;

            double inValue1 = 0;
            double outValue1 = 0;

            Items.AsParallel().ForAll(item =>
            {

                if (item.DontUseInCashFlow || item.TypeID != typeID) { }

                else if (item.OperationType == Models.OperationType.InOperation)
                {
                    inValue1 += foreignCurrency ? item.AmountСurrency : item.Amount;
                }

                else if (item.OperationType == Models.OperationType.OutOperation)
                {
                    outValue1 += foreignCurrency ? item.AmountСurrency : item.Amount;
                }
            });

            inValue = inValue1;
            outValue = outValue1;

            //foreach (var item in Items)
            //{
            //    if (item.DontUseInCashFlow || item.TypeID != typeID)
            //    {
            //        continue;
            //    }

            //    if (item.OperationType == Models.OperationType.InOperation)
            //    {
            //        inValue += foreignCurrency ? item.AmountСurrency : item.Amount;
            //    }

            //    else if (item.OperationType == Models.OperationType.OutOperation)
            //    {
            //        outValue += foreignCurrency ? item.AmountСurrency : item.Amount;
            //    }
            //}
        }

        #endregion
  
        #region FormItemsVisability

        public void SetPeriodOfView(PeriodsOfOperations period)
        {
            if (period != null)
            {
                SetPeriodValues(period);

                SetTitleText(period);

                LoadAllData();

                SetFilterViewOnForm(!IsFilterFilled());
            }

            else
            {
                this.Title = "Фінанси";
            }
        }

        public void SetTitleText(PeriodsOfOperations period)
        {
            string val = !foreignCurrency ? "грн" : "$";

            if (IsFilterFilled())
            {
                double inAll = inAmount0 + inAmount1;
                double outAll = outAmount0 + outAmount1;

                string valueFormat = "#,0.00";

                if (inAll > 0 && outAll > 0)
                {
                    this.Title = $"Прихід: {inAll.ToString(valueFormat)} Розхід: {outAll.ToString(valueFormat)} ({val})";
                }
                else if (inAll > 0)
                {
                    this.Title = $"Всього прихід: {inAll.ToString(valueFormat)} {val}";
                }
                else if (outAll > 0)
                {
                    this.Title = $"Всього розхід: {outAll.ToString(valueFormat)} {val}";
                }
            }

            else
            {
                if (period == null)
                {
                    this.Title = "Фінанси";
                }
                else if (typeOfPeriod != 2)
                {
                    this.Title = $"Фінанси ({period.PeriodName.ToLower()}) {val}";
                }
                else
                {
                    this.Title = "Фінанси (за весь період)";
                }
            }
        }

        public void SetPeriodValues(PeriodsOfOperations period)
        {
            periodStart = period.PeriodStart;
            periodEnd = new DateTime(period.PeriodEnd.Year, period.PeriodEnd.Month, DateTime.DaysInMonth(period.PeriodEnd.Year, period.PeriodEnd.Month), 23, 59, 59);
        }

        public void SetCurrentPeriodView(List<PeriodsOfOperations> curPeriodsOfOperations, int curtypeOfPeriod)
        {
            if (curPeriodsOfOperations.Count > 0)
            {
                currentPeriodOfUserChoice = curPeriodsOfOperations[^1];

                if (curtypeOfPeriod == 0)
                {
                    allPeriods.CurrentItem = currentPeriodOfUserChoice;
                }
                else if (curtypeOfPeriod == 1)
                {
                    allYears.CurrentItem = currentPeriodOfUserChoice;
                }

                SetPeriodValues(currentPeriodOfUserChoice);

            }
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            IsEditable = !IsEditable;

            if (IsEditable)
            {
                (sender as Button).ImageSource = "edit.png";
            }
            else
            {
                (sender as Button).ImageSource = "unedit.png";
            }

            LoadAllData();
        }

        private void MyListView0_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        async Task SetVisabilityOfAddButton(int firstIndex, string buttonName)
        {
            buttonsVisible.TryGetValue(buttonName, out Button button);

            if (firstIndex == 0 && !IsFilterFilled())
            {
                button.IsVisible = true;
                await button.FadeTo(0.97, 250);
            }
            else
            {
                await button.FadeTo(0, 250);
                //await Task.Delay(250);
                if (!button.IsVisible)
                    button.IsVisible = false;
            }
        }

        async Task SetVisabilityOfListPositionButtons(ItemsViewScrolledEventArgs e)
        {
            List<CashFlowOperations> list = (MyListView1.ItemsSource as List<CashFlowOperations>);

            if (e.FirstVisibleItemIndex == 0)
            {
                List1Up.IsVisible = false;
                List1Down.IsVisible = false;
            }
            else if (e.LastVisibleItemIndex == list.Count - 1)
            {
                List1Up.IsVisible = true;
                List1Down.IsVisible = false;
            }
            else
            {
                if (List1Up.IsVisible || List1Down.IsVisible)
                {
                }

                List1Up.IsVisible = e.FirstVisibleItemIndex >= 3;
                List1Down.IsVisible = true;
            }

            if (List1Up.IsVisible || List1Down.IsVisible)
            {
                await Task.Delay(4500);

                List1Up.IsVisible = false;
                List1Down.IsVisible = false;
            }


            //if (List1Up.IsVisible)
            //{ 
            //    await List1Up.FadeTo(0, 250);
            //}
            //else
            //{
            //    await List1Up.FadeTo(0.5, 0);
            //}

            //if (List1Down.IsVisible)
            //{
            //    await List1Down.FadeTo(0, 250);
            //}
            //else
            //{
            //    await List1Down.FadeTo(0.5, 0);
            //}

        }

        #endregion

        #region FiltersOfOperations
        private async void DetailedTypeTap_Tapped(object sender, EventArgs e)
        {
            if (detailedTypeFilter != null)
            {
                detailedTypeFilter = null;
            }

            else
            {
                Guid guid = (Guid)(e as TappedEventArgs).Parameter;
                detailedTypeFilter = await App.NotesDB.GetCashFlowDetailedTypeAsync(guid);
            }

            SetFilterViewOnForm(!IsFilterFilled());

            LoadAllData();
        }

        private async void ClientTap_Tapped(object sender, EventArgs e)
        {
            if (clientFilter != null)
            {
                clientFilter = null;
            }

            else
            {
                Guid guid = (Guid)(e as TappedEventArgs).Parameter;
                clientFilter = await App.NotesDB.GetClientAsync(guid);
            }

            SetFilterViewOnForm(!IsFilterFilled());

            LoadAllData();
        }

        private void DateTap_Tapped(object sender, EventArgs e)
        {
            if (dateFilter != DateTime.MinValue)
            {
                dateFilter = DateTime.MinValue;
            }

            else
            {
                dateFilter = (DateTime)(e as TappedEventArgs).Parameter;
            }

            SetFilterViewOnForm(!IsFilterFilled());

            LoadAllData();
        }
        private void InTap_Tapped(object sender, EventArgs e)
        {
            inOperationFilter = !inOperationFilter;
            outOperationFilter = false;

            SetFilterViewOnForm(!IsFilterFilled());

            LoadAllData();
        }

        private void OutTap_Tapped(object sender, EventArgs e)
        {
            outOperationFilter = !outOperationFilter;
            inOperationFilter = false;

            SetFilterViewOnForm(!IsFilterFilled());

            LoadAllData();
        }

        private void TapGestureRecognizer_ClientFilter(object sender, EventArgs e)
        {
            ClientTap_Tapped(sender, e);
        }

        private void TapGestureRecognizer_DetailedTypeFilter(object sender, EventArgs e)
        {
            DetailedTypeTap_Tapped(sender, e);
        }

        private void TapGestureRecognizer_DateFilter(object sender, EventArgs e)
        {
            DateTap_Tapped(sender, e);
        }

        private void TapGestureRecognizer_InOperationFilter(object sender, EventArgs e)
        {
            InTap_Tapped(sender, e);
        }

        private void TapGestureRecognizer_OutOperationFilter(object sender, EventArgs e)
        {
            OutTap_Tapped(sender, e);
        }

        private void SetFilterViewOnForm(bool resetFilter = false)
        {
            if (resetFilter)
            {
                detailedTypeFilter = null;
                clientFilter = null;
                dateFilter = DateTime.MinValue;
                inOperationFilter = false;
                outOperationFilter = false;

                //tbFilter.IsEnabled = false;

                CardIn0.Background = Color.White;
                CardOut0.Background = Color.White;

                //SetTitleText(null);
            }

            else
            {
                //tbFilter.IsEnabled = true;

                CardIn0.Background = inOperationFilter ? Color.FromHex("#E2DFCC") : Color.White;
                CardOut0.Background = outOperationFilter ? Color.FromHex("#FFE4E1") : Color.White;

                lblClientFilter.Text = ClientFilterName;
                lblClientFilter.IsVisible = !string.IsNullOrEmpty(lblClientFilter.Text);
                //lblClientFilterF.IsVisible = lblClientFilter.IsVisible;

                lblDetailedTypeFilter.Text = DetailedTypeFilterName;
                lblDetailedTypeFilter.IsVisible = !string.IsNullOrEmpty(lblDetailedTypeFilter.Text);

                lblDateFilter.Text = dateFilter.ToString("M");
                lblDateFilter.IsVisible = dateFilter != DateTime.MinValue;

                lblInOperationFilter.Text = inOperationFilter ? "Тільки вхідні операції" : "";
                lblInOperationFilter.IsVisible = inOperationFilter;

                lblOutOperationFilter.Text = outOperationFilter ? "Тільки вихідні операції" : "";
                lblOutOperationFilter.IsVisible = outOperationFilter;

                SetTitleText(null);
            }

            filterGroup.IsVisible = !resetFilter;
            //imgFilterCancel.IsVisible = !resetFilter;

            CardIn1.Background = CardIn0.Background;
            CardIn2.Background = CardIn0.Background;
            CardOut1.Background = CardOut0.Background;
            CardOut2.Background = CardOut0.Background;

        }

        private bool IsFilterFilled()
        {
            return !(detailedTypeFilter == null &&
            clientFilter == null &&
            dateFilter == DateTime.MinValue &&
            inOperationFilter == false &&
            outOperationFilter == false);
        }
        #endregion

        #region ItemsGestureRecognizer
        
        private void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {
            //App.NotesDB.GetFromMySQL();


            //SetFilterViewOnForm(true);

            //LoadAllData();
        }

        private void refreshMyListView0_Refreshing(object sender, EventArgs e)
        {
            LoadAllData();
            (sender as RefreshView).IsRefreshing = false;
        }

        public void allPeriods_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            PeriodsOfOperations period = (PeriodsOfOperations)(sender as CarouselView).CurrentItem;

            SetPeriodOfView(period);
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Dictionary<string, PeriodsOfOperations> data = new Dictionary<string, PeriodsOfOperations>();

            string[] periodsString = new string[PeriodsOfOperations.Count];

            int i = 0;

            foreach (var item in PeriodsOfOperations.OrderByDescending(x => x.PeriodStart).ToList())
            //foreach (var item in PeriodsOfOperations)
            {
                periodsString[i] = item.PeriodName;
                data.Add(item.PeriodName, item);
                i++;
            }

            string resault = await DisplayActionSheet("Виберіть період", "Відміна", null, periodsString);

            if (resault != null)
            {
                if (data.TryGetValue(resault, out PeriodsOfOperations resultChoise))
                {
                    SetPeriodOfView(resultChoise);
                    allPeriods.CurrentItem = resultChoise;
                    //((Label)sender).Text = resultChoise.PeriodName;
                }
            }
        }

        private async void periodView_SelectedIndexChanged(object sender, EventArgs e)
        {
            string resault = await DisplayActionSheet("Період для відображення", "Відміна", null, periodViewChoise);

            if (resault != null && resault != "Відміна")
            {
                for (int i = 0; i < periodViewChoise.Length; i++)
                {
                    if (periodViewChoise[i] == resault)
                    {
                        if (i != typeOfPeriod)
                        {
                            typeOfPeriod = i;

                            ((Label)sender).Text = resault;

                            LoadAllData(true, false);

                            SetFilterViewOnForm(!IsFilterFilled());
                        }
                    }
                }
            }
        }

        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            foreignCurrency = !foreignCurrency;

            if (foreignCurrency)
            {
                (sender as Image).Source = "USD.png";
            }

            else
            {
                (sender as Image).Source = "UAH.png";
            }

            PeriodsOfOperations period;

            if (typeOfPeriod == 0)
            {
                period = allPeriods.CurrentItem as PeriodsOfOperations;
            }
            else if (typeOfPeriod == 1)
            {
                period = allYears.CurrentItem as PeriodsOfOperations;
            }

            else
            {
                period = null;
            }

            LoadAllData();

            SetTitleText(period);
        }

        private void lblClientFilter_BindingContextChanged(object sender, EventArgs e)
        {

        }

        private void lblClientFilter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        private void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
        {
            SetFilterViewOnForm(true);

            LoadAllData();
        }

        private async void TapGestureRecognizer_Tapped_3(object sender, EventArgs e)
        {
            CashFlowOperations operation = (CashFlowOperations)(e as TappedEventArgs).Parameter;

            if (operation != null)
            {
                string operationPath = $"{nameof(CashFlowOperationForm)}?{nameof(CashFlowOperationForm.OperationId)}={operation.ID}";
                await Shell.Current.GoToAsync(operationPath);

                MyListView0.SelectedItem = null;
                MyListView1.SelectedItem = null;
                MyListView2.SelectedItem = null;
            }
        }

        private void MyListView1_ScrollToRequested(object sender, ScrollToRequestEventArgs e)
        {

        }

        private async void MyListView1_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            //LastItem1 = e.FirstVisibleItemIndex;

            await SetVisabilityOfAddButton(e.FirstVisibleItemIndex, "AddItemButton1");

            await SetVisabilityOfListPositionButtons(e);
        }

        private void List1Up_Clicked(object sender, EventArgs e)
        {
            MyListView1.ScrollTo(0, animate: false);
        }

        private void List1Down_Clicked(object sender, EventArgs e)
        {
            List<CashFlowOperations> list = (MyListView1.ItemsSource as List<CashFlowOperations>);

            var count = list[list.Count - 1];
            MyListView1.ScrollTo(count, animate: false);
        }

        private async void AddItemButton_Clicked(object sender, EventArgs e)
        {
            string inName = "Надходження коштів";
            string outName = "Розхід коштів";

            string resault = await DisplayActionSheet("Тип операції", "Відміна", null, inName, outName);

            if (resault != null && resault != "Відміна")
            {
                string operationPath = $"{nameof(CashAddForm)}?{nameof(CashAddForm.IsOut)}={resault == outName}";
                await Shell.Current.GoToAsync(operationPath);
            }
        }

        #endregion

        private async void TapGestureRecognizer_Tapped_4(object sender, EventArgs e)
        {
            var statement = await Data.Services.MonobankAPI.GetStatement(DateTime.Now);
        }
    }

    #region OthersAddClasses
    public class PeriodsOfOperations
    {
        public string PeriodName { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }

    #endregion

}
