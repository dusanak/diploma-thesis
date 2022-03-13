namespace DiplomaThesis.Server.Models;

public class UserGroup
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<ApplicationUser> Users { get; set; }
}