﻿namespace EternalFS.Library.Commands;

public class CommandInfo
{
    public string Name { get; init; }

    public bool NeedsFileSystem { get; init; }
    
    public CommandDocumentation? Documentation { get; init; }

    public CommandInfo(string name, bool needsFileSystem, CommandDocumentation? documentation = null)
    {
        Name = name;
        NeedsFileSystem = needsFileSystem;
        Documentation = documentation;
    }
}
