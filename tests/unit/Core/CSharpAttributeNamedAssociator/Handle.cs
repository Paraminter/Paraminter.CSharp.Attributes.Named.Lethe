namespace Paraminter.Associating.CSharp.Attributes.Named.Lethe;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using Paraminter.Arguments.CSharp.Attributes.Named.Models;
using Paraminter.Associating.Commands;
using Paraminter.Associating.CSharp.Attributes.Named.Lethe.Models;
using Paraminter.Cqs;
using Paraminter.Pairing.Commands;
using Paraminter.Parameters.Named.Models;

using System;
using System.Linq.Expressions;

using Xunit;

public sealed class Handle
{
    private readonly IFixture Fixture = FixtureFactory.Create();

    [Fact]
    public void NullCommand_ThrowsArgumentNullException()
    {
        var result = Record.Exception(() => Target(null!));

        Assert.IsType<ArgumentNullException>(result);
    }

    [Fact]
    public void NoSyntacticArguments_PairsNone()
    {
        Mock<IAssociateArgumentsCommand<IAssociateCSharpAttributeNamedArgumentsData>> commandMock = new();

        commandMock.Setup(static (command) => command.Data.SyntacticArguments).Returns([]);

        Target(commandMock.Object);

        Fixture.PairerMock.Verify(static (handler) => handler.Handle(It.IsAny<IPairArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>()), Times.Never());
    }

    [Fact]
    public void SyntacticConstructorArguments_PairsNone()
    {
        var syntacticArgument = SyntaxFactory.AttributeArgument(null, null, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(42)));

        Mock<IAssociateArgumentsCommand<IAssociateCSharpAttributeNamedArgumentsData>> commandMock = new();

        commandMock.Setup(static (command) => command.Data.SyntacticArguments).Returns([syntacticArgument]);

        Target(commandMock.Object);

        Fixture.PairerMock.Verify(static (handler) => handler.Handle(It.IsAny<IPairArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>()), Times.Never());
    }

    [Fact]
    public void SomeSyntacticArguments_PairsAll()
    {
        var parameter1Name = "Name1";
        var parameter2Name = "Name2";

        var syntacticArgument1 = SyntaxFactory.AttributeArgument(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(parameter1Name)), null, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(42)));
        var syntacticArgument2 = SyntaxFactory.AttributeArgument(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(parameter2Name)), null, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(43)));

        Mock<IAssociateArgumentsCommand<IAssociateCSharpAttributeNamedArgumentsData>> commandMock = new();

        commandMock.Setup(static (command) => command.Data.SyntacticArguments).Returns([syntacticArgument1, syntacticArgument2]);

        Target(commandMock.Object);

        Fixture.PairerMock.Verify(PairArgumentExpression(parameter1Name, syntacticArgument1), Times.Once());
        Fixture.PairerMock.Verify(PairArgumentExpression(parameter2Name, syntacticArgument2), Times.Once());
        Fixture.PairerMock.Verify(static (handler) => handler.Handle(It.IsAny<IPairArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>()), Times.Exactly(2));
    }

    private static Expression<Action<ICommandHandler<IPairArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>>> PairArgumentExpression(
        string parameterName,
        AttributeArgumentSyntax syntacticArgument)
    {
        return (handler) => handler.Handle(It.Is(MatchPairArgumentCommand(parameterName, syntacticArgument)));
    }

    private static Expression<Func<IPairArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>, bool>> MatchPairArgumentCommand(
        string parameterName,
        AttributeArgumentSyntax syntacticArgument)
    {
        return (command) => MatchParameter(parameterName, command.Parameter) && MatchArgumentData(syntacticArgument, command.ArgumentData);
    }

    private static bool MatchParameter(
        string parameterName,
        INamedParameter parameter)
    {
        return Equals(parameterName, parameter.Name);
    }

    private static bool MatchArgumentData(
        AttributeArgumentSyntax syntacticArgument,
        ICSharpAttributeNamedArgumentData argumentData)
    {
        return ReferenceEquals(syntacticArgument, argumentData.SyntacticArgument);
    }

    private void Target(
        IAssociateArgumentsCommand<IAssociateCSharpAttributeNamedArgumentsData> command)
    {
        Fixture.Sut.Handle(command);
    }
}
