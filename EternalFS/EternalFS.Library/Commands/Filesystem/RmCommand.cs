﻿using System;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;

namespace EternalFS.Library.Commands.Filesystem;

[Command("rm")]
[CommandDoc("Deletes a specified file.")]
public partial class RmCommand
{
    // TODO: handle deletion correctly.
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> fileName = context.ValueSpan.SplitIndex();

        EternalFileSystemManager manager = new(context.FileSystem);
        EternalFileSystemFatEntry directoryEntry = manager.OpenDirectory(context.CurrentDirectory);
        manager.DeleteFile(fileName, directoryEntry);

        return new();
    }
}
