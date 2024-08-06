namespace Paraminter.CSharp.Attributes.Named.Lethe;

using Moq;

using Paraminter.Arguments.CSharp.Attributes.Named.Models;

using Paraminter.Associators.Commands;
using Paraminter.Commands.Handlers;
using Paraminter.CSharp.Attributes.Named.Lethe.Models;
using Paraminter.Parameters.Named.Models;

internal static class FixtureFactory
{
    public static IFixture Create()
    {
        var recorderMock = new Mock<ICommandHandler<IRecordArgumentAssociationCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>>();

        SyntacticCSharpAttributeNamedAssociator sut = new(recorderMock.Object);

        return new Fixture(sut, recorderMock);
    }

    private sealed class Fixture
        : IFixture
    {
        private readonly ICommandHandler<IAssociateArgumentsCommand<IAssociateSyntacticCSharpAttributeNamedData>> Sut;

        private readonly Mock<ICommandHandler<IRecordArgumentAssociationCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>> RecorderMock;

        public Fixture(
            ICommandHandler<IAssociateArgumentsCommand<IAssociateSyntacticCSharpAttributeNamedData>> sut,
            Mock<ICommandHandler<IRecordArgumentAssociationCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>> recorderMock)
        {
            Sut = sut;

            RecorderMock = recorderMock;
        }

        ICommandHandler<IAssociateArgumentsCommand<IAssociateSyntacticCSharpAttributeNamedData>> IFixture.Sut => Sut;

        Mock<ICommandHandler<IRecordArgumentAssociationCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>> IFixture.RecorderMock => RecorderMock;
    }
}
