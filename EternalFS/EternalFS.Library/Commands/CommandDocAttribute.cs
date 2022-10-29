using System;

namespace EternalFS.Library.Commands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public class CommandDocAttribute : Attribute
{
    public CommandDocAttribute(string summary) { }
}
