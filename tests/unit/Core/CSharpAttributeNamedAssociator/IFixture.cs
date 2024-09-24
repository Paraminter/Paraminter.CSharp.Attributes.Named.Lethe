namespace Paraminter.Associating.CSharp.Attributes.Named.Lethe;

using Moq;

using Paraminter.Arguments.CSharp.Attributes.Named.Models;
using Paraminter.Associating.Commands;
using Paraminter.Associating.CSharp.Attributes.Named.Lethe.Models;
using Paraminter.Cqs.Handlers;
using Paraminter.Pairing.Commands;
using Paraminter.Parameters.Named.Models;

internal interface IFixture
{
    public abstract ICommandHandler<IAssociateArgumentsCommand<IAssociateCSharpAttributeNamedArgumentsData>> Sut { get; }

    public abstract Mock<ICommandHandler<IPairArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>>> PairerMock { get; }
}
