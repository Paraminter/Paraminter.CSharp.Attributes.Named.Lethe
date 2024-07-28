namespace Paraminter.CSharp.Attributes.Named.Lethe;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using Paraminter.Associators.Queries;
using Paraminter.CSharp.Attributes.Named.Lethe.Queries;
using Paraminter.CSharp.Attributes.Named.Queries.Collectors;

using System.Linq;

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
        Mock<IAssociateSyntacticCSharpAttributeNamedQueryResponseCollector> queryResponseCollectorMock = new() { DefaultValue = DefaultValue.Mock };

        queryMock.Setup((query) => query.Data.SyntacticArguments).Returns(syntacticArguments);

        Target(queryMock.Object, queryResponseCollectorMock.Object);

        queryResponseCollectorMock.Verify((collector) => collector.Associations.Add(parameters[0], syntacticArguments[0]), Times.Once());
        queryResponseCollectorMock.Verify((collector) => collector.Associations.Add(parameters[1], syntacticArguments[1]), Times.Once());
        queryResponseCollectorMock.Verify((collector) => collector.Associations.Add(It.IsAny<string>(), It.IsAny<AttributeArgumentSyntax>()), Times.Exactly(2));
    }

    private void Target(
        IAssociateArgumentsQuery<IAssociateSyntacticCSharpAttributeNamedData> query,
        IAssociateSyntacticCSharpAttributeNamedQueryResponseCollector queryResponseCollector)
    {
        Fixture.Sut.Handle(query, queryResponseCollector);
    }
}
