using System;

namespace EternalFS.Library.Commands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class CommandDocAttribute : Attribute
{
    public string Summary { get; }

    public CommandDocAttribute(string summary)
    {
        Summary = summary;
    }
}
