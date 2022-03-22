namespace DiplomaThesis.Shared.Contracts;

public class DashboardContract
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string EmbedUrl { get; init; } = null!;
}