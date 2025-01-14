using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExpenseTracker.Services
{
    public class JsonDbHandler<T>
    {
        private readonly string _filePath;

        public JsonDbHandler(string filePath)
        {
            _filePath = filePath;
        }

        public async Task<List<T>> ReadAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new List<T>();
            }

            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        public async Task WriteAsync(List<T> data)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }
    }
}
