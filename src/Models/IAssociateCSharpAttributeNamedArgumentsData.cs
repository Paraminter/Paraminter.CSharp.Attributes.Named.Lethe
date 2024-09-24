namespace Paraminter.Associating.CSharp.Attributes.Named.Lethe.Models;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Paraminter.Associating.Models;

using System.Collections.Generic;

/// <summary>Represents data used to associate syntactic C# named attribute arguments with parameters.</summary>
public interface IAssociateCSharpAttributeNamedArgumentsData
    : IAssociateArgumentsData
{
    /// <summary>The syntactic C# named attribute arguments, possibly also containing syntactic C# constructor attribute arguments.</summary>
    public abstract IReadOnlyList<AttributeArgumentSyntax> SyntacticArguments { get; }
}
