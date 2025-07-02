using System.Text.Json;
using DealW.Domain.Models.Enums;

namespace DealW.Domain.Models;

public class Question
{
    public Guid Id { get; set; }
    
    public string Text { get; set; }
    
    public QuestionType QuestionType { get; set; } = QuestionType.TextInput;
    
    public string CorrectAnswer { get; set; }
    
    public string? Explanation { get; set; }
    
    public string? Options { get; set; }
    
    public string? BlanksData { get; set; }
    
    public int Difficulty { get; set; } = 1;
    
    public string Category { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public List<DuelQuestion> DuelQuestions { get; set; } // ICollection
    
    public List<string>? GetOptions()
    {
        if (string.IsNullOrEmpty(Options))
            return null;
                
        var optionsData = JsonSerializer.Deserialize<JsonElement>(Options);
        return optionsData.GetProperty("options").EnumerateArray()
            .Select(o => o.GetString())
            .ToList();
    }
    
    public int GetCorrectOptionIndex()
    {
        if (string.IsNullOrEmpty(Options))
            return -1;
                
        var optionsData = JsonSerializer.Deserialize<JsonElement>(Options);
        return optionsData.TryGetProperty("correctIndex", out var indexElement) 
            ? indexElement.GetInt32() 
            : -1;
    }
    
    public string? GetBlanksTemplate()
    {
        if (string.IsNullOrEmpty(BlanksData))
            return null;
                
        var blanksData = JsonSerializer.Deserialize<JsonElement>(BlanksData);
        return blanksData.TryGetProperty("template", out var templateElement)
            ? templateElement.GetString()
            : null;
    }
    
    public List<string>? GetBlankValues()
    {
        if (string.IsNullOrEmpty(BlanksData))
            return null;
                
        var blanksData = JsonSerializer.Deserialize<JsonElement>(BlanksData);
        return blanksData.TryGetProperty("blanks", out var blanksElement)
            ? blanksElement.EnumerateArray()
                .Select(b => b.GetString())
                .ToList()
            : null;
    }
}

