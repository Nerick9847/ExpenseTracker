using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExpenseTracker.Models
{
    public class Debt
    {
        public Guid DebtId { get; set; } = Guid.NewGuid();
        public string DebtDescription { get; set; }
        public string DebtType { get; set; }
        public decimal DebtAmount { get; set; }
        public string DebtCategory { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }

        public DateTime WhenAdded { get; set; }
    }

    public class DebtDatabaseModel
    {
        public List<Debt> Debts { get; set; } = new List<Debt>();
        public List<string> Categories { get; set; } = new List<string>();
    }

    public static class DebtDatabase
    {
        private static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DebtDB.json");

        public static async Task<DebtDatabaseModel> LoadDatabaseAsync()
        {
            if (File.Exists(FilePath))
            {
                var json = await File.ReadAllTextAsync(FilePath);
                return JsonSerializer.Deserialize<DebtDatabaseModel>(json) ?? new DebtDatabaseModel();
            }
            return new DebtDatabaseModel();
        }

        public static async Task SaveDatabaseAsync(DebtDatabaseModel database)
        {
            var json = JsonSerializer.Serialize(database, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(FilePath, json);
        }
    }
}
