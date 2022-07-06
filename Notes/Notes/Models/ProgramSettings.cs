﻿using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Notes.Models
{
    public class ProgramSettings
    {
        [AutoIncrement,PrimaryKey]
        public int ID { get; set; } 

        public string MonoToken { get; set; }
        
        public int MainCurrency { get; set; }
        public int AdditionalСurrency { get; set; }

        public Guid MonoStorageID { get; set; }

        public Guid CashStorageID { get; set; }
    }
}
