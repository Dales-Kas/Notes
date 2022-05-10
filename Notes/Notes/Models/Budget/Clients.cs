using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Notes.Models.Budget
{
    public class Clients
    {
        [PrimaryKey,AutoIncrement]
        public Guid ID { get; set; }
        public Guid ParentID { get; set; }
        public bool IsGroup { get; set; }

        public string Name { get; set; }

        public string Comment { get; set; }
        public Guid DefaultCashFlowDetailedType { get; set; }

        public double PercentageCommission { get; set; }

    }

    public class ClientsToShow : Clients
    {
        public string FullName { get; set; }
    }
}
