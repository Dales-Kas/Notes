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
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void ImportJSON_Clicked(object sender, EventArgs e)
        {
            var res = PickAndShow();
        }

        private async Task<FileResult> PickAndShow()
        {
            string resault1 = await DisplayActionSheet("Виберіть тип завантаження", "Відміна", null, "NotesAndNotesFlags", "Cars","Budget", "ALL");

            if (resault1 != null && resault1 != "Відміна")
            {

            }
            else
            {
                return null;
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
                    string Text = $"File Name: {result.FileName}";
                    var stream = await result.OpenReadAsync();
                    var reader = new System.IO.StreamReader(stream);

                    var jsonString = reader.ReadToEnd();

                    switch (resault1)
                    {
                        case "NotesAndNotesFlags":
                            {
                                App.NotesDB.DropTable(nameof(Note),false);

                                await ImportAndSaveToBase(jsonString);

                                break;
                            }
                        case "Cars":
                            {
                                App.NotesDB.DropTable(nameof(Cars),false);

                                await ImportAndSaveToBase(jsonString);

                                break;
                            }
                        case "Budget":
                            {
                                App.NotesDB.DropTable("Budget",false);

                                await ImportAndSaveToBase(jsonString);

                                break;
                            }

                        case "ALL":
                            {
                                //App.NotesDB.DropTable(nameof(Note));
                                //App.NotesDB.DropTable(nameof(Cars));
                                //App.NotesDB.DropTable("Budget");

                                await ImportAndSaveToBase(jsonString);

                                break;
                            }
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("УВАГА!", ex.Message, "ОК");
            }

            await DisplayAlert("Дані завантажено","", "ОК");

            return null;
        }

        public async Task ImportAndSaveToBase(string jsonString)
        {
            List<ImportData> jsonData = JsonConvert.DeserializeObject<List<ImportData>>(jsonString);

            foreach (ImportData item in jsonData)
            {
                App.NotesDB.DropTable(item.Name);

                if (item.Name == "Notes")
                {
                    foreach (Note note in item.Notes)
                    {
                        note.Date = DateTime.Now;
                        await App.NotesDB.SaveNoteAsync(note, true, true, false);
                    }
                }
                else if (item.Name == "NotesFlags")
                {
                    foreach (NoteFlags noteFlag in item.NotesFlags)
                    {
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
                    foreach (Cars car in item.Cars)
                    {
                        await App.NotesDB.SaveCarAsync(car, true);
                    }
                }
                else
                {
                    //dynamic DataTable = null;

                    if (item.Name == "CarDescription")
                    {
                        //DataTable = item.CarDescription;
                        foreach (CarDescription i in item.CarDescription)
                        {
                            await App.NotesDB.SaveAsync((object)i, item.Name);
                        }
                    }
                    else if (item.Name == "CarNotes")
                    {
                        //DataTable = item.CarDescription;
                        foreach (CarNotes i in item.CarNotes)
                        {
                            await App.NotesDB.SaveAsync((object)i, item.Name);
                        }
                    }
                    else if (item.Name == "Currencies")
                    {
                        //DataTable = item.Currencies;
                        foreach (var i in item.Currencies)
                        {
                            await App.NotesDB.SaveAsync((object)i, item.Name,true,true);
                        }
                    }

                    else if (item.Name == "CashFlowDetailedType")
                    {
                        foreach (var i in item.CashFlowDetailedType)
                        {
                            await App.NotesDB.SaveAsync((object)i, item.Name);
                        }
                    }

                    else if (item.Name == "CashFlowOperations")
                    {
                        foreach (var i in item.CashFlowOperations)
                        {
                            await App.NotesDB.SaveAsync((object)i, item.Name);
                        }
                    }

                    else if (item.Name == "Clients")
                    {
                        foreach (var i in item.Clients)
                        {
                            await App.NotesDB.SaveAsync((object)i, item.Name);
                        }
                    }

                    else if (item.Name == "MoneyStorages")
                    {
                        foreach (var i in item.MoneyStorages)
                        {
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

        public class ImportData
        {
            public string Name { get; set; }
            //NOTES:
            public List<Note> Notes { get; set; }
            public List<NoteFlags> NotesFlags { get; set; }
            //CARS:
            public List<Cars> Cars { get; set; }
            public List<CarDescription> CarDescription { get; set; }
            public List<CarNotes> CarNotes { get; set; }

            public List<Currencies> Currencies { get; set; }

            public List<CashFlowDetailedType> CashFlowDetailedType { get; set; }

            public List<CashFlowOperations> CashFlowOperations { get; set; }

            public List<Clients> Clients { get; set; }
            public List<MoneyStorages> MoneyStorages { get; set; }

        }
    }
}