using EternalFS.Library.Utils;

namespace EternalFS.Library.Tests.Utils;

public class StringMapTests
{
    [Fact]
    public void Simple()
    {
        Assert.Equal("Test Foo", SimpleEnumMap.GetString(SimpleEnum.Foo));
        Assert.Equal("Test Baz", SimpleEnumMap.GetString(SimpleEnum.Baz));
        Assert.False(SimpleEnumMap.TryGetString((SimpleEnum)3, out _));
    }
}

[StringMap]
public enum SimpleEnum
{
    [Map("Test Foo")]
    Foo,

    [Map("Test Baz")]
    Baz
}
