using System;
using System.Collections.Immutable;

namespace EternalFS.Library.Commands;

/// <summary>
/// Represents documentation for <see cref="ICommand"/>.
/// </summary>
public class CommandDocumentation
{
    public string Summary { get; init; }

    public ImmutableArray<Argument> Arguments { get; init; }

    private CommandDocumentation(string summary, ImmutableArray<Argument> arguments)
    {
        Summary = summary;
        Arguments = arguments;
    }

    public static Builder CreateBuilder() => new();

    public class Builder
    {
        private string? _summary;

        private readonly ImmutableArray<Argument>.Builder _argsBuilder;

        public Builder()
        {
            _argsBuilder = ImmutableArray.CreateBuilder<Argument>();
        }

        public Builder SetSummary(string summary)
        {
            _summary = summary;
            return this;
        }

        public Builder AddArgument(Argument argument)
        {
            _argsBuilder.Add(argument);
            return this;
        }

        public CommandDocumentation ToDocumentation()
        {
            if (_summary is null)
                throw new InvalidOperationException("Cannot create CommandDocumentation with empty summary.");

            return new CommandDocumentation(_summary, _argsBuilder.ToImmutable());
        }
    }

    public readonly struct Argument
    {
        public readonly string Name;

        public readonly string Description;

        public readonly bool Required;

        public Argument(string name, string description, bool required)
        {
            Name = name;
            Description = description;
            Required = required;
        }
    }
}
