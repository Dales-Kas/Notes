using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Models
{    
    public enum OperationType
    {
        InOperation,
        OutOperation,
        UnknownOperation
    }

    public enum BankCardType
    {
        VisaMastercard,
        Monobank
    }

    public enum OperationValue
    {
        Cash,
        NonCash
    }

}
