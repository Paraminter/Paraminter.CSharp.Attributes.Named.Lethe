namespace Paraminter.CSharp.Attributes.Named.Lethe;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using Paraminter.Arguments.CSharp.Attributes.Named.Models;
using Paraminter.Commands;
using Paraminter.Cqs.Handlers;
using Paraminter.CSharp.Attributes.Named.Lethe.Models;
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
    public void NoSyntacticArguments_AssociatesNone()
    {
        Mock<IAssociateAllArgumentsCommand<IAssociateAllCSharpAttributeNamedArgumentsData>> commandMock = new();

        commandMock.Setup(static (command) => command.Data.SyntacticArguments).Returns([]);

        Target(commandMock.Object);

        Fixture.IndividualAssociatorMock.Verify(static (associator) => associator.Handle(It.IsAny<IAssociateSingleArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>()), Times.Never());
    }

    [Fact]
    public void SyntacticConstructorArguments_AssociatesNone()
    {
        var syntacticArgument = SyntaxFactory.AttributeArgument(null, null, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(42)));

        Mock<IAssociateAllArgumentsCommand<IAssociateAllCSharpAttributeNamedArgumentsData>> commandMock = new();

        commandMock.Setup(static (command) => command.Data.SyntacticArguments).Returns([syntacticArgument]);

        Target(commandMock.Object);

        Fixture.IndividualAssociatorMock.Verify(static (associator) => associator.Handle(It.IsAny<IAssociateSingleArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>()), Times.Never());
    }

    [Fact]
    public void SomeSyntacticArguments_AssociatesAllPairwise()
    {
        var parameter1Name = "Name1";
        var parameter2Name = "Name2";

        var syntacticArgument1 = SyntaxFactory.AttributeArgument(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(parameter1Name)), null, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(42)));
        var syntacticArgument2 = SyntaxFactory.AttributeArgument(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(parameter2Name)), null, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(43)));

        Mock<IAssociateAllArgumentsCommand<IAssociateAllCSharpAttributeNamedArgumentsData>> commandMock = new();

        commandMock.Setup(static (command) => command.Data.SyntacticArguments).Returns([syntacticArgument1, syntacticArgument2]);

        Target(commandMock.Object);

        Fixture.IndividualAssociatorMock.Verify(static (associator) => associator.Handle(It.IsAny<IAssociateSingleArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>()), Times.Exactly(2));
        Fixture.IndividualAssociatorMock.Verify(AssociateIndividualExpression(parameter1Name, syntacticArgument1), Times.Once());
        Fixture.IndividualAssociatorMock.Verify(AssociateIndividualExpression(parameter2Name, syntacticArgument2), Times.Once());
    }

    private static Expression<Action<ICommandHandler<IAssociateSingleArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>>> AssociateIndividualExpression(
        string parameterName,
        AttributeArgumentSyntax syntacticArgument)
    {
        return (associator) => associator.Handle(It.Is(MatchAssociateIndividualCommand(parameterName, syntacticArgument)));
    }

    private static Expression<Func<IAssociateSingleArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>, bool>> MatchAssociateIndividualCommand(
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
        IAssociateAllArgumentsCommand<IAssociateAllCSharpAttributeNamedArgumentsData> command)
    {
        Fixture.Sut.Handle(command);
    }
}
