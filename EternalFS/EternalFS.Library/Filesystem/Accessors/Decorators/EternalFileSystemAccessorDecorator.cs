using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace EternalFS.Library.Filesystem.Accessors.Decorators;

public abstract class EternalFileSystemAccessorDecorator : IEternalFileSystemAccessor
{
    public required IEternalFileSystemAccessor Accessor { get; init; }

    public event EventHandler<EntryLocatedEventArgs>? EntryLocated
    {
        add => Accessor.EntryLocated += value;
        remove => Accessor.EntryLocated -= value;
    }
    
    [SetsRequiredMembers]
    protected EternalFileSystemAccessorDecorator(IEternalFileSystemAccessor accessor) => Accessor = accessor;

    public virtual void Initialize(EternalFileSystem fileSystem)
        => Accessor.Initialize(fileSystem);

    public virtual EternalFileSystemEntry LocateSubEntry(in SubEntryInfo info)
        => Accessor.LocateSubEntry(info);

    public virtual EternalFileSystemEntry CreateSubEntry(in SubEntryInfo info, bool isDirectory)
        => Accessor.CreateSubEntry(info, isDirectory);

    public virtual void DeleteSubEntry(in SubEntryInfo info)
        => Accessor.DeleteSubEntry(info);

    public virtual void CopySubEntry(in SubEntryInfo from, in SubEntryInfo to)
        => Accessor.CopySubEntry(from, to);

    public virtual void WriteFile(in SubEntryInfo info, Stream source)
        => Accessor.WriteFile(info, source);
}
