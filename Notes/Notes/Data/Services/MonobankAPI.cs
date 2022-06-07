using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Notes.Data.Services
{
    public static class MonobankAPI
    {

        public static List<Statement> GetStatement(DateTime date, int account)
        {
            //"personal/statement/" + account + "/" + date_from + "/" + date_to);
            return new List<Statement>();
        }

        private static string GetMonoToken()
        {
            return "";
        }

        public class Statement
        {
            //https://api.monobank.ua/personal/statement/{account}/{from}/{to}
            string id;
            Int64 time;

            string description;

            int mcc;

            int originalMcc;

            bool hold;

            Int64 amount;

            Int64 operationAmount;

            int currencyCode;

            Int64 commissionRate;
            Int64 cashbackAmount;
            Int64 balance;
            string comment;
            string receiptId;

            string invoiceId;
            string counterEdrpou;

            string counterIban;
        }
    }
}
