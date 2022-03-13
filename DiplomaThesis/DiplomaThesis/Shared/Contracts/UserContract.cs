namespace DiplomaThesis.Shared.Contracts;

public class UserContract
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public RoleContract[] Roles { get; init; }
}