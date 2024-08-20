namespace Paraminter.CSharp.Attributes.Named.Lethe;

using Moq;

using Paraminter.Arguments.CSharp.Attributes.Named.Models;
using Paraminter.Commands;
using Paraminter.Cqs.Handlers;
using Paraminter.CSharp.Attributes.Named.Lethe.Models;
using Paraminter.Parameters.Named.Models;

internal static class FixtureFactory
{
    public static IFixture Create()
    {
        var individualAssociatorMock = new Mock<ICommandHandler<IAssociateSingleArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>>();

        SyntacticCSharpAttributeNamedAssociator sut = new(individualAssociatorMock.Object);

        return new Fixture(sut, individualAssociatorMock);
    }

    private sealed class Fixture
        : IFixture
    {
        private readonly ICommandHandler<IAssociateAllArgumentsCommand<IAssociateAllSyntacticCSharpAttributeNamedArgumentsData>> Sut;

        private readonly Mock<ICommandHandler<IAssociateSingleArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>> IndividualAssociatorMock;

        public Fixture(
            ICommandHandler<IAssociateAllArgumentsCommand<IAssociateAllSyntacticCSharpAttributeNamedArgumentsData>> sut,
            Mock<ICommandHandler<IAssociateSingleArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>> individualAssociatorMock)
        {
            Sut = sut;

            IndividualAssociatorMock = individualAssociatorMock;
        }

        ICommandHandler<IAssociateAllArgumentsCommand<IAssociateAllSyntacticCSharpAttributeNamedArgumentsData>> IFixture.Sut => Sut;

        Mock<ICommandHandler<IAssociateSingleArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>> IFixture.IndividualAssociatorMock => IndividualAssociatorMock;
    }
}
