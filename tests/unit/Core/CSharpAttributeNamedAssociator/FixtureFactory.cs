namespace Paraminter.Associating.CSharp.Attributes.Named.Lethe;

using Moq;

using Paraminter.Arguments.CSharp.Attributes.Named.Models;
using Paraminter.Associating.Commands;
using Paraminter.Associating.CSharp.Attributes.Named.Lethe.Models;
using Paraminter.Pairing.Commands;
using Paraminter.Parameters.Named.Models;

internal static class FixtureFactory
{
    public static IFixture Create()
    {
        Mock<ICommandHandler<IPairArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>> pairerMock = new();

        CSharpAttributeNamedAssociator sut = new(pairerMock.Object);

        return new Fixture(sut, pairerMock);
    }

    private sealed class Fixture
        : IFixture
    {
        private readonly ICommandHandler<IAssociateArgumentsCommand<IAssociateCSharpAttributeNamedArgumentsData>> Sut;

        private readonly Mock<ICommandHandler<IPairArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>> PairerMock;

        public Fixture(
            ICommandHandler<IAssociateArgumentsCommand<IAssociateCSharpAttributeNamedArgumentsData>> sut,
            Mock<ICommandHandler<IPairArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>> pairerMock)
        {
            Sut = sut;

            PairerMock = pairerMock;
        }

        ICommandHandler<IAssociateArgumentsCommand<IAssociateCSharpAttributeNamedArgumentsData>> IFixture.Sut => Sut;

        Mock<ICommandHandler<IPairArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>> IFixture.PairerMock => PairerMock;
    }
}
