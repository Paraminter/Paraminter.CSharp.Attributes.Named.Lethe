namespace Paraminter.Associating.CSharp.Attributes.Named.Lethe.Commands;

using Paraminter.Arguments.CSharp.Attributes.Named.Models;
using Paraminter.Pairing.Commands;
using Paraminter.Parameters.Named.Models;

internal sealed class PairArgumentCommand
    : IPairArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>
{
    private readonly INamedParameter Parameter;
    private readonly ICSharpAttributeNamedArgumentData ArgumentData;

    public PairArgumentCommand(
        INamedParameter parameter,
        ICSharpAttributeNamedArgumentData argumentData)
    {
        Parameter = parameter;
        ArgumentData = argumentData;
    }

    INamedParameter IPairArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>.Parameter => Parameter;
    ICSharpAttributeNamedArgumentData IPairArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>.ArgumentData => ArgumentData;
}
