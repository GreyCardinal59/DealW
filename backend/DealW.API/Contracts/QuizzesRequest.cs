namespace DealW.Contracts;

public record QuizzesRequest(
    string title,
    IList<string> questions);