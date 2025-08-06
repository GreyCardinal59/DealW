using DealW.Domain.Models;

namespace DealW.Infrastructure.Seeders;

public class Seeder(ApplicationDbContext context)
{
    public void Seed()
    {
        SeedUsers();
        SeedQuestions();
        SeedDuels();
    }
    
    private void SeedUsers()
    {
        // if (context.Users.Any())
        //     return;
        //
        // var users = new List<User>
        // {
        //     new(),
        //     new()
        // };
        //
        // context.Users.AddRange(users);
        // context.SaveChanges();
    }
    
    private void SeedQuestions()
    {
        // context.Questions.AddRange(questions);
        // context.SaveChanges();
    }
    
    private void SeedDuels()
    {
        // context.Duels.AddRange(duels);
        // context.SaveChanges();
    }
}