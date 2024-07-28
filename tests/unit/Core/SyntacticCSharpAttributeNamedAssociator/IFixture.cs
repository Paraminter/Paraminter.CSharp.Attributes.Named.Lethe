﻿namespace Paraminter.CSharp.Attributes.Named.Lethe;

using Paraminter.Associators.Queries;
using Paraminter.CSharp.Attributes.Named.Lethe.Queries;
using Paraminter.CSharp.Attributes.Named.Queries.Collectors;
using Paraminter.Queries.Handlers;

internal interface IFixture
{
    public abstract IQueryHandler<IAssociateArgumentsQuery<IAssociateSyntacticCSharpAttributeNamedData>, IAssociateSyntacticCSharpAttributeNamedQueryResponseCollector> Sut { get; }
}
