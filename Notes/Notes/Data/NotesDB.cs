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
        readonly SQLiteConnection db_add;
        private readonly string strOk = "►";
        private readonly string strNOk = "○";
        //Список усіх таблиць Бази Даних:
        private readonly Dictionary<string, Type> dataDictionary = new Dictionary<string, Type>();

        ////NOTES:
        //{ "Note", typeof(Note)},
        //{ "NoteFlags",typeof(NoteFlags)},
        //{ "NoteCategory",typeof(NoteCategory)},
        ////CARS:
        //{ "Cars",typeof(Cars)},
        //{ "CarDescription",typeof(CarDescription)},
        //{ "CarNotes",typeof(CarNotes)},
        ////BUDGET:
        //{ "Currencies",typeof(Currencies)},
        //{ "ExchangeRates",typeof(ExchangeRates)},
        //{ "CashFlowDetailedType",typeof(CashFlowDetailedType)},
        //{ "CashFlowOperations",typeof(CashFlowOperations)},
        //{ "Clients",typeof(Clients)},
        //{ "MoneyStorages",typeof(MoneyStorages)},
        //{ "ClientIdentificationTexts",typeof(ClientIdentificationTexts)},
        ////Settings:
        //{ "ProgramSettings",typeof(ProgramSettings)}     

        public NotesDB(string connectionString)
        {
            db = new SQLiteAsyncConnection(connectionString);
            db_add = new SQLiteConnection(connectionString);

            dataDictionary.Clear();
            foreach (Type item in GetAllTablesName())
            {
                dataDictionary.Add(item.Name, item);
                db.CreateTableAsync(dataDictionary[item.Name]).Wait();
            }
        }

        public List<Type> GetAllTablesName()
        {
            return new List<Type>() { 

            //NOTES:
            { typeof(Note)},
            { typeof(NoteFlags)},
            { typeof(NoteCategory)},
            //CARS:
            { typeof(Cars)},
            { typeof(CarDescription)},
            { typeof(CarNotes)},
            //BUDGET:
            { typeof(Currencies)},
            { typeof(ExchangeRates)},
            { typeof(CashFlowDetailedType)},
            { typeof(CashFlowOperations)},
            { typeof(Clients)},
            { typeof(MoneyStorages)},
            { typeof(ClientIdentificationTexts)},
            { typeof(MCCCodes)},
            //Settings:
            { typeof(ProgramSettings)}
            };
        }

        public async Task DropTable(string tableName)
        {
            dataDictionary.TryGetValue(tableName, out Type type);
            await db.DropTableAsync(await db.GetMappingAsync(type));
            await db.CreateTableAsync(type);
        }

        public Task<int> DeleteAsync(Object obj)
        {
            return db.DeleteAsync(obj);
        }

        public Task<int> SaveAsync(object obj, bool insert = false)
        {
            int EmtyInt = 0;
            Guid EmtyGuid = Guid.Empty;

            string objectType = obj.GetType().Name;

            bool flUpdate = false;
           
            switch (objectType)
            {
                case "Cars":
                    {
                        flUpdate = (obj as Cars).ID != EmtyGuid;
                        break;
                    }
                case "CarDescription":
                    {
                        flUpdate = ((CarDescription)obj).ID != EmtyInt;
                        break;
                    }

                case "CarNotes":
                    {
                        flUpdate = ((CarNotes)obj).ID != EmtyInt;
                        break;
                    }
                case "Currencies":
                    {
                        flUpdate = ((Currencies)obj).ID != EmtyInt;
                        break;
                    }

                case "NoteCategory":
                    {
                        flUpdate = ((NoteCategory)obj).ID != EmtyInt;
                        break;
                    }
                case "NoteFlags":
                    {
                        flUpdate = ((NoteFlags)obj).ID != EmtyGuid;
                        break;
                    }
                case "CashFlowOperations":
                    {
                        flUpdate = ((CashFlowOperations)obj).ID != EmtyGuid;
                        break;
                    }

            }

            if (flUpdate && !insert)
            {
                return db.UpdateAsync(obj);
            }

            else
            {
                return db.InsertAsync(obj);
            }
        }

        public async Task<List<T>> SelectAllAsyncFrom<T, F>(F id = default, string name = "ID") where T : new() 
        {
            var mapping = await db.GetMappingAsync<T>();

            string query = String.Format("select * from {0}", mapping.TableName);

            if (!id.Equals(default(F)))
            {
                query += $" where {name} = ?";

                return await db.QueryAsync<T>(query, id);                
            }

            return await db.QueryAsync<T>(query);           
        }

        public async Task<T> SelectAsyncFrom<T,F>(F id = default, string name = "ID") where T : new()
        {
            var mapping = await db.GetMappingAsync<T>();
            string query = $"select * from {mapping.TableName}";

            List<T> resault;

            if (!id.Equals(default(F)))
            {
                query += $" where {name} = ?";

                resault = await db.QueryAsync<T>(query, id);
                
            }
            else 
            {
                resault = await db.QueryAsync<T>(query);
            }
            
            return resault.Count > 0 ? resault[0] : default(T);
        }

        public T SelectFrom<T,F>(F id = default, string name = "ID") where T : new()
        {            
            var mapping = db_add.GetMapping<T>();
            string query = $"select * from {mapping.TableName}";

            List<T> resault;

            if (!id.Equals(default(F)))
            {
                query += $" where {name} = ?";

                resault = db_add.Query<T>(query, id);                
            }
            else
            {
                resault = db_add.Query<T>(query);
            }

            return resault.Count > 0 ? resault[0] : default(T);
        }

        public Task<int> InsertAllFrom<T>(List<T> list)
        {
            return db.InsertAllAsync(list, true);
        }       

        #region NotesData

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
                List<NoteFlags> allNotesflags = await SelectAllAsyncFrom<NoteFlags,Guid>(note.ID, name: "NoteID");//GetNoteFlagsAsync(note.ID);

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
                await DeleteNoteFlagsAsync(note.ID);
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
            List<NoteFlags> curTable = await SelectAllAsyncFrom<NoteFlags,Guid>(noteId,name:"NoteID");
            int i = 0;

            foreach (NoteFlags item in curTable)
            {
                i++;
                var resault = await SaveAsync(item);
            }
            return i;
        }
 
        public async Task<int> DeleteNoteFlagAsync(Guid noteFlagGuid)
        {
            NoteFlags noteFlag = await SelectAsyncFrom<NoteFlags,Guid>(noteFlagGuid);
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
                    await SaveAsync(flag);
                }
            }
        }

        #endregion

        #region NoteCategory
                        
        public string GetNoteCategoryName(int ID)
        {
            string categoryText;

            NoteCategory noteCategory = SelectFrom<NoteCategory,int>(ID);

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


        #region ExchangeRates

        public Task<ExchangeRates> GetExchangeRatesAsync(int currencyID, DateTime date)
        {
            return db.Table<ExchangeRates>().Where(i => i.CurrencyID == currencyID && i.Period == date).FirstOrDefaultAsync();
        }

        public async Task<double> GetSetExchangeRateAsync(int currencyID, DateTime date)
        {
            ExchangeRates curExchRate = await GetExchangeRatesAsync(currencyID, date);

            if (curExchRate == null)
            {
                var curExchRateAmount = await Services.MonobankAPI.GetMonoCurrencyExchRate(currencyID, 980, date);

                if (curExchRateAmount!=0)
                {
                    await SaveAsync(new ExchangeRates() { CurrencyID = currencyID, Rate = curExchRateAmount, Multiply = 1, Period = new DateTime(date.Year,date.Month,date.Day)},true);
                }                
                return curExchRateAmount;
            }

            return curExchRate.Multiply == 0 ? 0 : curExchRate.Rate / curExchRate.Multiply;
        }

        #endregion

        #region CashFlowOperations        
        public Task<CashFlowOperations> GetCashFlowOperationsAsync(DateTime date, double amount)
        {
            return db.Table<CashFlowOperations>().Where(i => i.Date == date && i.Amount == amount).FirstOrDefaultAsync();
        }        

        public async Task<List<CashFlowOperations>> GetAllCashOperations(DateTime periodStart, DateTime periodEnd, CashFlowDetailedType detailedTypeFilter, Clients clientFilter, DateTime dateFilter, bool inOperationFilter, bool outOperationFilter)
        {
            var mapping = await db.GetMappingAsync<CashFlowOperations>();
            string query = $"select * from {mapping.TableName}";

            query += " where Date >= ? and Date <= ?";

            DateTime dateFilterMin = new DateTime(dateFilter.Date.Year, dateFilter.Date.Month, dateFilter.Date.Day);
            DateTime dateFilterMax = new DateTime(dateFilter.Date.Year, dateFilter.Date.Month, dateFilter.Date.Day, 23, 59, 59);

            query += " and case when ? then Date >= ? and Date <= ? else true end";

            Guid typeID = Guid.NewGuid();

            if (detailedTypeFilter != null)
            {
                typeID = detailedTypeFilter.ID;     
            }
            
            query += " and case when ? then DetailedTypeID = ? else true end";

            Guid clientID = Guid.NewGuid();

            if (clientFilter != null)
            {
                clientID = clientFilter.ID;                
            }

            query += " and case when ? then Client = ? else true end";
            
            query += " and case when ? then OperationType = ? else true end";

            query += " and case when ? then OperationType = ? else true end";

            query += " ORDER BY Date desc";

            return await db.QueryAsync<CashFlowOperations>(query, periodStart, periodEnd, dateFilter > DateTime.MinValue, dateFilterMin, dateFilterMax, detailedTypeFilter != null, typeID, clientFilter != null, clientID, inOperationFilter, Models.OperationType.InOperation, outOperationFilter, Models.OperationType.OutOperation);
        }
        
        public async Task<List<Balance>> GetBalance(DateTime dateEnd)
        {
            var mapping = await db.GetMappingAsync<CashFlowOperations>();
            
            string amounttext = "item.Amount + item.AmountDelayCommission + case when item.IsIncludeCommission then 0 else item.AmountCommission end";

            string amountcondition =
                @$"sum(case when Not item.IsPlan then 
                   case when item.OperationType = 0 then 1                    
                        when item.OperationType = 1 then -1
                        else 0
                   end *
                   case when item.TypeID = %TypeID% then {amounttext}                        
                   else 0
                   end
                else 0 
                end)";

            //string amounttext = "item.Amount";

            string query = 
                $@"SELECT
                    {amountcondition.Replace("%TypeID%", "0")} as Balance0,
                    {amountcondition.Replace("%TypeID%", "1")} as Balance1,
                    {amountcondition.Replace("%TypeID%", "2")} as Balance2
                FROM {mapping.TableName} as item
                WHERE 
                    item.Date <= ?";         

            return await db.QueryAsync<Balance>(query, dateEnd);            
        }

        public class Balance
        {
            public double Balance0 { get; set; }
            public double Balance1 { get; set; }
            public double Balance2 { get; set; }
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
              
        public async Task<object> GetFromMySQL(Guid guid, Type type)
        {
            //Чогось через цей метод програма тормозить, поки не використовую... Треба розібратись

            //TableMapping tableMapping = new TableMapping(type, CreateFlags.AutoIncPK);
                var mapping = await db.GetMappingAsync(type);
                return await db.GetAsync(guid,mapping);                
            //try
            //{
            //}
            //catch (Exception e)
            //{
            //    //string msg = e.Message;                
            //    return null;
            //}

        }
    }
    #endregion
}

