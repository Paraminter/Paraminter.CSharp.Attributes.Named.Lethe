namespace Paraminter.CSharp.Attributes.Named.Lethe;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Paraminter.Associators.Queries;
using Paraminter.CSharp.Attributes.Named.Lethe.Common;
using Paraminter.CSharp.Attributes.Named.Lethe.Queries;
using Paraminter.CSharp.Attributes.Named.Queries.Handlers;
using Paraminter.Queries.Handlers;

using System;

/// <summary>Associates syntactic C# named attribute arguments.</summary>
public sealed class SyntacticCSharpAttributeNamedAssociator
    : IQueryHandler<IAssociateArgumentsQuery<IAssociateSyntacticCSharpAttributeNamedData>, IAssociateSyntacticCSharpAttributeNamedQueryResponseHandler>
{
    /// <summary>Instantiates a <see cref="SyntacticCSharpAttributeNamedAssociator"/>, associating syntactic C# type arguments.</summary>
    public SyntacticCSharpAttributeNamedAssociator() { }

    void IQueryHandler<IAssociateArgumentsQuery<IAssociateSyntacticCSharpAttributeNamedData>, IAssociateSyntacticCSharpAttributeNamedQueryResponseHandler>.Handle(
        IAssociateArgumentsQuery<IAssociateSyntacticCSharpAttributeNamedData> query,
        IAssociateSyntacticCSharpAttributeNamedQueryResponseHandler queryResponseHandler)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        if (queryResponseHandler is null)
        {
            throw new ArgumentNullException(nameof(queryResponseHandler));
        }

        foreach (var syntacticArgument in query.Data.SyntacticArguments)
        {
            if (syntacticArgument.NameEquals is not NameEqualsSyntax nameEqualsSyntax)
            {
                continue;
            }

            var parameterName = nameEqualsSyntax.Name.Identifier.Text;

            var command = new AddCSharpAttributeNamedAssociationCommand(parameterName, syntacticArgument);

            queryResponseHandler.AssociationCollector.Handle(command);
        }
    }
}
