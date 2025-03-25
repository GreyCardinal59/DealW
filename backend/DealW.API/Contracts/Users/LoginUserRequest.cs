using System.ComponentModel.DataAnnotations;

namespace DealW.Contracts.Users;

public record LoginUserRequest(
    [Required] string Email,
    [Required] string Password);