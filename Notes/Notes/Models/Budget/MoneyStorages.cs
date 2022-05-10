using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Notes.Models.Budget
{
    public class MoneyStorages
    {
        [PrimaryKey, AutoIncrement]
        public Guid ID { get; set; }        
        public string Name { get; set; }
    }
}
