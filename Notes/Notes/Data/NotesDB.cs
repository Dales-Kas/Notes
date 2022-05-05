﻿using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using Notes.Models;
using Notes.Models.Car;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using System.IO;

namespace Notes.Data
{
    public class NotesDB
    {
        readonly SQLiteAsyncConnection db;
        private string strOk = "►";
        private string strNOk = "○";

        public NotesDB(string connectionString)
        {
            db = new SQLiteAsyncConnection(connectionString);
            //NOTES:
            db.CreateTableAsync<Note>().Wait();
            db.CreateTableAsync<NoteFlags>().Wait();
            db.CreateTableAsync<NoteCategory>().Wait();
            //CARS:
            db.CreateTableAsync<Cars>().Wait();
            db.CreateTableAsync<CarDescription>().Wait();
            db.CreateTableAsync<CarNotes>().Wait();
        }

        public void DropTable(string tableName)
        {
            if (tableName == "Note")
            {
                db.DropTableAsync<Note>().Wait();
                db.CreateTableAsync<Note>().Wait();
                db.DropTableAsync<NoteFlags>().Wait();
                db.CreateTableAsync<NoteFlags>().Wait();
            }
            else if (tableName == "Cars")
            {
                db.DropTableAsync<Cars>().Wait();
                db.CreateTableAsync<Cars>().Wait();
                db.DropTableAsync<CarNotes>().Wait();
                db.CreateTableAsync<CarNotes>().Wait();
                db.DropTableAsync<CarDescription>().Wait();
                db.CreateTableAsync<CarDescription>().Wait();
            }
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
                    curText = string.Concat(curText.Substring(0, 100), "\n...\n", curText.Substring(curText.Length - 99, 99));
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

        public Task<int> DeleteNoteAsync(Note note)
        {
            return db.DeleteAsync(note);
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

        public Task<int> DeleteNoteFlagAsync(NoteFlags noteFlag)
        {
            return db.DeleteAsync(noteFlag);
        }

        public async Task<int> DeleteNoteFlagAsync(Guid noteFlagGuid)
        {
            NoteFlags noteFlag = await GetNoteFlagAsync(noteFlagGuid);
            await DeleteNoteFlagAsync(noteFlag);
            return 1;
        }

        public async void DeleteNoteFlagsAsync(Guid noteId)
        {
            List<NoteFlags> curTable = await db.Table<NoteFlags>().Where(x => x.NoteID == noteId).ToListAsync();
            int i = 0;

            foreach (NoteFlags item in curTable)
            {
                i++;
                //i = i + 
                var resault = await DeleteNoteFlagAsync(item);
            }

            //return i;
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
            DeleteNoteFlagsAsync(note.ID);

            if (!string.IsNullOrEmpty(note.Text))
            {
                int i = 0;
                NoteFlags flag = null;

                foreach (string item in note.Text.Split('\n'))
                {
                    i++;

                    bool isFinished = item.StartsWith(strOk);

                    string curText = "";

                    if (item.StartsWith(strOk) || item.StartsWith(strNOk))
                    {
                        curText = item.Substring(1);
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
            string categoryText = "";

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

        public Task<int> DeleteCarAsync(Cars car)
        {
            return db.DeleteAsync(car);
        }


        #endregion

        #region AllClass

        public Task<int> SaveAsync(object car, string objectType, bool findByID = true, bool insert = false)
        {
            int EmtyInt = 0;
            Guid EmtyGuid = Guid.Empty;
            
            bool flUpdate = false;

            switch (objectType)
            {
                case "Cars": 
                    {
                        flUpdate = ((Cars)car).ID != EmtyGuid;
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

        #endregion
    }
}
