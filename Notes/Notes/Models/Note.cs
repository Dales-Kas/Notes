using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SQLite;

namespace Notes.Models
{
    public class Note
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Text { get; set; }
        
        public string Descripton { get; set; }
        public DateTime Date { get; set; }

        public TimeSpan NoteTime { get; set; }

        public bool IsArchived { get; set; }

        public bool IsList { get; set; }

        public double Rate { get; set; }

        public int NoteFontSize { get; set; } = 18;

        //public Color NoteColor { get; set; }
        public string NoteColorText { get; set; } = "#000000";
        public string NoteColorBackGround { get; set; } = "#ffffff";

    }
}
