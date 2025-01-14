using System;

namespace ExpenseTracker.Models
{
    public class FinancialTransaction
    {
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
    }
}
