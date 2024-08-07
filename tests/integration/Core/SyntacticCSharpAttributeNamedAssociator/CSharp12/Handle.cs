namespace Paraminter.CSharp.Attributes.Named.Lethe;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using Paraminter.Arguments.CSharp.Attributes.Named.Models;
using Paraminter.Associators.Commands;
using Paraminter.Commands.Handlers;
using Paraminter.CSharp.Attributes.Named.Lethe.Models;
using Paraminter.Parameters.Named.Models;
using Paraminter.Recorders.Commands;

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

        Mock<IAssociateArgumentsCommand<IAssociateSyntacticCSharpAttributeNamedData>> commandMock = new();

        commandMock.Setup(static (command) => command.Data.SyntacticArguments).Returns(syntacticArguments);

        Target(commandMock.Object);

        Fixture.RecorderMock.Verify(static (recorder) => recorder.Handle(It.IsAny<IRecordArgumentAssociationCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>()), Times.Exactly(2));
        Fixture.RecorderMock.Verify(RecordExpression(parameters[0], syntacticArguments[0]), Times.Once());
        Fixture.RecorderMock.Verify(RecordExpression(parameters[1], syntacticArguments[1]), Times.Once());
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
