namespace DealW.Contracts;

public record QuestionResponse(int Id, int QuizId, string Text, int CorrectAnswerId);