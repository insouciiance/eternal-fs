using System;
using EternalFS.Library.Diagnostics;
using EternalFS.Library.Extensions;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Filesystem.Validation;

public class DefaultEternalFileSystemValidator : IEternalFileSystemValidator
{
    public void ValidateFileEntry(in ReadOnlySpan<byte> entry)
    {
        ValidateCommon(entry);

        if (entry.EndsWith(ByteSpanHelper.Period()))
            throw new EternalFileSystemValidationException(EternalFileSystemValidationState.FileTrailingDot);
    }

    public void ValidateDirectoryEntry(in ReadOnlySpan<byte> entry)
    {
        ValidateCommon(entry);
    }

    private static void ValidateCommon(in ReadOnlySpan<byte> entry)
    {
        if (entry.Length == 0)
            throw new EternalFileSystemValidationException(EternalFileSystemValidationState.NameEmpty);

        if (entry.StartsWith(ByteSpanHelper.Space()) || entry.EndsWith(ByteSpanHelper.Space()))
            throw new EternalFileSystemValidationException(EternalFileSystemValidationState.LeadingOrTrailingSpace);

        CheckWithout(entry, ByteSpanHelper.ForwardSlash());
        CheckWithout(entry, ByteSpanHelper.BackSlash());
        CheckWithout(entry, ByteSpanHelper.Ampersand());
        CheckWithout(entry, ByteSpanHelper.Asterisk());
        CheckWithout(entry, ByteSpanHelper.At());
        CheckWithout(entry, ByteSpanHelper.Colon());
        CheckWithout(entry, ByteSpanHelper.Dollar());
        CheckWithout(entry, ByteSpanHelper.DoubleQuote());
        CheckWithout(entry, ByteSpanHelper.Equals());
        CheckWithout(entry, ByteSpanHelper.QuestionMark());
        CheckWithout(entry, ByteSpanHelper.ExclamationMark());
        CheckWithout(entry, ByteSpanHelper.Hash());
        CheckWithout(entry, ByteSpanHelper.Percent());
        CheckWithout(entry, ByteSpanHelper.Pipe());
        CheckWithout(entry, ByteSpanHelper.Plus());
        CheckWithout(entry, ByteSpanHelper.LeftAngleBracket());
        CheckWithout(entry, ByteSpanHelper.RightAngleBracket());
        CheckWithout(entry, ByteSpanHelper.LeftCurlyBracket());
        CheckWithout(entry, ByteSpanHelper.RightCurlyBracket());
        CheckWithout(entry, ByteSpanHelper.SingleQuote());
    }

    private static void CheckWithout(in ReadOnlySpan<byte> entry, in ReadOnlySpan<byte> character)
    {
        if (entry.Contains(character))
            throw new EternalFileSystemValidationException(EternalFileSystemValidationState.ForbiddenCharacter, character.GetString());
    }
}
