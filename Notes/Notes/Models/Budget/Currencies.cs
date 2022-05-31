﻿using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Notes.Models.Budget
{
    public class Currencies
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public int Code { get; set; }
        public string Descripton { get; set; }

        public string CharName { get; set; }

        public string CharCode { get; set; }        
    }

    public class ExchangeRates 
    {
        public int CurrencyID { get; set; }

        public DateTime Period { get; set; }
        public double Rate { get; set; }

        public double Multiply { get; set; }        
    }
}
