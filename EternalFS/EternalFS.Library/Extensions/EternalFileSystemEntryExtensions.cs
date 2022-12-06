using System;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Extensions;

public static class EternalFileSystemEntryExtensions
{
    public static bool IsCommonSubDirectory(this in EternalFileSystemEntry entry)
    {
        if (!entry.IsDirectory)
            return false;

        if (((ReadOnlySpan<byte>)entry.SubEntryName).TrimEndNull().SequenceEqual(ByteSpanHelper.Period()))
            return false;

        if (((ReadOnlySpan<byte>)entry.SubEntryName).TrimEndNull().SequenceEqual(ByteSpanHelper.ParentDirectory()))
            return false;

        return true;
    }
}
