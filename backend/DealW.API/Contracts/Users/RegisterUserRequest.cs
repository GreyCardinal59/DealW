using System.ComponentModel.DataAnnotations;

namespace DealW.Contracts.Users;

public record RegisterUserRequest(
    [Required] string UserName,
    [Required] string Password,
    [Required] string Email);