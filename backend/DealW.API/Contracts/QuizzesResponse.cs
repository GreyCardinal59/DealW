namespace DealW.Contracts;

public record QuizzesResponse(
    Guid id,
    string title,
    IList<string> questions);