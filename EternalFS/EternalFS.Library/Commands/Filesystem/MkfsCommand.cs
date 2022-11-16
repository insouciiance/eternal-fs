using System;
using EternalFS.Library.Diagnostics;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Filesystem.Initializers;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Commands.Filesystem;

[Command("mkfs")]
[CommandDoc("Creates a file system given its name and size.")]
public partial class MkfsCommand
{
    [ByteSpan("-n")]
    private static partial ReadOnlySpan<byte> Name();

    [ByteSpan("-s")]
    private static partial ReadOnlySpan<byte> Size();

    [ByteSpan("-f")]
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
        context.CurrentDirectory.Add(EternalFileSystemMounter.ROOT_DIRECTORY_NAME);

        EternalFileSystemMounter.Mount(initializer);
        context.FileSystem = initializer.CreateFileSystem();
        context.Accessor.Initialize(context.FileSystem);

        return CommandExecutionResult.Default;
    }
}
