using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using Notes.Models;
using System.Threading.Tasks;
using System.IO;

namespace Notes.Data
{
    public class NotesDB
    {
        readonly SQLiteAsyncConnection db;

        public NotesDB(string connectionString)
        {
            db = new SQLiteAsyncConnection(connectionString);
            db.CreateTableAsync<Note>().Wait();
            //db.DropTableAsync<NoteFlags>().Wait();   
            db.CreateTableAsync<NoteFlags>().Wait();
        }

        public Task<List<Note>> GetNotesAsync()
        {
            return db.Table<Note>().ToListAsync();
        }

        public Task<Note> GetNoteAsync(int id)
        {
            return db.Table<Note>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveNoteAsync(Note note)
        {
            if (note.ID != 0)
            {
                return db.UpdateAsync(note);
            }

            else
            {
                return db.InsertAsync(note);
            }

            //return SaveNoteFlagsAsync(note.ID);
        }

        public Task<int> DeleteNoteAsync(Note note)
        {
            return db.DeleteAsync(note);
        }

        #region NotesFlags

        public async Task<int> SaveNoteFlagsAsync(int noteId)
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

        public Task<int> SaveNoteFlagAsync(NoteFlags noteFlag)
        {
            if (noteFlag.ID != Guid.Empty)
            {
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

        public async void DeleteNoteFlagsAsync(int noteId)
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

        public Task<List<NoteFlags>> GetNoteFlagsAsync(int noteId)
        {
            return db.Table<NoteFlags>().Where(x => x.NoteID == noteId).ToListAsync();
            //return db.Table<NoteFlags>().ToListAsync();
        }

        public Task<NoteFlags> GetNoteFlagAsync(Guid id, int noteId)
        {
            return db.Table<NoteFlags>().Where(i => i.ID == id && i.NoteID == noteId).FirstOrDefaultAsync();
        }

        #endregion

    }
}
