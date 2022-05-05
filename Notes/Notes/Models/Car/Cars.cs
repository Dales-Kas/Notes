using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Notes.Models.Car
{
    public class Cars
    {
        [PrimaryKey, AutoIncrement]
        public Guid ID { get; set; } 

        public string Name { get; set; }
        public DateTime PurchaseDate { get; set; }

        public DateTime SaleDate { get; set; }

        public string Comment { get; set; }

        public string Number { get; set; }

        public string VINNumber { get; set; }

        public string CarColor { get; set; }

        public DateTime ManufactureDate { get; set; }

        public string FuelType { get; set; }

        public int OdoStart { get; set; }
        
        public int OdoTotal { get; set; }
        
        public int OdoEnd { get; set; }

        public double AverageFlow { get; set; }

    }

    public class CarDescription
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public Guid CarID { get; set; }
        
        // string №:
        public int Code { get; set; }

        public string DescrValue { get; set; }
        public string DescrProperty { get; set; }

    }

    public class CarNotes
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public Guid CarID { get; set; }

        // string №:
        public int Code { get; set; }
        public string Text { get; set; }
    }
}
