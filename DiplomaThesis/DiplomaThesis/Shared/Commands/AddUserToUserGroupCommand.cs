namespace DiplomaThesis.Shared.Commands;

public class AddUserToUserGroupCommand
{
    public Guid UserGroupId { get; init; }
    public Guid UserId { get; init; }
}