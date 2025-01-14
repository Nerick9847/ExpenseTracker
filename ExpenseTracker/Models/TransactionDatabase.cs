using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

public class FinancialTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; }
    public string Category { get; set; }
    public string Notes { get; set; }
}

public class DatabaseModel
{
    public List<FinancialTransaction> Transactions { get; set; } = new List<FinancialTransaction>();
    public List<string> Categories { get; set; } = new List<string>();
}

public static class TransactionDatabase
{
    private static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TransactionsDB.json");

    public static async Task<DatabaseModel> LoadDatabaseAsync()
    {
        if (File.Exists(FilePath))
        {
            var json = await File.ReadAllTextAsync(FilePath);
            return JsonSerializer.Deserialize<DatabaseModel>(json) ?? new DatabaseModel();
        }
        return new DatabaseModel();
    }

    public static async Task SaveDatabaseAsync(DatabaseModel database)
    {
        var json = JsonSerializer.Serialize(database, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(FilePath, json);
    }
}
