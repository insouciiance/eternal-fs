using System;

namespace EternalFS.Library.Commands;

/// <summary>
/// Indicates a commmand type, i.e. the type that implements <see cref="ICommand"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public class CommandAttribute : Attribute
{
    public CommandAttribute(string name, bool needsFileSystem = false) { }
}
