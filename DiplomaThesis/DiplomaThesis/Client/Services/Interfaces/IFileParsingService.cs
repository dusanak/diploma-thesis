namespace DiplomaThesis.Client.Services.Interfaces;

public interface IFileParsingService
{
    public string ParseToJson(string datasetFile, string extension);
}