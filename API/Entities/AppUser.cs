namespace API.Entities;

public sealed class AppUser
{
    //Table users
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string DisplayName { get; set; }
    public required string Email { get; set; }
}
