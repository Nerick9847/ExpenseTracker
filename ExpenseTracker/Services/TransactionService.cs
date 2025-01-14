using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Services
{
    public class TransactionService
    {
        private List<Transaction> transactions = new List<Transaction>();

        public List<Transaction> GetTransactions()
        {
            return transactions;
        }

        public void AddTransaction(Transaction transaction)
        {
            transactions.Add(transaction);
        }

        public decimal GetTotalIncome()
        {
            return transactions.Where(t => t.Type == "Income").Sum(t => t.Amount);
        }

        public decimal GetTotalExpense()
        {
            return transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount);
        }

        public decimal GetTotalDebt()
        {
            return transactions.Where(t => t.Type == "Debt").Sum(t => t.Amount);
        }
    }

    public class Transaction
    {
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } // "Income", "Expense", "Debt"
    }
}
