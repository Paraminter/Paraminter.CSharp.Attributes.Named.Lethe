namespace Paraminter.Associating.CSharp.Attributes.Named.Lethe;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using Paraminter.Arguments.CSharp.Attributes.Named.Models;
using Paraminter.Associating.Commands;
using Paraminter.Associating.CSharp.Attributes.Named.Lethe.Models;
using Paraminter.Pairing.Commands;
using Paraminter.Parameters.Named.Models;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

public sealed class Handle
{
    private readonly IFixture Fixture = FixtureFactory.Create();

    [Fact]
    public async Task AttributeUsage_PairsAll()
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

        var attributeSyntax = (AttributeSyntax)await attribute.ApplicationSyntaxReference!.GetSyntaxAsync(CancellationToken.None);
        var syntacticArguments = attributeSyntax.ArgumentList!.Arguments;
        var parameters = syntacticArguments.Select(static (syntacticArgument) => syntacticArgument.NameEquals!.Name.Identifier.Text).ToArray();

        Mock<IAssociateArgumentsCommand<IAssociateCSharpAttributeNamedArgumentsData>> commandMock = new();

        commandMock.Setup(static (command) => command.Data.SyntacticArguments).Returns(syntacticArguments);

        await Target(commandMock.Object, CancellationToken.None);

        Fixture.PairerMock.Verify(PairArgumentExpression(parameters[0], syntacticArguments[0], It.IsAny<CancellationToken>()), Times.Once());
        Fixture.PairerMock.Verify(PairArgumentExpression(parameters[1], syntacticArguments[1], It.IsAny<CancellationToken>()), Times.Once());
        Fixture.PairerMock.Verify(static (handler) => handler.Handle(It.IsAny<IPairArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    private static Expression<Func<ICommandHandler<IPairArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>, Task>> PairArgumentExpression(
        string parameterName,
        AttributeArgumentSyntax syntacticArgument,
        CancellationToken cancellationToken)
    {
        return (handler) => handler.Handle(It.Is(MatchPairArgumentCommand(parameterName, syntacticArgument)), cancellationToken);
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

    private async Task Target(
        IAssociateArgumentsCommand<IAssociateCSharpAttributeNamedArgumentsData> command,
        CancellationToken cancellationToken)
    {
        await Fixture.Sut.Handle(command, cancellationToken);
    }
}
