using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExpenseTracker.Models
{
    // Represents a Debt record with various attributes
    public class Debt
    {
        public Guid DebtId { get; set; } = Guid.NewGuid(); // Auto-generate a unique ID
        public string DebtDescription { get; set; }
        public string DebtType { get; set; }
        public decimal DebtAmount { get; set; }
        public string DebtCategory { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }

        public DateTime WhenAdded { get; set; }
    }

    // Represents the database model for storing debts and categories
    public class DebtDatabaseModel
    {
        public List<Debt> Debts { get; set; } = new List<Debt>();
        public List<string> Categories { get; set; } = new List<string>();
    }

    // Provides methods to load and save the debt database as a JSON file
    public static class DebtDatabase
    {
        // Path to save the JSON file in the local app data folder
        private static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DebtDB.json");

        public static async Task<DebtDatabaseModel> LoadDatabaseAsync()
        {
            if (File.Exists(FilePath))
            {
                var json = await File.ReadAllTextAsync(FilePath);

                // Deserialize JSON into a DebtDatabaseModel
                return JsonSerializer.Deserialize<DebtDatabaseModel>(json) ?? new DebtDatabaseModel();
            }
            return new DebtDatabaseModel(); //Return new database if file dont exist
        }

        // Saves the current database state to the file as formatted JSON
        public static async Task SaveDatabaseAsync(DebtDatabaseModel database)
        {
            var json = JsonSerializer.Serialize(database, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(FilePath, json); // Write the serialized content to the file
        }
    }
}
