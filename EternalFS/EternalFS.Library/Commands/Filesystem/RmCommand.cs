﻿using System;
using EternalFS.Library.Extensions;

namespace EternalFS.Library.Commands.Filesystem;

[Command("rm", true)]
[CommandDoc("Deletes a specified file.")]
public partial class RmCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> fileName = context.ValueSpan.SplitIndex();
        context.Accessor.DeleteSubEntry(context.CurrentDirectory.FatEntryReference, fileName);

        return CommandExecutionResult.Default;
    }
}
