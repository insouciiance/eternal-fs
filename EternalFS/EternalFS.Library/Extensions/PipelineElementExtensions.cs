using System;
using System.Diagnostics.CodeAnalysis;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Extensions;

public static class PipelineElementExtensions
{
    public static bool Insert<TElement>(this TElement element, TElement value, Func<TElement, bool> predicate)
        where TElement : IPipelineElement<TElement>
    {
        TElement current = element;

        while (!predicate.Invoke(current))
        {
            if (current.Next is null)
                return false;

            current = current.Next;
        }

        current.SetNext(value);
        value.SetNext(current.Next);

        return true;
    }

    public static bool Remove<TElement>(this TElement element, Func<TElement, bool> predicate)
        where TElement : IPipelineElement<TElement>
    {
        TElement? previous = default;
        TElement current = element;

        while (!predicate.Invoke(current))
        {
            if (current.Next is null)
                return false;

            previous = current;
            current = current.Next;
        }

        previous?.SetNext(current.Next);

        return true;
    }

    public static bool TryFind<TElement>(this TElement element, Func<TElement, bool> predicate, [MaybeNullWhen(false)] out TElement result)
        where TElement : IPipelineElement<TElement>
    {
        TElement? current = element;

        while (current is not null)
        {
            if (predicate.Invoke(current))
            {
                result = current;
                return true;
            }

            current = current.Next;
        }

        result = default;
        return false;
    }
}
