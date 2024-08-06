namespace Paraminter.CSharp.Attributes.Named.Lethe;

using Moq;

using Paraminter.Arguments.CSharp.Attributes.Named.Models;

using Paraminter.Associators.Commands;
using Paraminter.Commands.Handlers;
using Paraminter.CSharp.Attributes.Named.Lethe.Models;
using Paraminter.Parameters.Named.Models;

internal interface IFixture
{
    public abstract ICommandHandler<IAssociateArgumentsCommand<IAssociateSyntacticCSharpAttributeNamedData>> Sut { get; }

    public abstract Mock<ICommandHandler<IRecordArgumentAssociationCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>> RecorderMock { get; }
}
