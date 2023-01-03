using System.Text;
using EternalFS.Library.Extensions;
using EternalFS.Library.Utils;

namespace EternalFS.Library.Tests.Utils;

public partial class ByteSpanTests
{
    [ByteSpan("foo")]
    private partial ReadOnlySpan<byte> SimpleSpan();

    [ByteSpan("baz")]
    private static partial ReadOnlySpan<byte> StaticSpan();

    [Fact]
    public void Simple()
    {
        Assert.Equal("foo", SimpleSpan().GetString());
    }

    [Fact]
    public void Static()
    {
        Assert.Equal("baz", StaticSpan().GetString());
    }
}
