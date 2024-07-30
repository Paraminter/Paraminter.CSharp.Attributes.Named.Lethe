namespace Paraminter.CSharp.Attributes.Named.Lethe;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using Paraminter.Associators.Queries;
using Paraminter.CSharp.Attributes.Named.Commands;
using Paraminter.CSharp.Attributes.Named.Lethe.Queries;
using Paraminter.CSharp.Attributes.Named.Queries.Handlers;

using System;
using System.Linq;
using System.Linq.Expressions;

using Xunit;

public sealed class Handle
{
    private readonly IFixture Fixture = FixtureFactory.Create();

    [Fact]
    public void AttributeUsage_AssociatesAll()
    {
        var source = """
            using System;

            public class CustomAttribute : Attribute
            {
                public double ValueA { get; set; }
                public double ValueB { get; set; }
            }

            [CustomAttribute(ValueA = 4.2, ValueB = 42)]
            public class Foo { }
            """;

        var compilation = CompilationFactory.Create(source);
        var attribute = compilation.GetTypeByMetadataName("Foo")!.GetAttributes()[0];

        var attributeSyntax = (AttributeSyntax)attribute.ApplicationSyntaxReference!.GetSyntax();

        var syntacticArguments = attributeSyntax.ArgumentList!.Arguments;
        var parameters = syntacticArguments.Select(static (syntacticArgument) => syntacticArgument.NameEquals!.Name.Identifier.Text).ToArray();

        Mock<IAssociateArgumentsQuery<IAssociateSyntacticCSharpAttributeNamedData>> queryMock = new();
        Mock<IAssociateSyntacticCSharpAttributeNamedQueryResponseHandler> queryResponseHandlerMock = new() { DefaultValue = DefaultValue.Mock };

        queryMock.Setup((query) => query.Data.SyntacticArguments).Returns(syntacticArguments);

        Target(queryMock.Object, queryResponseHandlerMock.Object);

        queryResponseHandlerMock.Verify(static (handler) => handler.AssociationCollector.Handle(It.IsAny<IAddCSharpAttributeNamedAssociationCommand>()), Times.Exactly(2));
        queryResponseHandlerMock.Verify(AssociationExpression(parameters[0], syntacticArguments[0]), Times.Once());
        queryResponseHandlerMock.Verify(AssociationExpression(parameters[1], syntacticArguments[1]), Times.Once());
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
