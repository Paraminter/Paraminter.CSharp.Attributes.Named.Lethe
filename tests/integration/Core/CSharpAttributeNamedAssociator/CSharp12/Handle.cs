namespace Paraminter.Associating.CSharp.Attributes.Named.Lethe;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using Paraminter.Arguments.CSharp.Attributes.Named.Models;
using Paraminter.Associating.Commands;
using Paraminter.Associating.CSharp.Attributes.Named.Lethe.Models;
using Paraminter.Cqs;
using Paraminter.Pairing.Commands;
using Paraminter.Parameters.Named.Models;

using System;
using System.Linq;
using System.Linq.Expressions;

using Xunit;

public sealed class Handle
{
    private readonly IFixture Fixture = FixtureFactory.Create();

    [Fact]
    public void AttributeUsage_PairsAll()
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

        Mock<IAssociateArgumentsCommand<IAssociateCSharpAttributeNamedArgumentsData>> commandMock = new();

        commandMock.Setup(static (command) => command.Data.SyntacticArguments).Returns(syntacticArguments);

        Target(commandMock.Object);

        Fixture.PairerMock.Verify(PairArgumentExpression(parameters[0], syntacticArguments[0]), Times.Once());
        Fixture.PairerMock.Verify(PairArgumentExpression(parameters[1], syntacticArguments[1]), Times.Once());
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
