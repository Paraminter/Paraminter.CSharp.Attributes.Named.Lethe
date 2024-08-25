namespace Paraminter.CSharp.Attributes.Named.Lethe;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using Paraminter.Arguments.CSharp.Attributes.Named.Models;
using Paraminter.Commands;
using Paraminter.Cqs.Handlers;
using Paraminter.CSharp.Attributes.Named.Lethe.Models;
using Paraminter.Parameters.Named.Models;

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

        Mock<IAssociateAllArgumentsCommand<IAssociateAllCSharpAttributeNamedArgumentsData>> commandMock = new();

        commandMock.Setup(static (command) => command.Data.SyntacticArguments).Returns(syntacticArguments);

        Target(commandMock.Object);

        Fixture.IndividualAssociatorMock.Verify(static (associator) => associator.Handle(It.IsAny<IAssociateSingleArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>()), Times.Exactly(2));
        Fixture.IndividualAssociatorMock.Verify(AssociateIndividualExpression(parameters[0], syntacticArguments[0]), Times.Once());
        Fixture.IndividualAssociatorMock.Verify(AssociateIndividualExpression(parameters[1], syntacticArguments[1]), Times.Once());
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
