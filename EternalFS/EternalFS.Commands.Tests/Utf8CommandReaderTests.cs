using System;
using System.Linq;
using System.Text;
using EternalFS.Library.Extensions;

namespace EternalFS.Commands.Tests;

public class Utf8CommandReaderTests
{
    [Fact]
    public void Simple()
    {
        string text = "test";
        Utf8CommandReader reader = new(GetBytes(text));

        Assert.True("test"u8.SequenceEqual(reader.ReadCommandName()));
        Assert.True(reader.IsFullyRead);
    }

    [Fact]
    public void NamedArguments()
    {
        string text = "test -n=name -f=file";
        Utf8CommandReader reader = new(GetBytes(text));

        Assert.True("test"u8.SequenceEqual(reader.ReadCommandName()));
        Assert.False(reader.IsFullyRead);

        Assert.False(reader.TryReadPositionalArgument(out _));

        Assert.True(reader.TryReadNamedArgument(GetBytes("-n"), out var name));
        Assert.Equal("name", name.Value.GetString());
        Assert.False(reader.IsFullyRead);
        
        Assert.True(reader.TryReadNamedArgument(GetBytes("-f"), out var file));
        Assert.Equal("file", file.Value.GetString());
        Assert.True(reader.IsFullyRead);
    }

    [Fact]
    public void PositionalArguments()
    {
        string text = "test file1 file2 file3";
        Utf8CommandReader reader = new(GetBytes(text));

        Assert.True("test"u8.SequenceEqual(reader.ReadCommandName()));
        Assert.False(reader.IsFullyRead);

        Assert.True(reader.TryReadPositionalArgument(out var arg1));
        Assert.Equal("file1", arg1.GetString());
        Assert.False(reader.IsFullyRead);

        Assert.True(reader.TryReadPositionalArgument(out var arg2));
        Assert.Equal("file2", arg2.GetString());
        Assert.False(reader.IsFullyRead);

        Assert.True(reader.TryReadPositionalArgument(out var arg3));
        Assert.Equal("file3", arg3.GetString());
        Assert.True(reader.IsFullyRead);
    }

    [Fact]
    public void MixedArguments()
    {
        string text = "test file1 file2 file3 -f1=file4 -f2=file5";
        Utf8CommandReader reader = new(GetBytes(text));

        Assert.True("test"u8.SequenceEqual(reader.ReadCommandName()));
        Assert.False(reader.IsFullyRead);

        Assert.True(reader.TryReadPositionalArgument(out var arg1));
        Assert.Equal("file1", arg1.GetString());
        Assert.False(reader.IsFullyRead);

        Assert.True(reader.TryReadPositionalArgument(out var arg2));
        Assert.Equal("file2", arg2.GetString());
        Assert.False(reader.IsFullyRead);

        Assert.True(reader.TryReadPositionalArgument(out var arg3));
        Assert.Equal("file3", arg3.GetString());
        Assert.False(reader.IsFullyRead);

        Assert.False(reader.TryReadPositionalArgument(out _));

        Assert.True(reader.TryReadNamedArgument(GetBytes("-f1"), out var name));
        Assert.Equal("file4", name.Value.GetString());
        Assert.False(reader.IsFullyRead);

        Assert.True(reader.TryReadNamedArgument(GetBytes("-f2"), out var file));
        Assert.Equal("file5", file.Value.GetString());
        Assert.True(reader.IsFullyRead);
    }

    [Fact]
    public void NamedArguments_ReverseOrder()
    {
        string text = "test -n=name -f=file";
        Utf8CommandReader reader = new(GetBytes(text));

        Assert.True("test"u8.SequenceEqual(reader.ReadCommandName()));
        Assert.False(reader.IsFullyRead);

        Assert.False(reader.TryReadPositionalArgument(out _));

        Assert.True(reader.TryReadNamedArgument(GetBytes("-f"), out var name));
        Assert.Equal("file", name.Value.GetString());
        Assert.False(reader.IsFullyRead);

        Assert.True(reader.TryReadNamedArgument(GetBytes("-n"), out var file));
        Assert.Equal("name", file.Value.GetString());
        Assert.True(reader.IsFullyRead);
    }

    private static ReadOnlySpan<byte> GetBytes(string text)
    {
        return Encoding.UTF8.GetBytes(text);
    }
}
