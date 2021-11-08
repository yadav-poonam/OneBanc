using System;

namespace OneBanc
{
    class OutputFileTransactionDetails
    {
        public DateTime date;
        double debit, credit;
        string currency, cardName, location, transactionDescription;
        public string transactionType;
        public OutputFileTransactionDetails(DateTime date , string transactionDescription, double debit, double credit, string currency, string cardName, string transactionType, string location )
        {
            this.date = date;
            this.transactionDescription = transactionDescription;
            this.debit = debit;
            this.credit = credit;
            this.currency = currency;
            this.cardName = cardName;
            this.transactionType = transactionType;
            this.location = location;
        }
        public string getString()
        {
            return (this.date.ToString("dd/MM/yyyy") + ", " + transactionDescription + ", " + debit.ToString() + ", " + credit.ToString() + ", " + currency + ", " + cardName + ", " + transactionType + ", " + location);
        }

    }
}