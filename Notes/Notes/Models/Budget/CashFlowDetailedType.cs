using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Notes.Models.Budget
{
    public class CashFlowDetailedType
    {
        [PrimaryKey, AutoIncrement]
        public Guid ID { get; set; }

        public string Code { get; set; }
        
        public string Name { get; set; }

        public string Descripton { get; set; }

        public bool DontUseInCashFlow { get; set; }

        public bool DontUseInPlanning { get; set; }

        public string OperationColor { get; set; }

        public string OperationTextColor { get; set; }

        public string MCC { get; set; }
        public int MCC_Count { get; set; }        
    }
}
