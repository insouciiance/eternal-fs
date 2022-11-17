namespace EternalFS.Library.Commands;

/// <summary>
/// Represents documentation for <see cref="ICommand"/>.
/// </summary>
public class CommandDocumentation
{
    public string Summary { get; init; }

    public CommandDocumentation(string summary)
    {
        Summary = summary;
    }
}
