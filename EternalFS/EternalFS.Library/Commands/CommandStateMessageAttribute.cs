using System;

namespace EternalFS.Library.Commands;

[AttributeUsage(AttributeTargets.Field)]
public class CommandStateMessageAttribute : Attribute
{
    public CommandStateMessageAttribute(string message) { }
}
