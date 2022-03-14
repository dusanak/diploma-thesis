namespace DiplomaThesis.Shared.Contracts;

public class UserContract
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public Guid UserGroupId { get; init; }
    public IEnumerable<RoleContract> Roles { get; init; }
}