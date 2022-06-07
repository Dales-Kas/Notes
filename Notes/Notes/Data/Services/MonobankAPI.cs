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
            //ЗапросHTTP = Новый HTTPЗапрос("personal/statement/" + account + "/" + date_from + "/" + date_to, ЗаголовкиОсновногоЗапроса);
            return new List<Statement>();
        }

        private static string GetMonoToken()
        {
            return "";
        }

        public class Statement
        {
            //https://api.monobank.ua/personal/statement/{account}/{from}/{to}
            string id { get; set; }
            Int64 time { get; set; }

            string description { get; set; }

            int mcc { get; set; }

            int originalMcc { get; set; }

            bool hold { get; set; }

            Int64 amount { get; set; }

            Int64 operationAmount { get; set; }

            int currencyCode { get; set; }

            Int64 commissionRate { get; set; }
            Int64 cashbackAmount { get; set; }
            Int64 balance { get; set; }
            string comment { get; set; }
            string receiptId { get; set; }

            string invoiceId { get; set; }
            string counterEdrpou { get; set; }

            string counterIban { get; set; }
        }
    }
}
