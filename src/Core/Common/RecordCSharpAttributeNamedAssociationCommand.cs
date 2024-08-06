namespace Paraminter.CSharp.Attributes.Named.Lethe.Common;

using Paraminter.Arguments.CSharp.Attributes.Named.Models;
using Paraminter.Associators.Commands;
using Paraminter.Parameters.Named.Models;

internal sealed class RecordCSharpAttributeNamedAssociationCommand
    : IRecordArgumentAssociationCommand<INamedParameter, ICSharpAttributeNamedArgumentData>
{
    private readonly INamedParameter Parameter;
    private readonly ICSharpAttributeNamedArgumentData ArgumentData;

    public RecordCSharpAttributeNamedAssociationCommand(
        INamedParameter parameter,
        ICSharpAttributeNamedArgumentData argumentData)
    {
        Parameter = parameter;
        ArgumentData = argumentData;
    }

    INamedParameter IRecordArgumentAssociationCommand<INamedParameter, ICSharpAttributeNamedArgumentData>.Parameter => Parameter;
    ICSharpAttributeNamedArgumentData IRecordArgumentAssociationCommand<INamedParameter, ICSharpAttributeNamedArgumentData>.ArgumentData => ArgumentData;
}
