namespace Paraminter.Associating.CSharp.Attributes.Named.Lethe;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Paraminter.Arguments.CSharp.Attributes.Named.Models;
using Paraminter.Associating.Commands;
using Paraminter.Associating.CSharp.Attributes.Named.Lethe.Commands;
using Paraminter.Associating.CSharp.Attributes.Named.Lethe.Models;
using Paraminter.Pairing.Commands;
using Paraminter.Parameters.Named.Models;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>Associates syntactic C# named attribute arguments with parameters.</summary>
public sealed class CSharpAttributeNamedAssociator
    : ICommandHandler<IAssociateArgumentsCommand<IAssociateCSharpAttributeNamedArgumentsData>>
{
    private readonly ICommandHandler<IPairArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>> Pairer;

    /// <summary>Instantiates an associator of syntactic C# named attribute arguments with parameters.</summary>
    /// <param name="pairer">Pairs syntactic C# named attribute arguments with parameters.</param>
    public CSharpAttributeNamedAssociator(
        ICommandHandler<IPairArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>> pairer)
    {
        Pairer = pairer ?? throw new ArgumentNullException(nameof(pairer));
    }

    async Task ICommandHandler<IAssociateArgumentsCommand<IAssociateCSharpAttributeNamedArgumentsData>>.Handle(
        IAssociateArgumentsCommand<IAssociateCSharpAttributeNamedArgumentsData> command,
        CancellationToken cancellationToken)
    {
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        foreach (var syntacticArgument in command.Data.SyntacticArguments)
        {
            if (syntacticArgument.NameEquals is not NameEqualsSyntax nameEqualsSyntax)
            {
                continue;
            }

            await PairArgument(nameEqualsSyntax.Name.Identifier.Text, syntacticArgument, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task PairArgument(
        string parameterName,
        AttributeArgumentSyntax syntacticArgument,
        CancellationToken cancellationToken)
    {
        var parameter = new NamedParameter(parameterName);
        var argumentData = new CSharpAttributeNamedArgumentData(syntacticArgument);

        var command = new PairArgumentCommand(parameter, argumentData);

        await Pairer.Handle(command, cancellationToken).ConfigureAwait(false);
    }
}
