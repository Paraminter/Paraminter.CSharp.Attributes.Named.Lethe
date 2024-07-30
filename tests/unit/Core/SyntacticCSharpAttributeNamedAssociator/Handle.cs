namespace Paraminter.CSharp.Attributes.Named.Lethe;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using Paraminter.Associators.Queries;
using Paraminter.CSharp.Attributes.Named.Commands;
using Paraminter.CSharp.Attributes.Named.Lethe.Queries;
using Paraminter.CSharp.Attributes.Named.Queries.Handlers;

using System;
using System.Linq.Expressions;

using Xunit;

public sealed class Handle
{
    private readonly IFixture Fixture = FixtureFactory.Create();

    [Fact]
    public void NullQuery_ThrowsArgumentNullException()
    {
        var result = Record.Exception(() => Target(null!, Mock.Of<IAssociateSyntacticCSharpAttributeNamedQueryResponseHandler>()));

        Assert.IsType<ArgumentNullException>(result);
    }

    [Fact]
    public void NullQueryResponseHandler_ThrowsArgumentNullException()
    {
        var result = Record.Exception(() => Target(Mock.Of<IAssociateArgumentsQuery<IAssociateSyntacticCSharpAttributeNamedData>>(), null!));

        Assert.IsType<ArgumentNullException>(result);
    }

    [Fact]
    public void NoSyntacticArguments_AddsNone()
    {
        Mock<IAssociateArgumentsQuery<IAssociateSyntacticCSharpAttributeNamedData>> queryMock = new();
        Mock<IAssociateSyntacticCSharpAttributeNamedQueryResponseHandler> queryResponseHandlerMock = new() { DefaultValue = DefaultValue.Mock };

        queryMock.Setup(static (query) => query.Data.SyntacticArguments).Returns([]);

        Target(queryMock.Object, queryResponseHandlerMock.Object);

        queryResponseHandlerMock.Verify(static (handler) => handler.AssociationCollector.Handle(It.IsAny<IAddCSharpAttributeNamedAssociationCommand>()), Times.Never());
    }

    [Fact]
    public void SyntacticConstructorArguments_AddsNone()
    {
        var syntacticArgument = SyntaxFactory.AttributeArgument(null, null, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(42)));

        Mock<IAssociateArgumentsQuery<IAssociateSyntacticCSharpAttributeNamedData>> queryMock = new();
        Mock<IAssociateSyntacticCSharpAttributeNamedQueryResponseHandler> queryResponseHandlerMock = new() { DefaultValue = DefaultValue.Mock };

        queryMock.Setup(static (query) => query.Data.SyntacticArguments).Returns([syntacticArgument]);

        Target(queryMock.Object, queryResponseHandlerMock.Object);

        queryResponseHandlerMock.Verify(static (handler) => handler.AssociationCollector.Handle(It.IsAny<IAddCSharpAttributeNamedAssociationCommand>()), Times.Never());
    }

    [Fact]
    public void SomeSyntacticArguments_AddsAllPairwise()
    {
        var parameter1Name = "Name1";
        var parameter2Name = "Name2";

        var syntacticArgument1 = SyntaxFactory.AttributeArgument(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(parameter1Name)), null, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(42)));
        var syntacticArgument2 = SyntaxFactory.AttributeArgument(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(parameter2Name)), null, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(43)));

        Mock<IAssociateArgumentsQuery<IAssociateSyntacticCSharpAttributeNamedData>> queryMock = new();
        Mock<IAssociateSyntacticCSharpAttributeNamedQueryResponseHandler> queryResponseHandlerMock = new() { DefaultValue = DefaultValue.Mock };

        queryMock.Setup((query) => query.Data.SyntacticArguments).Returns([syntacticArgument1, syntacticArgument2]);

        Target(queryMock.Object, queryResponseHandlerMock.Object);

        queryResponseHandlerMock.Verify(static (handler) => handler.AssociationCollector.Handle(It.IsAny<IAddCSharpAttributeNamedAssociationCommand>()), Times.Exactly(2));
        queryResponseHandlerMock.Verify(AssociationExpression(parameter1Name, syntacticArgument1), Times.Once());
        queryResponseHandlerMock.Verify(AssociationExpression(parameter2Name, syntacticArgument2), Times.Once());
    }

    private static Expression<Action<IAssociateSyntacticCSharpAttributeNamedQueryResponseHandler>> AssociationExpression(
        string parameterName,
        AttributeArgumentSyntax syntacticArgument)
    {
        return (handler) => handler.AssociationCollector.Handle(It.Is(MatchAssociationCommand(parameterName, syntacticArgument)));
    }

    private static Expression<Func<IAddCSharpAttributeNamedAssociationCommand, bool>> MatchAssociationCommand(
        string parameterName,
        AttributeArgumentSyntax syntacticArgument)
    {
        return (command) => ReferenceEquals(command.ParameterName, parameterName) && ReferenceEquals(command.SyntacticArgument, syntacticArgument);
    }

    private void Target(
        IAssociateArgumentsQuery<IAssociateSyntacticCSharpAttributeNamedData> query,
        IAssociateSyntacticCSharpAttributeNamedQueryResponseHandler queryResponseHandler)
    {
        Fixture.Sut.Handle(query, queryResponseHandler);
    }
}
