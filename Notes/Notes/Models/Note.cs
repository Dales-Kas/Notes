using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Notes.Models
{
    public class Note
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }

        public bool IsArchived { get; set; }

        public bool IsList { get; set; }

        public double Rate { get; set; }

        //public NoteFlags[] Flags { get; set; } 

    }

    public class NoteFlags
    {
        public bool Finished { get; set; }

        public string Text { get; set; }
    }
}
