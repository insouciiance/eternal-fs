using System;
using System.Text;
using EternalFS.Library.Commands;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Filesystem.Accessors;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Commands.Filesystem;

[Command("cat", true)]
[CommandDoc("Outputs the contents of a given file.")]
public partial class CatCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        ReadOnlySpan<byte> filename = context.ValueSpan.SplitIndex();

        if (!ValidationHelper.IsFilenameValid(filename))
            throw new EternalFileSystemException(EternalFileSystemState.InvalidFilename, filename.GetString());

        var directoryEntry = context.Accessor.LocateDirectory(context.CurrentDirectory);
        var fileEntry = context.Accessor.LocateSubEntry(directoryEntry.FatEntryReference, filename);
        
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
