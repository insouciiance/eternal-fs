using System;

namespace EternalFS.Library.Commands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class CommandDocAttribute : Attribute
{
    public CommandDocAttribute(string summary) { }
}
