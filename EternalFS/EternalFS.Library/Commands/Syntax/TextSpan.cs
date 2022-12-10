using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace EternalFS.Library.Commands.Syntax;

public readonly struct TextSpan
{
    public static readonly TextSpan Empty = new(int.MaxValue, int.MinValue);

    public readonly int Start;

    public readonly int End;
    
    public int Length => End - Start + 1;

    public TextSpan(int start, int end)
    {
        Start = start;
        End = end;
    }

    public static bool operator ==(TextSpan lhs, TextSpan rhs)
    {
        return lhs.Start == rhs.Start && lhs.End == rhs.End;
    }

    public static bool operator !=(TextSpan lhs, TextSpan rhs)
    {
        return !(lhs == rhs);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null)
            return false;

        TextSpan other = Unsafe.Unbox<TextSpan>(obj);
        return this == other;
    }

    public override int GetHashCode()
    {
        return Start ^ End;
    }
}
