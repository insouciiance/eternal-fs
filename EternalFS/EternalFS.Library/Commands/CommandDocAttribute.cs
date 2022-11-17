using System;

namespace EternalFS.Library.Commands;

/// <summary>
/// Adds command documentation to <see cref="ICommand.Info"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public class CommandDocAttribute : Attribute
{
    public CommandDocAttribute(string summary) { }
}
