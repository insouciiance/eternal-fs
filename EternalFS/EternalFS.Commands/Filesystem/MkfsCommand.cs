using System;
using EternalFS.Commands.Diagnostics;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Filesystem.Initializers;
using EternalFS.Library.Utils;

namespace EternalFS.Commands.Filesystem;

[Command("mkfs")]
[CommandSummary("Creates a file system given its name and size.")]
[CommandArgument(NAME_ARG, "The name of the file system to create.", true)]
[CommandArgument(SIZE_ARG, "The size of the file system to create (in bytes).", true)]
[CommandArgument(FILENAME_ARG, "The filename of the file system to create. If omitted, the filesystem fill be an in-memory filesystem.")]
public partial class MkfsCommand
{
    private const string NAME_ARG = "-n";

    private const string SIZE_ARG = "-s";

    private const string FILENAME_ARG = "-f";

    [ByteSpan(NAME_ARG)]
    private static partial ReadOnlySpan<byte> Name();

    [ByteSpan(SIZE_ARG)]
    private static partial ReadOnlySpan<byte> Size();

    [ByteSpan(FILENAME_ARG)]
    private static partial ReadOnlySpan<byte> FileName();

    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        if (!ArgumentsHelper.TryGetArgumentValue(context.ValueSpan, Name(), out var nameSpan))
            throw new CommandExecutionException(CommandExecutionState.Other);

        if (!ArgumentsHelper.TryGetArgumentValue(context.ValueSpan, Size(), out var sizeSpan))
            throw new CommandExecutionException(CommandExecutionState.Other);

        string name = nameSpan.GetString();
        int size = int.Parse(sizeSpan.GetString());

        IEternalFileSystemInitializer<EternalFileSystem> initializer;

        if (ArgumentsHelper.TryGetArgumentValue(context.ValueSpan, FileName(), out var fileName))
            initializer = new DiskEternalFileSystemInitializer(name, size, fileName.GetString());
        else
            initializer = new VirtualEternalFileSystemInitializer(name, size);

        context.CurrentDirectory.Clear();

        EternalFileSystemMounter.Mount(initializer);
        context.FileSystem = initializer.CreateFileSystem();
        context.Accessor.Initialize(context.FileSystem);

        return CommandExecutionResult.Default;
    }
}
