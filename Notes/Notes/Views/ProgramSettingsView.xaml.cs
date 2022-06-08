using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Newtonsoft.Json;
using Notes.Models;
using Notes.Models.Car;
using Notes.Models.Budget;

namespace Notes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProgramSettingsView : ContentPage
    {
        public ProgramSettings CurSettings { get; set; }

        public ProgramSettingsView()
        {
            InitializeComponent();

            OnFormOpen();
        }

        public async Task OnFormOpen()
        {
            CurSettings = await App.NotesDB.GetProgramSettingsAsync();

            MonoToken.Text = CurSettings.MonoToken;
        }

        private void ImportJSON_Clicked(object sender, EventArgs e)
        {
            PickAndShow();
        }

        private async void PickAndShow()
        {
            //string resault1 = await DisplayActionSheet("Виберіть тип завантаження", "Відміна", null, "NotesAndNotesFlags", "Cars","Budget", "ALL");
            string resault1 = await DisplayActionSheet("Виберіть тип завантаження", "Відміна", null, "ALL");

            if (resault1 != null && resault1 != "Відміна")
            {

            }
            else
            {
                return;
            }

            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                        {
                            { DevicePlatform.Android, new[] { "application/json" }
                        }
                    });
            var options = new PickOptions
            {
                PickerTitle = "Please select a json file",
                FileTypes = customFileType,
            };

            try
            {
                var result = await FilePicker.PickAsync(options);
                if (result != null)
                {
                    DateTime timeStart = DateTime.Now;

                    loadingText.IsVisible = true;
                    lblLoadingText.Text = "";

                    string Text = $"File Name: {result.FileName}";
                    var stream = await result.OpenReadAsync();
                    var reader = new System.IO.StreamReader(stream);

                    var jsonString = reader.ReadToEnd();

                    switch (resault1)
                    {
                        case "ALL":
                            {
                                await ImportAndSaveToBase(jsonString);

                                break;
                            }
                    }

                    loadingText.IsVisible = false;

                    TimeSpan timeEnd = DateTime.Now - timeStart;

                    await DisplayAlert("Дані завантажено", $"Успішно, час виконання {timeEnd.Minutes} хв. {timeEnd.Seconds} сек.", "ОК");

                    return;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("УВАГА!", ex.Message, "ОК");
            }

            return;
        }

        public async Task ImportAndSaveToBase(string jsonString)
        {
            List<ImportData> jsonData = JsonConvert.DeserializeObject<List<ImportData>>(jsonString);

            int iter = 0;

            foreach (ImportData item in jsonData)
            {
                App.NotesDB.DropTable(item.Name);

                if (item.Name == "Note")
                {
                    SetTextOfLoading(ref iter, 0, item.Name, item.Note.Count);

                    foreach (Note note in item.Note)
                    {
                        SetTextOfLoading(ref iter);

                        note.Date = DateTime.Now;
                        await App.NotesDB.SaveNoteAsync(note, true, true, false);
                    }
                }
                else if (item.Name == "NoteFlags")
                {
                    SetTextOfLoading(ref iter, 0, item.Name, item.NoteFlags.Count);

                    foreach (NoteFlags noteFlag in item.NoteFlags)
                    {
                        SetTextOfLoading(ref iter);

                        try
                        {
                            await App.NotesDB.SaveNoteFlagAsync(noteFlag);
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("УВАГА!", ex.Message, "ОК");
                        }
                    }
                }
                else if (item.Name == "Cars")
                {
                    SetTextOfLoading(ref iter, 0, item.Name, item.Cars.Count);

                    foreach (Cars car in item.Cars)
                    {
                        SetTextOfLoading(ref iter);

                        await App.NotesDB.SaveCarAsync(car, true);
                    }
                }
                else
                {
                    //dynamic DataTable = null;

                    if (item.Name == "CarDescription")
                    {
                        SetTextOfLoading(ref iter, 0, item.Name, item.CarDescription.Count);

                        //DataTable = item.CarDescription;
                        foreach (CarDescription i in item.CarDescription)
                        {
                            SetTextOfLoading(ref iter);

                            await App.NotesDB.SaveAsync((object)i, item.Name);
                        }
                    }
                    else if (item.Name == "CarNotes")
                    {
                        SetTextOfLoading(ref iter, 0, item.Name, item.CarNotes.Count);

                        //DataTable = item.CarDescription;
                        foreach (CarNotes i in item.CarNotes)
                        {
                            SetTextOfLoading(ref iter);

                            await App.NotesDB.SaveAsync((object)i, item.Name);
                        }
                    }
                    else if (item.Name == "Currencies")
                    {
                        SetTextOfLoading(ref iter, 0, item.Name, item.Currencies.Count);

                        //DataTable = item.Currencies;
                        foreach (var i in item.Currencies)
                        {
                            SetTextOfLoading(ref iter);

                            await App.NotesDB.SaveAsync((object)i, item.Name);
                        }
                    }

                    else if (item.Name == "ExchangeRates")
                    {
                        SetTextOfLoading(ref iter, 0, item.Name, item.ExchangeRates.Count);

                        foreach (var i in item.ExchangeRates)
                        {
                            SetTextOfLoading(ref iter);

                            await App.NotesDB.SaveAsync((object)i, item.Name);
                        }
                    }

                    else if (item.Name == "CashFlowDetailedType")
                    {
                        SetTextOfLoading(ref iter, 0, item.Name, item.CashFlowDetailedType.Count);

                        foreach (var i in item.CashFlowDetailedType)
                        {
                            SetTextOfLoading(ref iter);

                            await App.NotesDB.SaveAsync((object)i, item.Name);
                        }
                    }

                    else if (item.Name == "CashFlowOperations")
                    {
                        SetTextOfLoading(ref iter, 0, item.Name, item.CashFlowOperations.Count);

                        foreach (var i in item.CashFlowOperations)
                        {
                            SetTextOfLoading(ref iter);

                            await App.NotesDB.SaveAsync((object)i, item.Name);
                        }
                    }

                    else if (item.Name == "Clients")
                    {
                        SetTextOfLoading(ref iter, 0, item.Name, item.Clients.Count);

                        foreach (var i in item.Clients)
                        {
                            SetTextOfLoading(ref iter);

                            await App.NotesDB.SaveAsync((object)i, item.Name);
                        }
                    }

                    else if (item.Name == "MoneyStorages")
                    {
                        SetTextOfLoading(ref iter, 0, item.Name, item.MoneyStorages.Count);

                        foreach (var i in item.MoneyStorages)
                        {
                            SetTextOfLoading(ref iter);

                            await App.NotesDB.SaveAsync((object)i, item.Name);
                        }
                    }

                    //foreach (var i in DataTable)
                    //{
                    //    await App.NotesDB.SaveAsync((object)i, item.Name);
                    //}
                }
            }
        }

        public void SetTextOfLoading(ref int i, int stage = 1, string tableName = "", int countTotal = 0)
        {
            switch (stage)
            {
                case 0:
                    {
                        lblLoadingText.Text = $"Завантаження {tableName}: " + countTotal;
                        lblLoadingInt.Text = "";
                        i = 0;

                        break;
                    }
                case 1:
                    {
                        i++;
                        lblLoadingInt.Text = "/" + i.ToString();
                        break;
                    }
            }

        }

        public class ImportData
        {
            public string Name { get; set; }
            //NOTES:
            public List<Note> Note { get; set; }
            public List<NoteFlags> NoteFlags { get; set; }
            //CARS:
            public List<Cars> Cars { get; set; }
            public List<CarDescription> CarDescription { get; set; }
            public List<CarNotes> CarNotes { get; set; }

            public List<Currencies> Currencies { get; set; }

            public List<ExchangeRates> ExchangeRates { get; set; }

            public List<CashFlowDetailedType> CashFlowDetailedType { get; set; }

            public List<CashFlowOperations> CashFlowOperations { get; set; }

            public List<Clients> Clients { get; set; }
            public List<MoneyStorages> MoneyStorages { get; set; }

        }

        private void MonoToken_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MonoToken.Text != null)
            {
                CurSettings.MonoToken = MonoToken.Text;
                App.NotesDB.SaveProgramSettingsAsync(CurSettings);
            }
        }
    }
}