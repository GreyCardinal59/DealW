namespace DealW.Domain.Models;

public class User
{
    public int Id { get; set; }
    
    public string Username { get; set; }
    
    public string Email { get; set; }
    
    public string PasswordHash { get; set; }
        
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        
    public int TotalWins { get; set; } = 0;
        
    public int TotalLosses { get; set; } = 0;
        
    public int Rating { get; set; } = 1000;
        
    public bool IsSearchingOpponent { get; set; } = false;
        
    // Навигационное свойство для дуэлей, где пользователь был первым участником
    public ICollection<Duel> DuelsAsFirstUser { get; set; }
        
    // Навигационное свойство для дуэлей, где пользователь был вторым участником
    public ICollection<Duel> DuelsAsSecondUser { get; set; }
}