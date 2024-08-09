namespace Paraminter.CSharp.Attributes.Named.Lethe;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using Paraminter.Arguments.CSharp.Attributes.Named.Models;
using Paraminter.Commands;
using Paraminter.Cqs.Handlers;
using Paraminter.CSharp.Attributes.Named.Lethe.Models;
using Paraminter.Parameters.Named.Models;
using Paraminter.Recorders.Commands;

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
    public void NoSyntacticArguments_RecordsNone()
    {
        Mock<IAssociateArgumentsCommand<IAssociateSyntacticCSharpAttributeNamedData>> commandMock = new();

        commandMock.Setup(static (command) => command.Data.SyntacticArguments).Returns([]);

        Target(commandMock.Object);

        Fixture.RecorderMock.Verify(static (recorder) => recorder.Handle(It.IsAny<IRecordArgumentAssociationCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>()), Times.Never());
    }

    [Fact]
    public void SyntacticConstructorArguments_RecordsNone()
    {
        var syntacticArgument = SyntaxFactory.AttributeArgument(null, null, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(42)));

        Mock<IAssociateArgumentsCommand<IAssociateSyntacticCSharpAttributeNamedData>> commandMock = new();

        commandMock.Setup(static (command) => command.Data.SyntacticArguments).Returns([syntacticArgument]);

        Target(commandMock.Object);

        Fixture.RecorderMock.Verify(static (recorder) => recorder.Handle(It.IsAny<IRecordArgumentAssociationCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>()), Times.Never());
    }

    [Fact]
    public void SomeSyntacticArguments_RecordsAllPairwise()
    {
        var parameter1Name = "Name1";
        var parameter2Name = "Name2";

        var syntacticArgument1 = SyntaxFactory.AttributeArgument(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(parameter1Name)), null, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(42)));
        var syntacticArgument2 = SyntaxFactory.AttributeArgument(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(parameter2Name)), null, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(43)));

        Mock<IAssociateArgumentsCommand<IAssociateSyntacticCSharpAttributeNamedData>> commandMock = new();

        commandMock.Setup(static (command) => command.Data.SyntacticArguments).Returns([syntacticArgument1, syntacticArgument2]);

        Target(commandMock.Object);

        Fixture.RecorderMock.Verify(static (recorder) => recorder.Handle(It.IsAny<IRecordArgumentAssociationCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>()), Times.Exactly(2));
        Fixture.RecorderMock.Verify(RecordExpression(parameter1Name, syntacticArgument1), Times.Once());
        Fixture.RecorderMock.Verify(RecordExpression(parameter2Name, syntacticArgument2), Times.Once());
    }

    private static Expression<Action<ICommandHandler<IRecordArgumentAssociationCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>>> RecordExpression(
        string parameterName,
        AttributeArgumentSyntax syntacticArgument)
    {
        return (recorder) => recorder.Handle(It.Is(MatchRecordCommand(parameterName, syntacticArgument)));
    }

    private static Expression<Func<IRecordArgumentAssociationCommand<INamedParameter, ICSharpAttributeNamedArgumentData>, bool>> MatchRecordCommand(
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
        IAssociateArgumentsCommand<IAssociateSyntacticCSharpAttributeNamedData> command)
    {
        Fixture.Sut.Handle(command);
    }
}
