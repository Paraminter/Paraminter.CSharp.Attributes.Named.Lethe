namespace Paraminter.Associating.CSharp.Attributes.Named.Lethe;

using Moq;

using Paraminter.Arguments.CSharp.Attributes.Named.Models;
using Paraminter.Cqs;
using Paraminter.Pairing.Commands;
using Paraminter.Parameters.Named.Models;

using System;

using Xunit;

public sealed class Constructor
{
    [Fact]
    public void NullPairer_ThrowsArgumentNullException()
    {
        var result = Record.Exception(() => Target(null!));

        Assert.IsType<ArgumentNullException>(result);
    }

    [Fact]
    public void ValidArguments_ReturnsAssociator()
    {
        var result = Target(Mock.Of<ICommandHandler<IPairArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>>());

        Assert.NotNull(result);
    }

    private static CSharpAttributeNamedAssociator Target(
        ICommandHandler<IPairArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>> pairer)
    {
        return new CSharpAttributeNamedAssociator(pairer);
    }
}
