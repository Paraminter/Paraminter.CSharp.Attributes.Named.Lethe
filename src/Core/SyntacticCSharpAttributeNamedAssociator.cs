namespace Paraminter.CSharp.Attributes.Named.Lethe;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Paraminter.Arguments.CSharp.Attributes.Named.Models;
using Paraminter.Associators.Commands;
using Paraminter.Commands.Handlers;
using Paraminter.CSharp.Attributes.Named.Lethe.Common;
using Paraminter.CSharp.Attributes.Named.Lethe.Models;
using Paraminter.Parameters.Named.Models;
using Paraminter.Recorders.Commands;

using System;

/// <summary>Associates syntactic C# named attribute arguments.</summary>
public sealed class SyntacticCSharpAttributeNamedAssociator
    : ICommandHandler<IAssociateArgumentsCommand<IAssociateSyntacticCSharpAttributeNamedData>>
{
    private readonly ICommandHandler<IRecordArgumentAssociationCommand<INamedParameter, ICSharpAttributeNamedArgumentData>> Recorder;

    /// <summary>Instantiates a <see cref="SyntacticCSharpAttributeNamedAssociator"/>, associating syntactic C# named attribute arguments.</summary>
    /// <param name="recorder">Records associated syntactic C# named attribute arguments.</param>
    public SyntacticCSharpAttributeNamedAssociator(
        ICommandHandler<IRecordArgumentAssociationCommand<INamedParameter, ICSharpAttributeNamedArgumentData>> recorder)
    {
        Recorder = recorder ?? throw new ArgumentNullException(nameof(recorder));
    }

    void ICommandHandler<IAssociateArgumentsCommand<IAssociateSyntacticCSharpAttributeNamedData>>.Handle(
        IAssociateArgumentsCommand<IAssociateSyntacticCSharpAttributeNamedData> command)
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

            var parameter = new NamedParameter(nameEqualsSyntax.Name.Identifier.Text);
            var argumentData = new CSharpAttributeNamedArgumentData(syntacticArgument);

            var recordCommand = new RecordCSharpAttributeNamedAssociationCommand(parameter, argumentData);

            Recorder.Handle(recordCommand);
        }
    }
}
