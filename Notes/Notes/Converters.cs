using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Notes.Models.Budget;
using Xamarin.Forms;

namespace Notes
{
    public class GetClientNameFromGuid : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Guid)value == Guid.Empty)
            {
                return "";
            }

            Clients clients = App.NotesDB.GetClientAsync((Guid)value).Result;

            if (clients != null)
            {
                return clients.Name;
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }

    public class GetCashFlowDetailedTypeFromGuid : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Guid)value == Guid.Empty)
            {
                return "";
            }

            CashFlowDetailedType type = App.NotesDB.GetCashFlowDetailedTypeAsync((Guid)value).Result;

            if (type != null)
            {
                return type.Name;
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }

    public class GetOperationTypeColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 0)
            {
                return Color.Black.ToHex().ToString();
            }
            else if ((int)value == 1)
            {
                return Color.Red.ToHex().ToString();
            }

            return Color.Blue.ToHex().ToString();            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }

    public class GetDetailedOperationTypeColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string defaultColor = Color.White.ToHex().ToString();

            if ((Guid)value == Guid.Empty)
            {
                return defaultColor;
            }

            CashFlowDetailedType type = App.NotesDB.GetCashFlowDetailedTypeAsync((Guid)value).Result;

            if (type != null)
            {
                return type.OperationColor;
            }

            return defaultColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }

    public class GetGroupTypeColor : IValueConverter
    {
        public string DefaultColor { get; set; }
        public string SetColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {            

            if ((bool)value == true)
            {
                return SetColor;
            }

            return DefaultColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }

    public class GetUnbool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;                        
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }
    public class GetVisabilityOfGuidRef : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Guid)value == Guid.Empty)
            {
                return false;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }

    public class GetVisabilityOfString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty((string)value))
            {
                return false;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }

    public class GetVisabilityOfDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((double)value==0)
            {
                return false;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }
}
