using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

// Represents a Transaction record with various attributes
public class FinancialTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid(); // Auto-generate a unique ID
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; }
    public string Category { get; set; }
    public string Notes { get; set; }
}

// Represents the database model for storing transactions and categories
public class DatabaseModel
{
    public List<FinancialTransaction> Transactions { get; set; } = new List<FinancialTransaction>();
    public List<string> Categories { get; set; } = new List<string>();
}

public static class TransactionDatabase
{
    // Path to save the JSON file in the local app data folder
    private static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TransactionsDB.json");

    public static async Task<DatabaseModel> LoadDatabaseAsync()
    {
        if (File.Exists(FilePath))
        {
            var json = await File.ReadAllTextAsync(FilePath); 
            return JsonSerializer.Deserialize<DatabaseModel>(json) ?? new DatabaseModel(); // Deserialize JSON into a DebtDatabaseModel; fallback to a new model if null
        }
        return new DatabaseModel(); 
    }

    // Saves the current database state to the file as formatted JSON.
    public static async Task SaveDatabaseAsync(DatabaseModel database)
    {
        var json = JsonSerializer.Serialize(database, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(FilePath, json); // Write the serialized content to the file
    }
}

