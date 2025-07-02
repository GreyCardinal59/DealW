namespace DealW.Domain.Models.Enums;

/// <summary>
/// Типы вопросов в дуэли
/// </summary>
public enum QuestionType
{
    /// <summary>
    /// Вопрос с вводом текста
    /// </summary>
    TextInput = 1,
        
    /// <summary>
    /// Вопрос с выбором вариантов ответа
    /// </summary>
    MultipleChoice = 2,
        
    /// <summary>
    /// Вопрос с заполнением пропусков
    /// </summary>
    FillBlanks = 3
}