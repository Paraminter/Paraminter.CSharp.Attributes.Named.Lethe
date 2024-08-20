namespace Paraminter.CSharp.Attributes.Named.Lethe;

using Moq;

using Paraminter.Arguments.CSharp.Attributes.Named.Models;
using Paraminter.Commands;
using Paraminter.Cqs.Handlers;
using Paraminter.CSharp.Attributes.Named.Lethe.Models;
using Paraminter.Parameters.Named.Models;

internal interface IFixture
{
    public abstract ICommandHandler<IAssociateAllArgumentsCommand<IAssociateAllSyntacticCSharpAttributeNamedArgumentData>> Sut { get; }

    public abstract Mock<ICommandHandler<IAssociateSingleArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>> IndividualAssociatorMock { get; }
}
