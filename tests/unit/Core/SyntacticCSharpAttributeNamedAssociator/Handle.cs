namespace Paraminter.CSharp.Attributes.Named.Lethe;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using Paraminter.Associators.Queries;
using Paraminter.CSharp.Attributes.Named.Lethe.Queries;
using Paraminter.CSharp.Attributes.Named.Queries.Collectors;

using System;

using Xunit;

public sealed class Handle
{
    private readonly IFixture Fixture = FixtureFactory.Create();

    [Fact]
    public void NullQuery_ThrowsArgumentNullException()
    {
        var result = Record.Exception(() => Target(null!, Mock.Of<IAssociateSyntacticCSharpAttributeNamedQueryResponseCollector>()));

        Assert.IsType<ArgumentNullException>(result);
    }

    [Fact]
    public void NullQueryResponseCollector_ThrowsArgumentNullException()
    {
        var result = Record.Exception(() => Target(Mock.Of<IAssociateArgumentsQuery<IAssociateSyntacticCSharpAttributeNamedData>>(), null!));

        Assert.IsType<ArgumentNullException>(result);
    }

    [Fact]
    public void NoSyntacticArguments_AddsNone()
    {
        Mock<IAssociateArgumentsQuery<IAssociateSyntacticCSharpAttributeNamedData>> queryMock = new();
        Mock<IAssociateSyntacticCSharpAttributeNamedQueryResponseCollector> queryResponseCollectorMock = new() { DefaultValue = DefaultValue.Mock };

        queryMock.Setup(static (query) => query.Data.SyntacticArguments).Returns([]);

        Target(queryMock.Object, queryResponseCollectorMock.Object);

        queryResponseCollectorMock.Verify(static (collector) => collector.Associations.Add(It.IsAny<string>(), It.IsAny<AttributeArgumentSyntax>()), Times.Never());
    }

    [Fact]
    public void SyntacticConstructorArguments_AddsNone()
    {
        var syntacticArgument = SyntaxFactory.AttributeArgument(null, null, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(42)));

        Mock<IAssociateArgumentsQuery<IAssociateSyntacticCSharpAttributeNamedData>> queryMock = new();
        Mock<IAssociateSyntacticCSharpAttributeNamedQueryResponseCollector> queryResponseCollectorMock = new() { DefaultValue = DefaultValue.Mock };

        queryMock.Setup(static (query) => query.Data.SyntacticArguments).Returns([syntacticArgument]);

        Target(queryMock.Object, queryResponseCollectorMock.Object);

        queryResponseCollectorMock.Verify(static (collector) => collector.Associations.Add(It.IsAny<string>(), It.IsAny<AttributeArgumentSyntax>()), Times.Never());
    }

    [Fact]
    public void SomeSyntacticArguments_AddsAllPairwise()
    {
        var parameter1Name = "Name1";
        var parameter2Name = "Name2";

        var syntacticArgument1 = SyntaxFactory.AttributeArgument(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(parameter1Name)), null, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(42)));
        var syntacticArgument2 = SyntaxFactory.AttributeArgument(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(parameter2Name)), null, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(43)));

        Mock<IAssociateArgumentsQuery<IAssociateSyntacticCSharpAttributeNamedData>> queryMock = new();
        Mock<IAssociateSyntacticCSharpAttributeNamedQueryResponseCollector> queryResponseCollectorMock = new() { DefaultValue = DefaultValue.Mock };

        queryMock.Setup((query) => query.Data.SyntacticArguments).Returns([syntacticArgument1, syntacticArgument2]);

        Target(queryMock.Object, queryResponseCollectorMock.Object);

        queryResponseCollectorMock.Verify(static (collector) => collector.Associations.Add(It.IsAny<string>(), It.IsAny<AttributeArgumentSyntax>()), Times.Exactly(2));
        queryResponseCollectorMock.Verify((collector) => collector.Associations.Add(parameter1Name, syntacticArgument1), Times.Once());
        queryResponseCollectorMock.Verify((collector) => collector.Associations.Add(parameter2Name, syntacticArgument2), Times.Once());
    }

    private void Target(
        IAssociateArgumentsQuery<IAssociateSyntacticCSharpAttributeNamedData> query,
        IAssociateSyntacticCSharpAttributeNamedQueryResponseCollector queryResponseCollector)
    {
        Fixture.Sut.Handle(query, queryResponseCollector);
    }
}
