namespace DiplomaThesis.Shared;

public class ReportContract
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string EmbedUrl { get; init; } = null!;
    public string EmbedToken { get; init; } = null!;
    public Guid UserGroupId { get; init; }
}