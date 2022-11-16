﻿using System;
using System.Collections.Generic;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Commands.Filesystem;

[Command("cd", true)]
[CommandDoc("Changes the current working directory.")]
public partial class CdCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> directoryName = context.ValueSpan.SplitIndex();

        List<string> stack = context.CurrentDirectory;

        if (directoryName.SequenceEqual(ByteSpanHelper.ParentDirectory()))
            stack.RemoveAt(context.CurrentDirectory.Count - 1);
        else
            stack.Add(directoryName.GetString());

        try
        {
            context.Accessor.LocateDirectory(stack);
        }
        catch(EternalFileSystemException)
        {
            stack.RemoveAt(stack.Count - 1);
            throw;
        }

        return CommandExecutionResult.Default;
    }
}
