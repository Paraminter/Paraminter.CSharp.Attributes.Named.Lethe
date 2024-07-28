namespace Paraminter.CSharp.Attributes.Named.Lethe;

using Paraminter.Associators.Queries;
using Paraminter.CSharp.Attributes.Named.Lethe.Queries;
using Paraminter.CSharp.Attributes.Named.Queries.Collectors;
using Paraminter.Queries.Handlers;

internal static class FixtureFactory
{
    public static IFixture Create()
    {
        SyntacticCSharpAttributeNamedAssociator sut = new();

        return new Fixture(sut);
    }

    private sealed class Fixture
        : IFixture
    {
        private readonly IQueryHandler<IAssociateArgumentsQuery<IAssociateSyntacticCSharpAttributeNamedData>, IAssociateSyntacticCSharpAttributeNamedQueryResponseCollector> Sut;

        public Fixture(
            IQueryHandler<IAssociateArgumentsQuery<IAssociateSyntacticCSharpAttributeNamedData>, IAssociateSyntacticCSharpAttributeNamedQueryResponseCollector> sut)
        {
            Sut = sut;
        }

        IQueryHandler<IAssociateArgumentsQuery<IAssociateSyntacticCSharpAttributeNamedData>, IAssociateSyntacticCSharpAttributeNamedQueryResponseCollector> IFixture.Sut => Sut;
    }
}
