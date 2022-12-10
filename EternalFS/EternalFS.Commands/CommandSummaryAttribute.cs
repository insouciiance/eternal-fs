using System;

namespace EternalFS.Commands;

/// <summary>
/// Adds command documentation to <see cref="ICommand.Info"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public class CommandSummaryAttribute : Attribute
{
    public CommandSummaryAttribute(string summary) { }
}
