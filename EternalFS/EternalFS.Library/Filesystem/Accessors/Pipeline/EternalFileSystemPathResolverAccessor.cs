﻿using System;
using System.IO;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Filesystem.Accessors.Pipeline;

public class EternalFileSystemPathResolverAccessor : AccessorPipelineElement
{
    public override EternalFileSystemEntry LocateSubEntry(in SubEntryInfo info)
    {
        var traversed = TraversePath(info);
        return base.LocateSubEntry(traversed);
    }

    public override EternalFileSystemEntry CreateSubEntry(in SubEntryInfo info, bool isDirectory)
    {
        var traversed = TraversePath(info);
        return base.CreateSubEntry(traversed, isDirectory);
    }

    public override void DeleteSubEntry(in SubEntryInfo info)
    {
        var traversed = TraversePath(info);
        base.DeleteSubEntry(traversed);
    }

    public override void WriteFile(in SubEntryInfo info, Stream source, bool append = false)
    {
        var traversed = TraversePath(info);
        base.WriteFile(traversed, source, append);
    }

    public override void CopySubEntry(in SubEntryInfo from, in SubEntryInfo to)
    {
        var fromTraversed = TraversePath(from);
        var toTraversed = TraversePath(to);
        base.CopySubEntry(fromTraversed, toTraversed);
    }

    private SubEntryInfo TraversePath(in SubEntryInfo info)
    {
        // short path for root entry
        if (info.Name.SequenceEqual(ByteSpanHelper.ForwardSlash()))
            return new SubEntryInfo(EternalFileSystemMounter.RootDirectoryEntry, ByteSpanHelper.Period());

        EternalFileSystemSubEntryEnumerator enumerator = new(info.Name);

        enumerator.MoveNext();

        ReadOnlySpan<byte> current = enumerator.Current;
        EternalFileSystemFatEntry currentFatEntry = info.FatEntry;

        while (enumerator.MoveNext())
        {
            var currentEntry = base.LocateSubEntry(new(currentFatEntry, current));
            currentFatEntry = currentEntry.FatEntryReference;
            current = enumerator.Current;
        }

        return new SubEntryInfo(currentFatEntry, current);
    }
}
