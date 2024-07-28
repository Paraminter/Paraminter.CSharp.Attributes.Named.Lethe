﻿namespace Paraminter.CSharp.Attributes.Named.Lethe;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Paraminter.Associators.Queries;
using Paraminter.CSharp.Attributes.Named.Lethe.Queries;
using Paraminter.CSharp.Attributes.Named.Queries.Collectors;
using Paraminter.Queries.Handlers;

using System;

/// <summary>Associates syntactic C# named attribute arguments.</summary>
public sealed class SyntacticCSharpAttributeNamedAssociator
    : IQueryHandler<IAssociateArgumentsQuery<IAssociateSyntacticCSharpAttributeNamedData>, IAssociateSyntacticCSharpAttributeNamedQueryResponseCollector>
{
    /// <summary>Instantiates a <see cref="SyntacticCSharpAttributeNamedAssociator"/>, associating syntactic C# type arguments.</summary>
    public SyntacticCSharpAttributeNamedAssociator() { }

    void IQueryHandler<IAssociateArgumentsQuery<IAssociateSyntacticCSharpAttributeNamedData>, IAssociateSyntacticCSharpAttributeNamedQueryResponseCollector>.Handle(
        IAssociateArgumentsQuery<IAssociateSyntacticCSharpAttributeNamedData> query,
        IAssociateSyntacticCSharpAttributeNamedQueryResponseCollector queryResponseCollector)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        if (queryResponseCollector is null)
        {
            throw new ArgumentNullException(nameof(queryResponseCollector));
        }

        foreach (var syntacticArgument in query.Data.SyntacticArguments)
        {
            if (syntacticArgument.NameEquals is not NameEqualsSyntax nameEqualsSyntax)
            {
                continue;
            }

            var parameterName = nameEqualsSyntax.Name.Identifier.Text;

            queryResponseCollector.Associations.Add(parameterName, syntacticArgument);
        }
    }
}
