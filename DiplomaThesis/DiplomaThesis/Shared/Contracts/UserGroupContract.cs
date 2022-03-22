namespace DiplomaThesis.Shared.Contracts;

public class UserGroupContract
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public IEnumerable<Guid> Users { get; init; }
}