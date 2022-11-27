using System;

namespace EternalFS.Library.Commands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
public class CommandArgumentAttribute : Attribute
{
    public CommandArgumentAttribute(string name, string description, bool required = false) { }
}
