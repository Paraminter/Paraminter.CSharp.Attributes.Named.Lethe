namespace Paraminter.CSharp.Attributes.Named.Lethe.Commands;

using Paraminter.Arguments.CSharp.Attributes.Named.Models;
using Paraminter.Commands;
using Paraminter.Parameters.Named.Models;

internal sealed class AssociateSingleArgumentCommand
    : IAssociateSingleArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>
{
    private readonly INamedParameter Parameter;
    private readonly ICSharpAttributeNamedArgumentData ArgumentData;

    public AssociateSingleArgumentCommand(
        INamedParameter parameter,
        ICSharpAttributeNamedArgumentData argumentData)
    {
        Parameter = parameter;
        ArgumentData = argumentData;
    }

    INamedParameter IAssociateSingleArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>.Parameter => Parameter;
    ICSharpAttributeNamedArgumentData IAssociateSingleArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>.ArgumentData => ArgumentData;
}
