namespace EternalFS.Library.Commands;

public class CommandDocumentation
{
    public string Summary { get; init; }

    public CommandDocumentation(string summary)
    {
        Summary = summary;
    }
}
