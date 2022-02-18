namespace DiplomaThesis.Shared.Models;

public class User
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public Role[] Roles { get; init; }
}