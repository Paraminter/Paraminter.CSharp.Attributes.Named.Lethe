namespace Paraminter.CSharp.Attributes.Named.Lethe;

using Xunit;

public sealed class Constructor
{
    [Fact]
    public void ReturnsAssociator()
    {
        var result = Target();

        Assert.NotNull(result);
    }

    private static SyntacticCSharpAttributeNamedAssociator Target() => new();
}
