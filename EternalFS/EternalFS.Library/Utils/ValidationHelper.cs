using System;
using EternalFS.Library.Extensions;

namespace EternalFS.Library.Utils;

public static class ValidationHelper
{
    public static bool IsFilenameValid(in ReadOnlySpan<byte> filename)
    {
        if (filename.Length == 0)
            return false;

        if (filename.StartsWith(ByteSpanHelper.Space()) || filename.EndsWith(ByteSpanHelper.Space()))
            return false;

        return true;
    }

    public static bool IsDirectoryValid(in ReadOnlySpan<byte> directoryName)
    {
        if (directoryName.Length == 0)
            return false;

        if (directoryName.Contains(ByteSpanHelper.ForwardSlash()))
            return false;

        return true;
    }
}
