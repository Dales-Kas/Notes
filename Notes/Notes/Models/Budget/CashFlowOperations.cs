using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Notes.Models.Budget
{
    public class CashFlowOperations
    {
        [PrimaryKey, AutoIncrement]
        public Guid ID { get; set; }

        public string MonoId { get; set; }

        public Guid ID_Parent { get; set; }

        public DateTime Date { get; set; }

        public OperationType OperationType { get; set; }

        public  int TypeID { get; set; }

        public Guid Client { get; set; }

        public Guid StorageID { get; set; }

        public Guid DetailedTypeID { get; set; }

        public int CurrencyID { get; set; }
        public double Amount { get; set; }

        public string Description { get; set; }

        public double AmountСurrency { get; set; }

        public double ExchangeRate { get; set; }

        public double CurrencyExchangeRate { get; set; }        

        public bool IsIncludeCommission { get; set; }

        public bool IsСurrencyOperartion { get; set; }

        public bool IsFixedOperartion { get; set; }

        public bool IsPlan { get; set; }

        public double AmountCommission { get; set; }
        public double AmountDelayCommission { get; set; }

        public double AmountBalanceAfterOperation { get; set; }

        public bool DontUseInCashFlow { get; set; }

        public double AmountInSMS { get; set; }
        
        //public string TextInSMS { get; set; }

        public string TextToIdentifyClient { get; set; }
        
        public string MCC { get; set; }

    }

    public class CashFlowOperationsToShow : CashFlowOperations
    {
        public string ClientName { get; set; }
        public string StorageName { get; set; }

        public string DetailedTypeName { get; set; }

        public string CurrencyName { get; set; }

        public string OperationColor { get; set; }
    }
}
