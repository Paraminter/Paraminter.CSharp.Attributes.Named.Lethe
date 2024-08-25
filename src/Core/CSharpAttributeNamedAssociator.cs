namespace Paraminter.CSharp.Attributes.Named.Lethe;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Paraminter.Arguments.CSharp.Attributes.Named.Models;
using Paraminter.Commands;
using Paraminter.Cqs.Handlers;
using Paraminter.CSharp.Attributes.Named.Lethe.Commands;
using Paraminter.CSharp.Attributes.Named.Lethe.Models;
using Paraminter.Parameters.Named.Models;

using System;

/// <summary>Associates syntactic C# named attribute arguments with parameters.</summary>
public sealed class CSharpAttributeNamedAssociator
    : ICommandHandler<IAssociateAllArgumentsCommand<IAssociateAllCSharpAttributeNamedArgumentsData>>
{
    private readonly ICommandHandler<IAssociateSingleArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>> IndividualAssociator;

    /// <summary>Instantiates an associator of syntactic C# named attribute arguments with parameters.</summary>
    /// <param name="individualAssociator">Associates individual syntactic C# named attribute arguments with parameters.</param>
    public CSharpAttributeNamedAssociator(
        ICommandHandler<IAssociateSingleArgumentCommand<INamedParameter, ICSharpAttributeNamedArgumentData>> individualAssociator)
    {
        IndividualAssociator = individualAssociator ?? throw new ArgumentNullException(nameof(individualAssociator));
    }

    void ICommandHandler<IAssociateAllArgumentsCommand<IAssociateAllCSharpAttributeNamedArgumentsData>>.Handle(
        IAssociateAllArgumentsCommand<IAssociateAllCSharpAttributeNamedArgumentsData> command)
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

            AssociateArgument(nameEqualsSyntax.Name.Identifier.Text, syntacticArgument);
        }
    }

    private void AssociateArgument(
        string parameterName,
        AttributeArgumentSyntax syntacticArgument)
    {
        var parameter = new NamedParameter(parameterName);
        var argumentData = new CSharpAttributeNamedArgumentData(syntacticArgument);

        var command = new AssociateSingleArgumentCommand(parameter, argumentData);

        IndividualAssociator.Handle(command);
    }
}
