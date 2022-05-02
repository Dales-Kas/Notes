using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Models
{
    public class NoteCategory
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
