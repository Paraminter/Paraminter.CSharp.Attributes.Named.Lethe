namespace Paraminter.CSharp.Attributes.Named.Lethe;

using Moq;

using Paraminter.Arguments.CSharp.Attributes.Named.Models;
using Paraminter.Commands;
using Paraminter.Cqs.Handlers;
using Paraminter.Parameters.Named.Models;

using System;

using Xunit;

public sealed class Constructor
{
    [Fact]
    public void NullIndividualAssociator_ThrowsArgumentNullException()
    {
        var result = Record.Exception(() => Target(null!));

        Assert.IsType<ArgumentNullException>(result);
    }

    [Fact]
    public void ValidArguments_ReturnsAssociator()
    {
        var result = Target(Mock.Of<ICommandHandler<IAssociateSingleArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>>());

        Assert.NotNull(result);
    }

    private static CSharpAttributeNamedAssociator Target(
        ICommandHandler<IAssociateSingleArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>> individualAssociator)
    {
        return new CSharpAttributeNamedAssociator(individualAssociator);
    }
}
