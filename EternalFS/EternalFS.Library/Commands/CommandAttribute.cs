using System;

namespace EternalFS.Library.Commands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public class CommandAttribute : Attribute
{
    public CommandAttribute(string name, bool needsFileSystem = false) { }
}
