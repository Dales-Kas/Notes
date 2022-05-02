using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Notes.Models
{
    public class NoteFlags
    {
        //public NoteFlags()
        //{

        //}
        //public NoteFlags(int noteID, string text, bool finished=true)
        //{
        //    NoteID = noteID;
        //    Text = text;
        //    Finished = finished;
        //}

        [PrimaryKey, AutoIncrement]
        public Guid ID { get; set; }

        public Guid NoteID { get; set; }
        public bool Finished { get; set; }

        public string Text { get; set; }

        public int Code { get; set; }            
    }
}
