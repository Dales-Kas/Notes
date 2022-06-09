using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using Notes.Models;
using Notes.Models.Car;
using Notes.Models.Budget;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using System.IO;

namespace Notes.Data
{
    public class NotesDB
    {
        readonly SQLiteAsyncConnection db;
        private readonly string strOk = "►";
        private readonly string strNOk = "○";
        //Список усіх таблиць Бази Даних:
        private Dictionary<string, Type> DataDictionary = new Dictionary<string, Type>()
        {
            //NOTES:
            { "Note", typeof(Note)},
            { "NoteFlags",typeof(NoteFlags)},
            { "NoteCategory",typeof(NoteCategory)},
            //CARS:
            { "Cars",typeof(Cars)},
            { "CarDescription",typeof(CarDescription)},
            { "CarNotes",typeof(CarNotes)},
            //BUDGET:
            { "Currencies",typeof(Currencies)},
            { "ExchangeRates",typeof(ExchangeRates)},
            { "CashFlowDetailedType",typeof(CashFlowDetailedType)},
            { "CashFlowOperations",typeof(CashFlowOperations)},
            { "Clients",typeof(Clients)},
            { "MoneyStorages",typeof(MoneyStorages)},
            //Settings:
            { "ProgramSettings",typeof(ProgramSettings)}
        };

        public NotesDB(string connectionString)
        {
            db = new SQLiteAsyncConnection(connectionString);
            
            foreach (var item in DataDictionary)
            {
                db.CreateTableAsync(DataDictionary[item.Key]).Wait();
            }

        }

        public async Task DropTable(string tableName)
        {
            DataDictionary.TryGetValue(tableName,out Type type);
            await db.DropTableAsync(await db.GetMappingAsync(type));
            await db.CreateTableAsync(type);           
        }

        public Task<int> DeleteAsync(Object obj)
        {
            return db.DeleteAsync(obj);
        }

        #region NotesData

        public Task<List<Note>> GetNotesAsync()
        {
            return db.Table<Note>().ToListAsync();
        }

        public Task<Note> GetNoteAsync(Guid id)
        {
            return db.Table<Note>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public Task<Note> GetNoteAsync(int id)
        {
            return db.Table<Note>().Take(id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveNoteAsync(Note note, bool insert = false, bool createTextFromFlags = true, bool saveNoteFlags = true)
        {
            if (new DateTime(note.Date.Year, note.Date.Month, note.Date.Day) <= new DateTime(1900, 1, 1))
            {
                note.Date = DateTime.UtcNow;
                note.NoteTime = new TimeSpan(note.Date.Hour, note.Date.Minute, note.Date.Second);
            }
            else
            {
                note.Date = new DateTime(note.Date.Year, note.Date.Month, note.Date.Day, note.NoteTime.Hours, note.NoteTime.Minutes, note.NoteTime.Seconds);
            }

            if (note.IsList && createTextFromFlags)
            {
                note.Text = "";
                int i = 0;
                List<NoteFlags> allNotesflags = await GetNoteFlagsAsync(note.ID);

                foreach (NoteFlags item in allNotesflags)
                {
                    i++;
                    string enter = i > 1 ? "\n" : "";

                    if (item.Finished)
                    {
                        note.Text = note.Text + enter + strOk + item.Text;
                    }
                    else
                    {
                        note.Text = note.Text + enter + strNOk + item.Text;
                    }
                }
            }

            string curText = note.Text;

            if (!String.IsNullOrEmpty(curText))
            {
                if (curText.Length >= 200)
                {
                    curText = string.Concat(curText[..100], "\n...\n", curText.Substring(curText.Length - 99, 99));
                }
            }
            note.ShortText = curText;

            if (note.ID != Guid.Empty && !insert)
            {
                if (note.IsList && saveNoteFlags)
                {
                    await db.UpdateAsync(note);
                }
                else
                {
                    return await db.UpdateAsync(note);
                }
            }

            else
            {
                if (note.IsList && saveNoteFlags)
                {
                    await db.InsertAsync(note);
                }
                else
                {
                    return await db.InsertAsync(note);
                }
            }

            if (!note.IsList)
            {
                DeleteNoteFlagsAsync(note.ID);
            }

            if (saveNoteFlags)
            {
                await SaveNoteFlagsAsync(note.ID);
            }

            return 0;
        }
       
        #endregion

        #region NotesFlags

        public async Task<int> SaveNoteFlagsAsync(Guid noteId)
        {
            List<NoteFlags> curTable = await db.Table<NoteFlags>().Where(x => x.NoteID == noteId).ToListAsync();
            int i = 0;

            foreach (NoteFlags item in curTable)
            {
                i++;
                var resault = await SaveNoteFlagAsync(item);
            }
            return i;
        }

        public Task<int> SaveNoteFlagAsync(NoteFlags noteFlag, bool insert = false)
        {
            if (noteFlag.ID != Guid.Empty && !insert)
            {
                //#if DEBUG
                //DisplayAlert(App.GetToastOptions($"стрічка {noteFlag.ID} оновлена..."));
                //#endif
                return db.UpdateAsync(noteFlag);
            }

            else
            {
                return db.InsertAsync(noteFlag);
            }
        }
        
        public async Task<int> DeleteNoteFlagAsync(Guid noteFlagGuid)
        {
            NoteFlags noteFlag = await GetNoteFlagAsync(noteFlagGuid);
            return await DeleteAsync(noteFlag);            
        }

        public async Task DeleteNoteFlagsAsync(Guid noteId)
        {
            List<NoteFlags> curTable = await db.Table<NoteFlags>().Where(x => x.NoteID == noteId).ToListAsync();
            int i = 0;

            foreach (NoteFlags item in curTable)
            {
                i++;
                var resault = await DeleteAsync(item);
            }
        }

        public Task<List<NoteFlags>> GetNoteFlagsAsync(Guid noteId)
        {
            return db.Table<NoteFlags>().Where(x => x.NoteID == noteId).ToListAsync();
        }

        public Task<NoteFlags> GetNoteFlagAsync(Guid noteFlagId)
        {
            return db.Table<NoteFlags>().Where(x => x.ID == noteFlagId).FirstOrDefaultAsync();
        }

        public Task<NoteFlags> GetNoteFlagAsync(Guid id, Guid noteId)
        {
            return db.Table<NoteFlags>().Where(i => i.ID == id && i.NoteID == noteId).FirstOrDefaultAsync();
        }

        public async void CreateNoteFlagsFromNote(Note note)
        {
            await DeleteNoteFlagsAsync(note.ID);

            if (!string.IsNullOrEmpty(note.Text))
            {
                int i = 0;
                NoteFlags flag;

                foreach (string item in note.Text.Split('\n'))
                {
                    i++;

                    bool isFinished = item.StartsWith(strOk);

                    string curText = "";

                    if (item.StartsWith(strOk) || item.StartsWith(strNOk))
                    {
                        curText = item[1..];
                    }
                    else
                    {
                        curText = item;
                    }

                    flag = new NoteFlags
                    {
                        NoteID = note.ID,
                        Text = curText,
                        Finished = isFinished
                    };
                    await SaveNoteFlagAsync(flag);
                }
            }
        }

        #endregion

        #region NoteCategory

        public Task<NoteCategory> GetNoteCategoryAsync(int id)
        {
            return db.Table<NoteCategory>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public NoteCategory GetNoteCategory(int id)
        {
            return db.Table<NoteCategory>().Where(i => i.ID == id).FirstOrDefaultAsync().Result;
        }

        public Task<List<NoteCategory>> GetNotesCategoriesAsync()
        {
            return db.Table<NoteCategory>().ToListAsync();
        }

        public Task<int> SaveNoteCategoryAsync(NoteCategory noteCategory, bool insert = false)
        {
            if (noteCategory.ID != 0 && !insert)
            {
                return db.UpdateAsync(noteCategory);
            }

            else
            {
                return db.InsertAsync(noteCategory);
            }
        }

        public string GetNoteCategoryName(int ID)
        {
            string categoryText;

            NoteCategory noteCategory = GetNoteCategoryAsync(ID).Result;

            if (noteCategory != null)
            {
                categoryText = noteCategory.Name;
            }
            else
            {
                categoryText = "";
            }

            return categoryText;
        }
        #endregion

        #region Cars

        public Task<List<Cars>> GetCarsAsync()
        {
            return db.Table<Cars>().ToListAsync();
        }

        public Task<Cars> GetCarAsync(Guid id)
        {
            return db.Table<Cars>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public Task<List<CarDescription>> GetCarDescriptionAsync(Guid carid)
        {
            return db.Table<CarDescription>().Where(i => i.CarID == carid).ToListAsync();
        }

        public Task<List<CarNotes>> GetCarNotesAsync(Guid carid)
        {
            return db.Table<CarNotes>().Where(i => i.CarID == carid).ToListAsync();
        }

        public Task<int> SaveCarAsync(Cars car, bool insert = false)
        {
            if (car.ID != Guid.Empty && !insert)
            {
                return db.UpdateAsync(car);
            }

            else
            {
                return db.InsertAsync(car);
            }
        }

        //public Task<int> DeleteCarAsync(Cars car)
        //{
        //    return db.DeleteAsync(car);
        //}


        #endregion

        #region Currencies

        public Task<List<Currencies>> GetCurrenciesAsync()
        {
            return db.Table<Currencies>().ToListAsync();
        }
        public Task<Currencies> GetCurrenciesAsync(int id)
        {
            return db.Table<Currencies>().Where(i => i.Code == id).FirstOrDefaultAsync();
        }
        #endregion

        #region ExchangeRates

        public Task<List<ExchangeRates>> GetExchangeRatesAsync()
        {
            return db.Table<ExchangeRates>().ToListAsync();
        }
        public Task<ExchangeRates> GetExchangeRatesAsync(int currencyID, DateTime date)
        {
            return db.Table<ExchangeRates>().Where(i => i.CurrencyID == currencyID && i.Period == date).FirstOrDefaultAsync();
        }

        public Task<List<ExchangeRates>> GetExchangeRatesAsync(int currencyID)
        {
            return db.Table<ExchangeRates>().Where(i => i.CurrencyID == currencyID).ToListAsync();
        }
        #endregion

        #region CashFlowDetailedType

        public Task<List<CashFlowDetailedType>> GetCashFlowDetailedTypeAsync()
        {
            return db.Table<CashFlowDetailedType>().ToListAsync();
        }

        public Task<CashFlowDetailedType> GetCashFlowDetailedTypeAsync(Guid id)
        {
            return db.Table<CashFlowDetailedType>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        #endregion

        #region CashFlowOperations

        public Task<List<CashFlowOperations>> GetCashFlowOperationsAsync()
        {
            return db.Table<CashFlowOperations>().ToListAsync();
        }

        public Task<CashFlowOperations> GetCashFlowOperationsAsync(Guid id)
        {
            return db.Table<CashFlowOperations>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public Task<CashFlowOperations> GetCashFlowOperationsAsync(DateTime date, double amount)
        {
            return db.Table<CashFlowOperations>().Where(i => i.Date == date && i.Amount == amount).FirstOrDefaultAsync();
        }

        public Task<CashFlowOperations> GetCashFlowOperationsAsync(string id)
        {
            return db.Table<CashFlowOperations>().Where(i => i.MonoId == id).FirstOrDefaultAsync();
        }

        //public Task<int> DeleteCashFlowOperationAsync(CashFlowOperations operation)
        //{
        //    return db.DeleteAsync(operation);
        //}

        #endregion

        #region Clients

        public Task<List<Clients>> GetClientsAsync()
        {
            return db.Table<Clients>().ToListAsync();
        }

        public Task<Clients> GetClientAsync(Guid id)
        {
            return db.Table<Clients>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }
        #endregion

        #region MoneyStorages

        public Task<List<MoneyStorages>> GetMoneyStoragesAsync()
        {
            return db.Table<MoneyStorages>().ToListAsync();
        }

        public Task<MoneyStorages> GetMoneyStoragesAsync(Guid id)
        {
            return db.Table<MoneyStorages>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }
        #endregion

        #region ProgramSettings 

        public async Task<ProgramSettings> GetProgramSettingsAsync()
        {
            ProgramSettings curSett = await db.Table<ProgramSettings>().FirstOrDefaultAsync();

            if (curSett == null)
            {
                curSett = new ProgramSettings();

                await db.InsertAsync(curSett);

                return curSett;
            }

            return curSett;
        }

        public Task<int> SaveProgramSettingsAsync(ProgramSettings programSettings)
        {
            return db.UpdateAsync(programSettings);
        }

        #endregion

        #region AllClass

        public Task<int> SaveAsync(object car, string objectType, bool insert = false)
        {
            int EmtyInt = 0;
            Guid EmtyGuid = Guid.Empty;

            bool flUpdate = false;

            switch (objectType)
            {
                case "Cars":
                    {
                        flUpdate = (car as Cars).ID != EmtyGuid;
                        break;
                    }
                case "CarDescription":
                    {
                        flUpdate = ((CarDescription)car).ID != EmtyInt;
                        break;
                    }

                case "CarNotes":
                    {
                        flUpdate = ((CarNotes)car).ID != EmtyInt;
                        break;
                    }
                case "Currencies":
                    {
                        flUpdate = ((Currencies)car).ID != EmtyInt;
                        break;
                    }
            }

            if (flUpdate && !insert)
            {
                return db.UpdateAsync(car);
            }

            else
            {
                return db.InsertAsync(car);
            }
        }

        //public Task<int> DeleteCarAsync(Cars car)
        //{
        //    return db.DeleteAsync(car);
        //}

        public async Task<Object> GetFromMySQL(Guid guid, Type type)
        {
            //Чогось через цей метод програма тормозить, поки не використовую... Треба розібратись

            TableMapping tableMapping = new TableMapping(type, CreateFlags.AutoIncPK);
            try
            {
                var i = await db.GetAsync(guid, tableMapping);
                return i;
            }
            catch (Exception e)
            {
                //string msg = e.Message;                
                return null;
            }

        }
    }
    #endregion
}

