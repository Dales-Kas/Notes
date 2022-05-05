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
            string resault1 = await DisplayActionSheet("Виберіть тип завантаження", "Відміна", null, "NotesAndNotesFlags", "Cars");

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
                                App.NotesDB.DropTable(nameof(Note));

                                await ImportAndSaveToBase(jsonString);

                                break;
                            }
                        case "Cars":
                            {
                                App.NotesDB.DropTable(nameof(Cars));

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

            return null;
        }

        public async Task ImportAndSaveToBase(string jsonString)
        {
            List<ImportData> jsonData = JsonConvert.DeserializeObject<List<ImportData>>(jsonString);

            foreach (ImportData item in jsonData)
            {
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
                else if (item.Name == "CarDescription")
                {
                    foreach (CarDescription carDescription in item.CarDescription)
                    {
                        //await App.NotesDB.SaveCarAsync(car, true);
                        await App.NotesDB.SaveAsync((object)carDescription, nameof(carDescription), true);
                    }
                }
                else if (item.Name == "CarNotes")
                {
                    foreach (CarNotes carNotes in item.CarNotes)
                    {
                        //await App.NotesDB.SaveCarAsync(car, true);
                        await App.NotesDB.SaveAsync((object)carNotes, nameof(carNotes), true);
                    }
                }
            }
        }

        public class ImportData
        {
            public string Name { get; set; }
            //NOTES:
            public List <Note> Notes { get; set; }
            public List <NoteFlags> NotesFlags { get; set; }
            //CARS:
            public List <Cars> Cars { get; set; }
            public List<CarDescription> CarDescription { get; set; }
            public List<CarNotes> CarNotes { get; set; }
        }
    }
}