using System;
using System.Text;
using EternalFS.Library.Diagnostics;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Commands.Filesystem;

[Command("cat", true)]
[CommandSummary("Outputs the contents of a given file.")]
public partial class CatCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> filename = context.ValueSpan.SplitIndex();

        var fileEntry = context.Accessor.LocateSubEntry(new(context.CurrentDirectory.FatEntryReference, filename));
        
        if (fileEntry.IsDirectory)
            throw new EternalFileSystemException(EternalFileSystemState.CantOpenFile, filename.GetString());

        using EternalFileSystemFileStream stream = new(context.FileSystem, fileEntry.FatEntryReference);

        byte[] content = new byte[fileEntry.Size];
        stream.Read(content, 0, content.Length);
        string contentString = Encoding.UTF8.GetString(content);

        context.Writer.Append(contentString);

        return CommandExecutionResult.Default;
    }
}
