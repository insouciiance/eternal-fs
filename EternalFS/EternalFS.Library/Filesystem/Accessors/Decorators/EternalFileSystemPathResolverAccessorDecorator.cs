using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Filesystem.Accessors.Decorators;

public class EternalFileSystemPathResolverAccessorDecorator : EternalFileSystemAccessorDecorator
{
    [SetsRequiredMembers]
    public EternalFileSystemPathResolverAccessorDecorator(IEternalFileSystemAccessor accessor)
        : base(accessor) { }

    public override EternalFileSystemEntry LocateSubEntry(in SubEntryInfo info)
    {
        var traversed = TraversePath(info);
        return Accessor.LocateSubEntry(traversed);
    }

    public override EternalFileSystemEntry CreateSubEntry(in SubEntryInfo info, bool isDirectory)
    {
        var traversed = TraversePath(info);
        return Accessor.CreateSubEntry(traversed, isDirectory);
    }

    public override void DeleteSubEntry(in SubEntryInfo info)
    {
        var traversed = TraversePath(info);
        Accessor.DeleteSubEntry(traversed);
    }

    public override void WriteFile(in SubEntryInfo info, Stream source)
    {
        var traversed = TraversePath(info);
        Accessor.WriteFile(traversed, source);
    }

    public override void CopySubEntry(in SubEntryInfo from, in SubEntryInfo to)
    {
        var fromTraversed = TraversePath(from);
        var toTraversed = TraversePath(to);
        Accessor.CopySubEntry(fromTraversed, toTraversed);
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
            var currentEntry = Accessor.LocateSubEntry(new(currentFatEntry, current));
            currentFatEntry = currentEntry.FatEntryReference;
            current = enumerator.Current;
        }

        return new SubEntryInfo(currentFatEntry, current);
    }
}
