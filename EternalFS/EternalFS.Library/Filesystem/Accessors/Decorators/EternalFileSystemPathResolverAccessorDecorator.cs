using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

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
        EternalFileSystemSubEntryEnumerator enumerator = new(info.Name);

        EternalFileSystemFatEntry currentFatEntry = info.FatEntry;

        while (enumerator.MoveNext())
        {
            if (info.Name.EndsWith(enumerator.Current))
                return new(currentFatEntry, enumerator.Current);

            var currentEntry = Accessor.LocateSubEntry(new(currentFatEntry, enumerator.Current));
            currentFatEntry = currentEntry.FatEntryReference;
        }

        throw new UnreachableException();
    }
}
