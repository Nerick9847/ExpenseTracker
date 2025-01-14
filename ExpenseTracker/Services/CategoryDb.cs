using System.Text.Json;

public class CategoryDb
{
    private readonly string _categoriesFile = "wwwroot/data/categories.json";

    public async Task<List<string>> GetCategoriesAsync()
    {
        // Ensure the file exists
        if (!File.Exists(_categoriesFile))
        {
            await File.WriteAllTextAsync(_categoriesFile, "[]");
        }

        var json = await File.ReadAllTextAsync(_categoriesFile);
        return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
    }

    public async Task SaveCategoriesAsync(List<string> categories)
    {
        var json = JsonSerializer.Serialize(categories);
        await File.WriteAllTextAsync(_categoriesFile, json);
    }
}
