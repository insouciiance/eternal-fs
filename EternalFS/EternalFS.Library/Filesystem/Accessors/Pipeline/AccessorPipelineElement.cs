using System;
using System.IO;
using EternalFS.Library.Diagnostics;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Filesystem.Accessors.Pipeline;

public class AccessorPipelineElement : IEternalFileSystemAccessor, IPipelineElement<AccessorPipelineElement>
{
    public AccessorPipelineElement? Next { get; private set; }

    public virtual event EventHandler<EntryLocatedEventArgs>? EntryLocated
    {
        add
        {
            if (Next is not null)
                Next.EntryLocated += value;
        }
        remove
        {
            if (Next is not null)
                Next.EntryLocated -= value;
        }
    }

    public void SetNext(AccessorPipelineElement? next)
    {
        Next = next;
    }

    public virtual void Initialize(EternalFileSystem fileSystem)
    {
        Next?.Initialize(fileSystem);
    }

    public virtual EternalFileSystemEntry LocateSubEntry(in SubEntryInfo info)
    {
        if (Next is null)
            throw new EternalFileSystemException(EternalFileSystemState.Other);

        return Next.LocateSubEntry(info);
    }

    public virtual EternalFileSystemEntry CreateSubEntry(in SubEntryInfo info, bool isDirectory)
    {
        if (Next is null)
            throw new EternalFileSystemException(EternalFileSystemState.Other);

        return Next.CreateSubEntry(info, isDirectory);
    }

    public virtual void DeleteSubEntry(in SubEntryInfo info)
    {
        if (Next is null)
            throw new EternalFileSystemException(EternalFileSystemState.Other);

        Next.DeleteSubEntry(info);
    }

    public virtual void CopySubEntry(in SubEntryInfo from, in SubEntryInfo to)
    {
        if (Next is null)
            throw new EternalFileSystemException(EternalFileSystemState.Other);

        Next.CopySubEntry(from, to);
    }

    public virtual void WriteFile(in SubEntryInfo info, Stream source)
    {
        if (Next is null)
            throw new EternalFileSystemException(EternalFileSystemState.Other);

        Next.WriteFile(info, source);
    }
}
